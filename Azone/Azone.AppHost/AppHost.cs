using Azone.AppHost;
using dotenv.net;

var builder = DistributedApplication.CreateBuilder(args);

DotEnv.Load();

var cache = builder.AddRedis("redis")
    .WithDeveloperCertificateTrust(false)
    .WithOtlpExporter();
var postgres = builder.AddPostgres("postgres");

var authDb = postgres.AddDatabase("auth-db");
var merchantDb = postgres.AddDatabase("merchant-db");
var catalogDb = postgres.AddDatabase("catalog-db");

var auth = builder.AddProject<Projects.Azone_Auth>("auth")
    .WithOtlpExporter()
    .WithDefaultReferences(authDb, cache)
    .WithDefaultSecuritySettings();

var merchant = builder.AddProject<Projects.Azone_Merchant>("merchant")
    .WithOtlpExporter()
    .WithDefaultReferences(merchantDb, cache)
    .WithDefaultSecuritySettings();

var catalog = builder.AddProject<Projects.Azone_Catalog>("catalog")
    .WithOtlpExporter()
    .WithDefaultReferences(catalogDb, cache)
    .WithDefaultSecuritySettings()
    .AddServiceConnectionString(merchant);

var gateway = builder.AddProject<Projects.Azone_Gateway>("gateway")
    .WithOtlpExporter()
    .WithDefaultReferences(authDb, cache)
    .WithDefaultSecuritySettings()
    .AddServiceConnectionString(auth)
    .AddServiceConnectionString(merchant);


builder.Build().Run();