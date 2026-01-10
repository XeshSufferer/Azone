using Grpc.Core;

namespace Azone.Gateway;

public static class Extensions
{
    public static WebApplication MapGrpcPost<TClient, TRequest, TResponse>(
        this WebApplication app,
        string path,
        Func<TClient, TRequest, HttpContext, Task<TResponse>> handler)
        where TClient : class
    {
        app.MapPost(path, async (TRequest request, HttpContext ctx) =>
        {
            var client = ctx.RequestServices.GetRequiredService<TClient>();
            try
            {
                var response = await handler(client, request, ctx);
                return Results.Ok(response);
            }
            catch (RpcException ex)
            {
                return HandleGrpcError(ex);
            }
        });
        return app;
    }
    
    public static RouteGroupBuilder MapGrpcPost<TClient, TRequest, TResponse>(
        this RouteGroupBuilder app,
        string path,
        Func<TClient, TRequest, HttpContext, Task<TResponse>> handler)
        where TClient : class 
    {
        app.MapPost(path, async (TRequest request, HttpContext ctx) =>
        {
            var client = ctx.RequestServices.GetRequiredService<TClient>();
            try
            {
                var response = await handler(client, request, ctx);
                return Results.Ok(response);
            }
            catch (RpcException ex)
            {
                return HandleGrpcError(ex);
            }
        });
        return app;
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