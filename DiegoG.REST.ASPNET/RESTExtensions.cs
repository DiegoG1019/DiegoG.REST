using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace DiegoG.REST.ASPNET;

public static class RESTExtensions
{
    public static IApplicationBuilder UseRESTExceptionHandler<TResponseCode>(
        this IApplicationBuilder app,
        ExceptionResponseFactory<TResponseCode> responseFactory
    ) where TResponseCode : struct, IEquatable<TResponseCode>
    {
        ArgumentNullException.ThrowIfNull(responseFactory);
        app.UseMiddleware<RESTExceptionHandler<TResponseCode>>(responseFactory);
        return app;
    }

    public static IServiceCollection UseRESTInvalidModelStateResponse<TResponseCode>(
        this IServiceCollection services,
        Func<ActionContext, RESTObjectResult<TResponseCode>> responseFactory
    ) where TResponseCode : struct, IEquatable<TResponseCode>
    {
        ArgumentNullException.ThrowIfNull(responseFactory);
        services.Configure<ApiBehaviorOptions>(o => o.InvalidModelStateResponseFactory = responseFactory);
        return services;
    }

    public static IServiceCollection UseRESTInvalidModelStateResponse<TResponseCode>(
        this IServiceCollection services,
        Func<ActionContext, List<string>, RESTObjectResult<TResponseCode>> responseFactory
    ) where TResponseCode : struct, IEquatable<TResponseCode>
    {
        ArgumentNullException.ThrowIfNull(responseFactory);
        services.Configure<ApiBehaviorOptions>(o => o.InvalidModelStateResponseFactory = c =>
        {
            List<string> errors = new();
            foreach (var (k, v) in c.ModelState)
                foreach (var e in v.Errors)
                    errors.Add($"{k}: {e.ErrorMessage}");
            return responseFactory(c, errors);
        });
        return services;
    }

    public static IServiceCollection AddRESTObjectTypeTable<TRESTObjectCode>(
        this IServiceCollection services,
        RESTObjectTypeTable<TRESTObjectCode> table
    )
        where TRESTObjectCode : struct, IEquatable<TRESTObjectCode>
    {
        services.AddSingleton(table);
        return services;
    }

    public static IServiceCollection AddRESTObjectSerializer<TRESTObjectCode>(
        this IServiceCollection services, 
        Func<IServiceProvider, IRESTObjectSerializer<TRESTObjectCode>> serializer
    )
        where TRESTObjectCode : struct, IEquatable<TRESTObjectCode>
    {
        services.AddScoped(serializer); return services;
    }

    public static IServiceCollection AddRESTObjectSerializer<TRESTObjectCode>(
        this IServiceCollection services,
        IRESTObjectSerializer<TRESTObjectCode> serializer
    )
        where TRESTObjectCode : struct, IEquatable<TRESTObjectCode>
    {
        services.AddSingleton(serializer); return services;
    }
}