namespace MealBot.Api.Auth.Features.GetToken.Google;

public sealed record GetAccessTokenGoogleQuery(string AuthorizationCode, string CallBackUri) : IRequest<ErrorOr<TokenBundle>>;