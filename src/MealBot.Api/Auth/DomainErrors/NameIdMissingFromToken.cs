namespace MealBot.Api.Auth.DomainErrors;

internal static partial class Errors
{
    public static Error NameIdMissingFromToken() => Error.Unauthorized(
        code: "Auth.NameIdMissingFromToken",
        description: $"Then `nameid` claim was missing from the user token.");
}