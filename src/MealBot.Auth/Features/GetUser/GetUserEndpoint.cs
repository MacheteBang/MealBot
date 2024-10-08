namespace MealBot.Auth.Features.GetUser;

internal sealed class GetUserEndpoint : AuthEndpoint
{
    public override void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet(Globals.GetUserRoute, async (HttpContext httpContext, ISender sender) =>
        {
            string? emailAddress = httpContext.User.FindFirst(ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(emailAddress))
            {
                // TODO: Should this be a different response?
                return Results.Unauthorized();
            }

            var query = new GetUserQuery(emailAddress);
            var result = await sender.Send(query);

            return result.Match(
                user =>
                {
                    UserResponse userResponse = new(
                        user.EmailAddress,
                        user.AuthProvider.ToString(),
                        user.FirstName,
                        user.LastName,
                        user.PictureUri);

                    return Results.Ok(userResponse);
                },
                error => Problem(error));
        })
        .RequireAuthorization();
    }
}