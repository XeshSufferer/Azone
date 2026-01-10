using Azone.Auth.Models;
using Azone.Contracts.Models.Generated;

namespace Azone.Auth.Mappers;

public static class UserMapper
{
    public static UserData ToUserData(this User user)
    {
        return new UserData
        {
            Id = user.Id
        };
    }
}