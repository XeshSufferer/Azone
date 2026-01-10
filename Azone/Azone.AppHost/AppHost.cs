using Azone.AppHost;
using dotenv.net;

var builder = DistributedApplication.CreateBuilder(args);

DotEnv.Load();

var cache = builder.AddRedis("redis")
    .WithDeveloperCertificateTrust(false)
    .WithOtlpExporter();
var db = builder.AddPostgres("postgres")
    .AddDatabase("main");

var auth = builder.AddProject<Projects.Azone_Auth>("auth");

var gateway = builder.AddProject<Projects.Azone_Gateway>("gateway")
    .WithOtlpExporter()
    .WithDefaultReferences(db, cache)
    .WithDefaultSecuritySettings();



builder.Build().Run();