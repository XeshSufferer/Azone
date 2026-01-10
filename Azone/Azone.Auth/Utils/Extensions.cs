using Azone.Auth.Services;

namespace Azone.Auth.Utils;

public static class Extensions
{
    public static string Code(this AuthError error)
        => $"Auth.{error.ToString()}";
}