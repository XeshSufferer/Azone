using Grpc.Core;
using Microsoft.AspNetCore.Http;

namespace Azone.Gateway;

public static class Extensions
{
    // Для WebApplication
    public static WebApplication MapGrpcPost<TClient, TRequest, TResponse>(
        this WebApplication app,
        string path,
        Func<TClient, TRequest, Metadata, HttpContext, Task<TResponse>> handler,
        bool forwardAuth = true)
        where TClient : class
    {
        app.MapPost(path, async (TRequest request, HttpContext ctx) =>
        {
            var client = ctx.RequestServices.GetRequiredService<TClient>();
            var metadata = BuildMetadata(ctx, forwardAuth);

            try
            {
                var response = await handler(client, request, metadata, ctx);
                return Results.Ok(response);
            }
            catch (RpcException ex)
            {
                return HandleGrpcError(ex);
            }
        });
        return app;
    }

    // Для RouteGroupBuilder
    public static RouteGroupBuilder MapGrpcPost<TClient, TRequest, TResponse>(
        this RouteGroupBuilder app,
        string path,
        Func<TClient, TRequest, Metadata, HttpContext, Task<TResponse>> handler,
        bool forwardAuth = true)
        where TClient : class
    {
        app.MapPost(path, async (TRequest request, HttpContext ctx) =>
        {
            var client = ctx.RequestServices.GetRequiredService<TClient>();
            var metadata = BuildMetadata(ctx, forwardAuth);

            try
            {
                var response = await handler(client, request, metadata, ctx);
                return Results.Ok(response);
            }
            catch (RpcException ex)
            {
                return HandleGrpcError(ex);
            }
        });
        return app;
    }

    private static Metadata BuildMetadata(HttpContext context, bool forwardAuth)
    {
        var metadata = new Metadata();

        if (forwardAuth && context.Request.Headers.TryGetValue("Authorization", out var authValues))
        {
            foreach (var value in authValues)
            {
                // gRPC требует lowercase ключей
                metadata.Add("authorization", value);
            }
        }

        return metadata;
    }

    private static IResult HandleGrpcError(RpcException ex)
    {
        var httpStatus = ex.StatusCode switch
        {
            StatusCode.InvalidArgument => 400,
            StatusCode.NotFound => 404,
            StatusCode.AlreadyExists => 409,
            StatusCode.PermissionDenied => 403,
            StatusCode.Unauthenticated => 401,
            StatusCode.DeadlineExceeded or StatusCode.Cancelled => 408,
            _ => 500
        };

        return Results.StatusCode(httpStatus);
    }
}