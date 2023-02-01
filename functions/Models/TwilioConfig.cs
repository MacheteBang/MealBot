namespace MealBot.Models;

public class TwilioConfig {
    public string AccountSid { get; }
    public string AuthToken { get; }

    private TwilioConfig() { }

    public TwilioConfig(string accountSid, string authToken)
    {
        AccountSid = accountSid;
        AuthToken = authToken;
    }
}