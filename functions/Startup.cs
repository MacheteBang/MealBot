using System.Text.Json;
using System.Text.Json.Serialization;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using MealBot.Services;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(MealBot.Startup))]

namespace MealBot;

public class Startup : FunctionsStartup
{
    public override void Configure(IFunctionsHostBuilder builder)
    {

        string keyVaultUri = Environment.GetEnvironmentVariable("KEY_VAULT_URI", EnvironmentVariableTarget.Process);

        var client = new SecretClient(vaultUri: new Uri(keyVaultUri), credential: new DefaultAzureCredential());
        string twilioAccountSid = client.GetSecret("Twilio-AccountSid").Value.Value;
        string twilioAuthToken = client.GetSecret("Twilio-AuthToken").Value.Value;
        TwilioConfig twilioConfig = new(twilioAccountSid, twilioAuthToken);
        builder.Services.AddSingleton<TwilioConfig>(twilioConfig);


        JsonSerializerOptions options = new JsonSerializerOptions
        {
            Converters = { new JsonStringEnumConverter() },
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };
        builder.Services.AddSingleton<JsonSerializerOptions>(options);

        builder.Services.AddSingleton<StorageService>(new StorageService(Environment.GetEnvironmentVariable("AzureWebJobsStorage"), options));
    }
}
