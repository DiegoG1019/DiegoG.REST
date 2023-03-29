using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace DiegoG.REST.ASPNET;

/// <summary>
/// Extension methods to configure an application and services for use with REST models created using this library
/// </summary>
public static class RESTExtensions
{
    /// <summary>
    /// Adds a middleware that handles action exceptions. Be sure to call this method first or early, as it may be overriden by other middleware that was added first.
    /// </summary>
    /// <remarks>
    /// The added middleware does not handle model validation exceptions
    /// </remarks>
    /// <typeparam name="TResponseCode"></typeparam>
    /// <param name="app"></param>
    /// <param name="responseFactory"></param>
    /// <returns></returns>
    public static IApplicationBuilder UseRESTExceptionHandler<TResponseCode>(
        this IApplicationBuilder app,
        ExceptionResponseFactory<TResponseCode> responseFactory
    ) where TResponseCode : struct, IEquatable<TResponseCode>
    {
        ArgumentNullException.ThrowIfNull(responseFactory);
        app.UseMiddleware<RESTExceptionHandler<TResponseCode>>(responseFactory);
        return app;
    }

    /// <summary>
    /// Registers the service for <see cref="ValidateRESTObjectActionFilter{TRESTObjectCode}"/> to permit the usage of <see cref="ValidateRESTObjectAttribute"/> in controllers and actions
    /// </summary>
    public static IServiceCollection UseRESTObjectValidationFilter<TResponseCode>(
        this IServiceCollection services
    ) where TResponseCode : struct, IEquatable<TResponseCode>
    {
        services.AddScoped<ValidateRESTObjectActionFilter<TResponseCode>>();
        return services;
    }

    /// <summary>
    /// Configures <paramref name="services"/> to use the specific, REST model compliant <paramref name="responseFactory"/> to respond to requests that cause model validation errors not otherwise handled by Middleware
    /// </summary>
    public static IServiceCollection UseRESTInvalidModelStateResponse<TResponseCode>(
        this IServiceCollection services,
        Func<ActionContext, RESTObjectResult<TResponseCode>> responseFactory
    ) where TResponseCode : struct, IEquatable<TResponseCode>
    {
        ArgumentNullException.ThrowIfNull(responseFactory);
        services.Configure<ApiBehaviorOptions>(o => o.InvalidModelStateResponseFactory = responseFactory);
        return services;
    }

    /// <summary>
    /// Configures <paramref name="services"/> to use the specific, REST model compliant <paramref name="responseFactory"/> to respond to requests that cause model validation errors not otherwise handled by Middleware
    /// </summary>
    /// <remarks>
    /// This method accepts a delegate that also receives a <see cref="List{T}"/> of <see cref="string"/>s that serve as a pre-rendered list of model errors
    /// </remarks>
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

    /// <summary>
    /// Adds <paramref name="table"/> as a singleton service into <paramref name="services"/>
    /// </summary>
    public static IServiceCollection AddRESTObjectTypeTable<TRESTObjectCode>(
        this IServiceCollection services,
        RESTObjectTypeTable<TRESTObjectCode> table
    )
        where TRESTObjectCode : struct, IEquatable<TRESTObjectCode>
    {
        services.AddSingleton(table);
        return services;
    }

    /// <summary>
    /// Adds <paramref name="serializerFactory"/> as a scoped service into <paramref name="services"/>, to generate serializers with other necessary services
    /// </summary>
    public static IServiceCollection AddRESTObjectSerializer<TRESTObjectCode>(
        this IServiceCollection services, 
        Func<IServiceProvider, IRESTObjectSerializer<TRESTObjectCode>> serializerFactory
    )
        where TRESTObjectCode : struct, IEquatable<TRESTObjectCode>
    {
        services.AddScoped(serializerFactory); return services;
    }

    /// <summary>
    /// Adds <paramref name="serializer"/> as a scoped service into <paramref name="services"/>
    /// </summary>
    public static IServiceCollection AddRESTObjectSerializer<TRESTObjectCode>(
        this IServiceCollection services,
        IRESTObjectSerializer<TRESTObjectCode> serializer
    )
        where TRESTObjectCode : struct, IEquatable<TRESTObjectCode>
    {
        services.AddSingleton(serializer); return services;
    }
}