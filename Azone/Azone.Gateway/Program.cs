using Azone.Contracts.Models.Generated;
using Azone.Gateway;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

builder.AddServiceDefaults();

builder.Services.AddServiceDiscovery();

builder.Services.AddValidation();

builder.Services.AddGrpcClient<Auth.AuthClient>(op =>
{
    op.Address = new Uri(builder.Configuration["Auth:connection"]);
});

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



app.Run();