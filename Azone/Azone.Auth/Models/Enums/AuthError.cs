namespace Azone.Auth.Services;

public enum AuthError 
{
    InvalidPasswordOrLogin,
    UserWithLoginAlreadyExist,
    UserNotFound,
    RefreshTokenInvalidOrExpired
}