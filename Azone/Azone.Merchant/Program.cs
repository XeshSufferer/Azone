using Azone.Merchant.DBs;
using Azone.Merchant.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddGrpc();

builder.AddServiceDefaults();

builder.AddNpgsqlDbContext<MerchantDbContext>("merchant-db");

var app = builder.Build();

await using var scope = app.Services.CreateAsyncScope();

var db = scope.ServiceProvider.GetRequiredService<MerchantDbContext>();
await db.Database.MigrateAsync();

app.MapDefaultEndpoints();

app.MapGrpcService<MerchantService>();

app.Run();