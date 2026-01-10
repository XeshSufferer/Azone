namespace Azone.Auth.Helpers;

public interface IHasher
{
    string Hash(string content);
    bool Verify(string content, string hash);
}