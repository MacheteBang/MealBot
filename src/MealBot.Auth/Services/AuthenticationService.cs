namespace MealBot.Auth.Services;

internal interface IAuthenticationService
{
    Task<ErrorOr<User>> AddOrGetUserAsync(ExternalIdentity externalIdentity);
}

internal sealed class AuthenticationService(IUserService _userRepository) : IAuthenticationService
{
    private readonly IUserService userRepository = _userRepository;

    public async Task<ErrorOr<User>> AddOrGetUserAsync(ExternalIdentity externalIdentity)
    {
        var userResult = await userRepository.GetByEmailAddressAsync(externalIdentity.EmailAddress);

        if (userResult.IsError && userResult.FirstError.Type != ErrorType.NotFound)
        {
            return userResult;
        }

        if (userResult.IsError && userResult.FirstError.Type == ErrorType.NotFound)
        {
            var newUser = new User
            {
                EmailAddress = externalIdentity.EmailAddress,
                AuthProvider = externalIdentity.AuthProvider,
                ExternalId = externalIdentity.Id,
                FirstName = externalIdentity.FirstName,
                LastName = externalIdentity.LastName,
                PictureUri = externalIdentity.ProfilePictureUri
            };

            var addUserResult = await userRepository.AddAsync(newUser);

            if (addUserResult.IsError)
            {
                return addUserResult;
            }
            return newUser;
        }

        if (IsUserDifferent(userResult.Value, externalIdentity))
        {
            var foundUser = userResult.Value;
            foundUser.AuthProvider = externalIdentity.AuthProvider;
            foundUser.ExternalId = externalIdentity.Id;
            foundUser.FirstName = externalIdentity.FirstName;
            foundUser.LastName = externalIdentity.LastName;
            foundUser.PictureUri = externalIdentity.ProfilePictureUri;

            var updateResult = await userRepository.UpdateAsync(foundUser);

            return updateResult.Match<ErrorOr<User>>(
                user => user,
                errors => errors
            );
        }

        return userResult.Value;
    }

    private static bool IsUserDifferent(User user, ExternalIdentity externalIdentity) =>
        user.AuthProvider != externalIdentity.AuthProvider ||
        user.ExternalId != externalIdentity.Id ||
        user.FirstName != externalIdentity.FirstName ||
        user.LastName != externalIdentity.LastName ||
        user.PictureUri != externalIdentity.ProfilePictureUri;
}