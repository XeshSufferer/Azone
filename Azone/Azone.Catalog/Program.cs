using Azone.Catalog.DB;
using Azone.Catalog.Services;
using Azone.Contracts.Models.Generated;
using Azone.Infra.Shared;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddGrpc();

builder.Services.AddGrpcClientByLink<Merchant.MerchantClient>("merchant");
builder.AddNpgsqlDbContext<CatalogDbContext>("catalog-db");

var app = builder.Build();

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
app.MapGrpcService<CatalogService>();

app.Run();