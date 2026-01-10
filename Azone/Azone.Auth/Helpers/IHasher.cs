namespace Azone.Auth.Helpers;

public interface IHasher
{
    string HashBcrypt(string content);
    bool Verify(string content, string hash);
}