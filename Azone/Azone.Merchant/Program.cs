using Azone.Merchant.DBs;
using Azone.Merchant.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddGrpc();

builder.AddServiceDefaults();

builder.AddNpgsqlDbContext<MerchantDbContext>("merchant-db");

var app = builder.Build();

app.MapDefaultEndpoints();

app.MapGrpcService<MerchantService>();

app.Run();