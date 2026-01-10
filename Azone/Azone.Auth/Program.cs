using Azone.Auth.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddGrpc();

builder.AddServiceDefaults();

var app = builder.Build();

app.MapDefaultEndpoints();

app.MapGrpcService<AuthService>();

app.Run();