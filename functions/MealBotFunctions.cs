using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using Azure.Data.Tables;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using MealBot.Services;
using Twilio.TwiML;

namespace MealBot;

public class MealBotFunctions
{
    private readonly string _rowKeyDateFormat = "yyyy-MM-dd";

    private readonly JsonSerializerOptions _jsonSerializerOptions;
    private readonly StorageService _storageService;

    public MealBotFunctions(JsonSerializerOptions jsonSerializerOptions, StorageService storageService, TwilioConfig twilioConfig)
    {
        _jsonSerializerOptions = jsonSerializerOptions;
        _storageService = storageService;
        TwilioClient.Init(twilioConfig.AccountSid, twilioConfig.AuthToken);
    }

    [FunctionName("AddMeal")]
    public async Task<IActionResult> AddMealAsync(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "meals")] HttpRequest req,
        ILogger log)
    {
        log.LogInformation($"HTTP Trigger of {req.Method} {req.Path} endpoint.");

        log.LogInformation("Getting request body");
        string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

        log.LogInformation("Attempting to deserialize body into `Meal` object");
        Meal mealData;
        try
        {
            mealData = JsonSerializer.Deserialize<Meal>(requestBody, _jsonSerializerOptions);
            log.LogInformation("Deserialized!");

        }
        catch (Exception e)
        {
            log.LogWarning("Failed to deserialize data into a `Meal` object");
            log.LogDebug(e, "Posted data: {requestBody}", requestBody);
            return new BadRequestObjectResult("Data posted isn't in the correct format");
        }

        await _storageService.AddMealAsync(mealData);

        return new OkResult();
    }

    [FunctionName("GetMeals")]
    public IActionResult GetMeals(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "meals")] HttpRequest req,
        ILogger log)
    {
        log.LogInformation($"HTTP Trigger of {req.Method} {req.Path} endpoint.");

        string day = req.Query["day"];
        if (!Enum.TryParse<Enums.Meals>(req.Query["mealType"], out Enums.Meals mealType)) ;

        var meals = _storageService.GetMeals(day, mealType);

        string json = JsonSerializer.Serialize(meals, _jsonSerializerOptions);

        return new ContentResult()
        {
            ContentType = "application/json",
            Content = json,
            StatusCode = 200
        };
    }

    [FunctionName("SmsMeal")]
    public void SmsMealAsync(
        [Table("Meals", Connection = "AzureWebJobsStorage")] TableClient client,
        // [TimerTrigger("* * 15 * * *")] TimerInfo timer,
        [TimerTrigger("%SMS_SCHEDULE%")] TimerInfo timer,
        ILogger log)
    {
        log.LogInformation("Timer Trigger hit");
        log.LogInformation($"Next run after this one {timer.Schedule.GetNextOccurrence(DateTime.Now).ToLongTimeString()}");

        Enums.Meals mealType = Meals.Dinner;
        string day = DateTimeOffset.Now.ToString(_rowKeyDateFormat);

        log.LogInformation("Getting meal from database");
        var meal = _storageService.GetMeals(day, mealType).FirstOrDefault();

        if (meal is null)
        {
            log.LogInformation($"{day} / {mealType.ToString()} has no data associated with it");
            return;
        }

        if (string.IsNullOrEmpty(meal.Main) && string.IsNullOrEmpty(meal.Side) && string.IsNullOrEmpty(meal.Vegetable))
        {
            log.LogInformation("Had a record, but no meal data");
            return;
        }

        MessageResource message;

        foreach (var f in _storageService.GetFamilyMembers())
        {
            log.LogInformation($"Attempting send to {f.FirstName} {f.LastName}");

            try
            {
                message = MessageResource.Create(
                   body: GetMealText(meal, f),
                   from: new Twilio.Types.PhoneNumber("+19704648286"),
                   to: new Twilio.Types.PhoneNumber(f.MobileNumber)
               );
            }
            catch (Exception e)
            {
                log.LogError(e, $"Error sending message to {f.FirstName} {f.LastName}");
            }
        }
    }

    [FunctionName("SmsHook")]
    public async Task<IActionResult> SmsHook(
        [Table("SmsMessages", Connection = "AzureWebJobsStorage")] TableClient client,
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "smsHook")] HttpRequest req,
        ILogger log)
    {
        log.LogInformation($"HTTP Trigger of {req.Method} {req.Path} endpoint.");

        var smsMessage = await _storageService.AddSmsMessage(req.Query);

        if (!smsMessage.Body.ToUpper().Contains("DINNER")) return new OkResult();


        string today = DateTime.Now.ToString(_rowKeyDateFormat);
        var meal = _storageService.GetMeals(today, Meals.Dinner).FirstOrDefault();
        if (meal is null) return new OkResult();

        string mealText = GetMealText(meal, _storageService.GetFamilyMember(smsMessage.From));

        return new OkObjectResult(mealText);
    }

    private string GetMealText(Meal meal, FamilyMemberEntity familyMember)
    {
        return $"Hey {familyMember.FirstName}! Tonight you will be enjoying "
             + (string.IsNullOrEmpty(meal.Main) ? "no main course" : meal.Main)
             + (string.IsNullOrEmpty(meal.Side) ? "" : $" with a side of {meal.Side}")
             + (string.IsNullOrEmpty(meal.Vegetable) ? "" : $" and yummy {meal.Vegetable}")
             + ".";
    }
}