using Azone.Gateway;
using Azone.Models.Generated;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

builder.AddServiceDefaults();

var app = builder.Build();

app.MapDefaultEndpoints();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapGet("/say-hello", () => "Hello!");
var api = app.MapGroup("api/");

var accounts = api.MapGroup("accounts/");

accounts.MapGrpcPost<Auth.AuthClient, CreateAccountRequest, CreateAccountReply>("/create",
    async (client, req, ctx) => await client.CreateAccountAsync(req));

accounts.MapGrpcPost<Auth.AuthClient, LoginRequest, LoginReply>("/login",
    async (client, request, ctx) => await client.LoginAsync(request));

accounts.MapGrpcPost<Auth.AuthClient, LogoutRequest, LogoutReply>("/logout",
    async (client, request, ctx) => await client.LogoutAsync(request));

app.Run();
