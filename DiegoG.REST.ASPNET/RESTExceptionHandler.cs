using System.Net;
using System.Text;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.DependencyInjection;
using static System.Net.Mime.MediaTypeNames;

namespace DiegoG.REST.ASPNET;

/// <summary>
/// Represents a REST Response that, along with <paramref name="ResponseStatus"/> represents a full HTTP response for the exception compliant with the models of the rest of the API
/// </summary>
public readonly record struct ExceptionRESTResponse<TObjectCode>(RESTObject<TObjectCode> RESTObject, HttpStatusCode ResponseStatus = HttpStatusCode.InternalServerError)
     where TObjectCode : struct, IEquatable<TObjectCode>;

/// <summary>
/// Creates a new object of type <see cref="ExceptionRESTResponse{TObjectCode}"/> from the provided data
/// </summary>
/// <param name="request">The request that caused the exception</param>
/// <param name="exception">The exception that ensued</param>
/// <param name="services">A service provider for use within this factory</param>
/// <param name="context">The entire HttpContext</param>
/// <returns></returns>
public delegate Task<ExceptionRESTResponse<TObjectCode>> ExceptionResponseFactory<TObjectCode>(
    HttpRequest request,
    Exception? exception,
    IServiceProvider services,
    HttpContext context
) where TObjectCode : struct, IEquatable<TObjectCode>;

/// <summary>
/// Middleware that handles action exceptions
/// </summary>
/// <remarks>
/// Keep in mind that this middleware does not handle model validation exceptions
/// </remarks>
public class RESTExceptionHandler<TObjectCode> where TObjectCode : struct, IEquatable<TObjectCode>
{
    private readonly RequestDelegate _next;
    private readonly IServiceProvider Services;
    private readonly ExceptionResponseFactory<TObjectCode> ExceptionResponseFactory;

    /// <summary>
    /// Creates a new instance of <see cref="RESTExceptionHandler{TObjectCode}"/>
    /// </summary>
    /// <param name="next">The next middleware in the pipeline</param>
    /// <param name="services">A service provider for this middleware</param>
    /// <param name="exceptionResponseFactory">This middleware's exception factory</param>
    /// <exception cref="ArgumentNullException"></exception>
    public RESTExceptionHandler(
        RequestDelegate next,
        IServiceProvider services,
        ExceptionResponseFactory<TObjectCode> exceptionResponseFactory
    )
    {
        _next = next;
        Services = services ?? throw new ArgumentNullException(nameof(services));
        ExceptionResponseFactory = exceptionResponseFactory ?? throw new ArgumentNullException(nameof(exceptionResponseFactory));
    }

    /// <summary>
    /// Invokes this Middleware's behavior under <paramref name="context"/>
    /// </summary>
    public async Task Invoke(HttpContext context)
    {
        Exception e;
        try
        {
            await _next.Invoke(context);
            var eHandler = context.Features.Get<IExceptionHandlerFeature>();
            if (context.Response.HasStarted || eHandler?.Error is not Exception excp) 
                return;

            e = excp;
        }
        catch (Exception ex)
        {
            e = ex;
        }

        var __scope = Services.CreateScope();
        var services = __scope.ServiceProvider;
        using (__scope)
        {
            var resp = await ExceptionResponseFactory(
                    context.Request,
                    e,
                    services,
                    context
                );

            if (context.Response.HasStarted is false)
            {
                var serializer = services.GetRequiredService<IRESTObjectSerializer<TObjectCode>>();
                var sb = new StringBuilder(200);
                sb.AppendJoin("; ", serializer.MIMETypes);
                if (serializer.Charset is string charset)
                    sb.Append("; charset=").Append(charset);

                context.Response.StatusCode = (int)resp.ResponseStatus;
                context.Response.ContentType = sb.ToString();
                await serializer.SerializeAsync(
                    resp.RESTObject,
                    context.Response.Body
                );
            }
        }
    }
}