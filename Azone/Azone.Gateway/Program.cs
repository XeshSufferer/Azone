using Azone.Contracts.Models.Generated;
using Azone.Gateway;
using Google.Protobuf.WellKnownTypes;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

builder.AddServiceDefaults();

builder.Services.AddServiceDiscovery();

builder.Services.AddValidation();

builder.Services.AddGrpcClientByLink<Auth.AuthClient>("auth");
builder.Services.AddGrpcClientByLink<Merchant.MerchantClient>("merchant");

var app = builder.Build();

app.MapDefaultEndpoints();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

var api = app.MapGroup("api/");
var auth = api.MapGroup("auth/");
var merchant = api.MapGroup("merchant/");


auth.MapGet("/validate-token", () => new { IsValid = true })
    .RequireAuthorization();

auth.MapGrpcPost<Auth.AuthClient, CreateAccountRequest, CreateAccountReply>("/create",
    async (client, req, md, ctx) => await client.CreateAccountAsync(req), false);

auth.MapGrpcPost<Auth.AuthClient, LoginRequest, LoginReply>("/login",
    async (client, request, md, ctx) => await client.LoginAsync(request), false);

auth.MapGrpcPost<Auth.AuthClient, RefreshToken, IsSuccess>("/logout",
    async (client, request, md, ctx) => await client.LogoutAsync(request), false);

auth.MapGrpcPost<Auth.AuthClient, RefreshToken, TokenPair>("/refresh",
    async (client, request, md, ctx) => await client.RefreshAsync(request), false);

auth.MapGrpcPost<Auth.AuthClient, ChangePasswordRequest, IsSuccess>("/change-password",
    async (client, request, md, ctx) => await client.ChangePasswordAsync(request), false);


merchant.MapGrpcPost<Merchant.MerchantClient, CreateShopRequest, ShopData>("/create-shop", 
    async (client, request, md, ctx) => await client.CreateShopAsync(request, md));

merchant.MapGrpcGet<Merchant.MerchantClient, Empty, PermissionsList>("/get-permissions", 
    async (client, empty, md, ctx) => await client.GetPermissionListAsync(empty, md), false);

merchant.MapGrpcPost<Merchant.MerchantClient, EditShopFieldRequest, IsSuccess>("/rename-shop", 
    async (client, request, md, ctx) => await client.EditShopNameAsync(request, md));

merchant.MapGrpcPost<Merchant.MerchantClient, EditShopFieldRequest, IsSuccess>("/edit-description", 
    async (client, request, md, ctx) => await client.EditShopDescriptionAsync(request, md));

merchant.MapGrpcPost<Merchant.MerchantClient, EditShopFieldRequest, IsSuccess>("/edit-logo",
    async (client, request, md, ctx) => await client.EditShopLogoAsync(request, md));

merchant.MapGrpcPost<Merchant.MerchantClient, EditOwnerPermissionList, IsSuccess>("/edit-shop", 
    async (client, request, md, ctx) => await client.EditOwnerPermissionsAsync(request, md));

merchant.MapGrpcGet<Merchant.MerchantClient, int, ShopData>("/shop-data/{shop-id:int}",
    async (client, i, md, ctx) => await client.GetShopByIdAsync(new ShopId {Id = i}, md), false);

merchant.MapGrpcGet<Merchant.MerchantClient, int, ShopPreview>("/shop-preview/{shop-id:int}",
    async (client, i, md, ctx) => await client.GetPreviewShopByIdAsync(new ShopId {Id = i}, md));


app.Run();