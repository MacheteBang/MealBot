using System.Text.Json;
using Azure.Data.Tables;
using Microsoft.AspNetCore.Http;

namespace MealBot.Services;

public class StorageService
{
    private readonly string _rowKeyDateFormat = "yyyy-MM-dd";

    private readonly TableClient _mealsClient;
    private readonly TableClient _familyMembersClient;
    private readonly TableClient _smsMessagesClient;
    private readonly JsonSerializerOptions _jsonOptions;

    public StorageService(string connectionString, JsonSerializerOptions jsonOptions)
    {
        _mealsClient = new TableClient(connectionString, "Meals");
        _familyMembersClient = new TableClient(connectionString, "FamilyMembers");
        _smsMessagesClient = new TableClient(connectionString, "SmsMessages");
        _jsonOptions = jsonOptions;
    }

    public List<Meal> GetMeals(string day = null, Enums.Meals? mealType = null)
    {
        string dayQuery = (string.IsNullOrEmpty(day) ? "" : $"Day eq '{day}'");
        string mealTypeQuery = (mealType is null || mealType == Meals.Unknown ? "" : $"MealType eq '{mealType}'");


        string query = dayQuery
            + (!string.IsNullOrWhiteSpace(dayQuery) && !string.IsNullOrWhiteSpace(mealTypeQuery) ? " and " : "")
            + mealTypeQuery;

        var mealEntities = _mealsClient.Query<MealTableEntity>(query).ToList();

        return mealEntities
            .Select(e => JsonSerializer.Deserialize<Meal>(e.Meal, _jsonOptions))
            .ToList();
    }
    public async Task AddMealAsync(Meal meal)
    {
        var e = new MealTableEntity
        {
            RowKey = DateOnly.FromDateTime(meal.Day).ToString(_rowKeyDateFormat) + meal.MealType.ToString(),
            Timestamp = DateTimeOffset.Now,
            Day = DateOnly.FromDateTime(meal.Day).ToString(_rowKeyDateFormat),
            MealType = meal.MealType.ToString(),
            Meal = JsonSerializer.Serialize(meal, _jsonOptions)
        };

        try
        { // Add it
            await _mealsClient.AddEntityAsync<MealTableEntity>(e);
        }
        catch
        { // If that fails, then update what's there.
            await _mealsClient.UpdateEntityAsync<MealTableEntity>(e, Azure.ETag.All, TableUpdateMode.Replace);
        }

    }
    public List<FamilyMemberEntity> GetFamilyMembers()
    {
        return _familyMembersClient
            .Query<FamilyMemberEntity>("")
            .ToList();
    }
    public async Task<SmsMessageEntity> AddSmsMessage(IQueryCollection parameters)
    {
        var response = new SmsMessageEntity
        {
            PartitionKey = "MealBot",
            RowKey = Guid.NewGuid().ToString(),
            ToCountry = parameters["ToCountry"],
            ToState = parameters["ToState"],
            SmsMessageSid = parameters["SmsMessageSid"],
            ToCity = parameters["ToCity"],
            FromZip = parameters["FromZip"],
            SmsSid = parameters["SmsSid"],
            FromState = parameters["FromState"],
            SmsStatus = parameters["SmsStatus"],
            FromCity = parameters["FromCity"],
            Body = parameters["Body"],
            FromCountry = parameters["FromCountry"],
            To = parameters["To"],
            ToZip = parameters["ToZip"],
            MessageSid = parameters["MessageSid"],
            AccountSid = parameters["AccountSid"],
            From = parameters["From"],
            ApiVersion = parameters["ApiVersion"]
        };
        response.NumMedia = int.TryParse(parameters["NumMedia"], out int i) ? int.Parse(parameters["NumMedia"]) : 0;
        response.NumSegments = int.TryParse(parameters["NumSegments"], out int j) ? int.Parse(parameters["NumSegments"]) : 0;
        response.ReferralNumMedia = int.TryParse(parameters["ReferralNumMedia"], out int k) ? int.Parse(parameters["ReferralNumMedia"]) : 0;

        for (int l = 0; l < response.NumMedia; l++)
        {
            if (l != 0) response.Media += "|";
            response.Media += parameters[$"MediaUrl{l}"].ToString();
        }

        await _smsMessagesClient.AddEntityAsync<SmsMessageEntity>(response);

        return response;
    }
    public FamilyMemberEntity GetFamilyMember(string mobileNumber)
    {
        return _familyMembersClient
            .Query<FamilyMemberEntity>($"MobileNumber eq '{mobileNumber}'")
            .FirstOrDefault();
    }
}