namespace Azone.AppHost;

public static class Extensions
{
    public static IResourceBuilder<ProjectResource> WithDefaultReferences(
        this IResourceBuilder<ProjectResource> builder,
        IResourceBuilder<PostgresDatabaseResource> db,
        IResourceBuilder<RedisResource> redis)
    {
        return builder
            .WithReference(db)
            .WithReference(redis)
            .WaitFor(db)
            .WaitFor(redis);
    }
    
    public static IResourceBuilder<ProjectResource> AddServiceConnectionString(
        this IResourceBuilder<ProjectResource> builder,
        string key, IResourceBuilder<ProjectResource> service)
    {
        return builder
            .WithReference(service)
            .WithEnvironment(key, $"http://{service.Resource.Name}");
    }
    
    public static IResourceBuilder<ProjectResource> WithDefaultSecuritySettings(
        this IResourceBuilder<ProjectResource> builder)
    {
        builder
            .WithEnvironment("JWT_AUDIENCE", Environment.GetEnvironmentVariable("JWT_AUDIENCE") ?? "audience")
            .WithEnvironment("JWT_ISSUER", Environment.GetEnvironmentVariable("JWT_ISSUER") ?? "issuer")
            .WithEnvironment("JWT_SECRET", Environment.GetEnvironmentVariable("JWT_SECRET"))
            .WithEnvironment("JWT_EXPIRE_MINUTES", Environment.GetEnvironmentVariable("JWT_EXPIRE_MINUTES") ?? "60")
            .WithEnvironment("REFRESH_EXPIRE_DAYS", Environment.GetEnvironmentVariable("REFRESH_EXPIRE_DAYS") ?? "14");
        return builder;
    }
}