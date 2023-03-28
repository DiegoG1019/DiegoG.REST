using System.Net;
using System.Text;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.DependencyInjection;
using static System.Net.Mime.MediaTypeNames;

namespace DiegoG.REST.ASPNET;

public readonly record struct ExceptionRESTResponse<TObjectCode>(RESTObject<TObjectCode> RESTObject, HttpStatusCode ResponseStatus = HttpStatusCode.InternalServerError)
     where TObjectCode : struct, IEquatable<TObjectCode>;

public delegate Task<ExceptionRESTResponse<TObjectCode>> ExceptionResponseFactory<TObjectCode>(
    HttpRequest request,
    Exception? exception,
    IServiceProvider services,
    HttpContext context
) where TObjectCode : struct, IEquatable<TObjectCode>;

public class RESTExceptionHandler<TObjectCode> where TObjectCode : struct, IEquatable<TObjectCode>
{
    private readonly RequestDelegate _next;
    private readonly IServiceProvider Services;
    private readonly ExceptionResponseFactory<TObjectCode> ExceptionResponseFactory;

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