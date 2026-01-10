using Azone.Accounts.Services;
using Azone.Accounts.Services.Sub_Services;
using Azone.Accounts.Services.Sub_Services.Contracts;
using Azone.Auth.Helpers;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddGrpc();

builder.AddServiceDefaults();

builder.Services.AddSingleton<IHasher, Hasher>();
builder.Services.AddScoped<IRefreshService, RefreshService>();
builder.Services.AddScoped<IJwtService, JwtService>();

var app = builder.Build();

app.MapDefaultEndpoints();

app.MapGrpcService<AuthService>();

app.Run();