var builder = DistributedApplication.CreateBuilder(args);


var gateway = builder.AddProject<Projects.Azone_Gateway>("gateway")
    .WithOtlpExporter();

builder.Build().Run();