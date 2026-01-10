using Azone.Auth.Db;
using Azone.Auth.Services;
using Azone.Auth.Services.Sub_Services;
using Azone.Auth.Services.Sub_Services.Contracts;
using Azone.Auth.Helpers;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddGrpc();

builder.AddServiceDefaults();

builder.Services.AddSingleton<IHasher, Hasher>();
builder.Services.AddScoped<IRefreshService, RefreshService>();
builder.Services.AddScoped<IJwtService, JwtService>();

builder.AddNpgsqlDbContext<AuthDbContext>("auth-db");

var app = builder.Build();

app.MapDefaultEndpoints();

app.MapGrpcService<AuthService>();

app.Run();