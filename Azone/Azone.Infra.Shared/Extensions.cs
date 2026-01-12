using Azone.Contracts.Models.Generated;
using Microsoft.Extensions.DependencyInjection;

namespace Azone.Infra.Shared;

public static class Extensions
{
    public static IServiceCollection AddGrpcClientByLink<TClient>(
        this IServiceCollection app,
        string path)
        where TClient : class
    {
        app.AddGrpcClient<TClient>(o =>
        {
            o.Address = new Uri(Environment.GetEnvironmentVariable($"{path}:connection") ?? throw new ArgumentNullException($"{path}:connect", $"Environment variable {path} not found"));
        });
        return app;
    }
    
    public static IsSuccess ToIsSuccess(this bool isSuccess)
        => new IsSuccess { Success = isSuccess };
}