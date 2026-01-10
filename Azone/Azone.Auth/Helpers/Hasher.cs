namespace Azone.Auth.Helpers;

public class Hasher : IHasher
{
    public string HashBcrypt(string content)
    {
        return BCrypt.Net.BCrypt.HashPassword(content);
    }

    public bool Verify(string content, string hash)
    {
        return BCrypt.Net.BCrypt.Verify(content, hash);
    }
}