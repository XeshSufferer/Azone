using FluentValidation;
using Grpc.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Formatters;

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
        var endpoint = app.MapPost(path, async (TRequest request, HttpContext ctx) =>
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
        
        if(forwardAuth)
            endpoint.RequireAuthorization();
        
        return app;
    }

    public static RouteGroupBuilder MapGrpcPost<TClient, TRequest, TResponse>(
        this RouteGroupBuilder app,
        string path,
        Func<TClient, TRequest, Metadata, HttpContext, Task<TResponse>> handler,
        bool forwardAuth = true)
        where TClient : class
    {
        var endpoint = app.MapPost(path, async (TRequest request, HttpContext ctx) =>
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
        
        if(forwardAuth)
            endpoint.RequireAuthorization();
        return app;
    }
    
    
    public static WebApplication MapGrpcGet<TClient, TRequest, TResponse>(
        this WebApplication app,
        string path,
        Func<TClient, TRequest, Metadata, HttpContext, Task<TResponse>> handler,
        bool forwardAuth = true)
        where TClient : class
    {
        
        
        var endpoint = app.MapGet(path, async (TRequest request, HttpContext ctx) =>
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
        
        if(forwardAuth)
            endpoint.RequireAuthorization();
        return app;
    }

    public static RouteGroupBuilder MapGrpcGet<TClient, TRequest, TResponse>(
        this RouteGroupBuilder app,
        string path,
        Func<TClient, TRequest, Metadata, HttpContext, Task<TResponse>> handler,
        bool forwardAuth = true)
        where TClient : class
    {
        var endpoint = app.MapGet(path, async (TRequest request, HttpContext ctx) =>
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

        if (forwardAuth)
            endpoint.RequireAuthorization();
        return app;
    }
    
    public static RouteGroupBuilder MapGrpcGet<TClient, TParam, TRequest, TResponse>(
        this RouteGroupBuilder app,
        string path,
        Func<TParam, TRequest> requestFactory,
        Func<TClient, TRequest, Metadata, HttpContext, Task<TResponse>> handler,
        bool forwardAuth = true)
        where TClient : class
    {
        var endpoint = app.MapGet(path, async (TParam param, HttpContext ctx) =>
        {
            var request = requestFactory(param);
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

        if (forwardAuth)
            endpoint.RequireAuthorization();

        return app;
    }

    public static RouteGroupBuilder MapGrpcGet<TClient, TParam1, TParam2, TRequest, TResponse>(
        this RouteGroupBuilder app,
        string path,
        Func<TParam1, TParam2, TRequest> requestFactory,
        Func<TClient, TRequest, Metadata, HttpContext, Task<TResponse>> handler,
        bool forwardAuth = true)
        where TClient : class
    {
        var endpoint = app.MapGet(path, async (TParam1 p1, TParam2 p2, HttpContext ctx) =>
        {
            var request = requestFactory(p1, p2);
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

        if (forwardAuth)
            endpoint.RequireAuthorization();

        return app;
    }
    
    public static WebApplication MapGrpcGet<TClient, TParam, TRequest, TResponse>(
        this WebApplication app,
        string path,
        Func<TParam, TRequest> requestFactory,
        Func<TClient, TRequest, Metadata, HttpContext, Task<TResponse>> handler,
        bool forwardAuth = true)
        where TClient : class
    {
        var endpoint = app.MapGet(path, async (TParam param, HttpContext ctx) =>
        {
            var request = requestFactory(param);
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

        if (forwardAuth)
            endpoint.RequireAuthorization();

        return app;
    }

    public static WebApplication MapGrpcGet<TClient, TParam1, TParam2, TRequest, TResponse>(
        this WebApplication app,
        string path,
        Func<TParam1, TParam2, TRequest> requestFactory,
        Func<TClient, TRequest, Metadata, HttpContext, Task<TResponse>> handler,
        bool forwardAuth = true)
        where TClient : class
    {
        var endpoint = app.MapGet(path, async (TParam1 p1, TParam2 p2, HttpContext ctx) =>
        {
            var request = requestFactory(p1, p2);
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

        if (forwardAuth)
            endpoint.RequireAuthorization();

        return app;
    }

    private static Metadata BuildMetadata(HttpContext context, bool forwardAuth)
    {
        var metadata = new Metadata();

        if (forwardAuth && context.Request.Headers.TryGetValue("Authorization", out var authValues))
        {
            foreach (var value in authValues)
            {
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

        return Results.Problem(statusCode: httpStatus, detail: ex.Message);
    }
    
    public static IServiceCollection AddGrpcClientByLink<TClient>(
        this IServiceCollection app,
        string path)
    where TClient : class
    {
        app.AddGrpcClient<TClient>(o =>
        {
            o.Address = new Uri(Environment.GetEnvironmentVariable($"{path}:connection") ?? throw new ArgumentNullException($"{path}:connect", $"Environment variable {path} not found"));
        });
        return app;
    }

    public static IRuleBuilder<T, string> IsPassword<T>(this IRuleBuilder<T, string> builder)
    {
        return builder
            .NotEmpty().WithMessage("New password is required")
            .Length(6, 64).WithMessage("Password must be between 6 and 64 characters");
    }

    public static IRuleBuilder<T, string> IsLogin<T>(this IRuleBuilder<T, string> builder)
    {
        return builder
            .NotEmpty().WithMessage("Login name is required")
            .Length(6, 64).WithMessage("Login name must be between 6 and 64 characters");
    }
    
    public static IRuleBuilderOptions<T, string> IsGuid<T>(this IRuleBuilder<T, string> builder)
    {
        return builder
            .Must(g => !Guid.TryParse(g, out _));
    } 
}