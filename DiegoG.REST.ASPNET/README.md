# DiegoG.REST.ASPNET

[![NuGet Package](https://img.shields.io/badge/NuGet-v1.1.1-blue)](https://www.nuget.org/packages/DiegoG.REST.ASPNET)

Adds extension methods, attribute action filters and middleware to register services, validations and catch both action exceptions and model validation errors to ensure the API always returns standardized responses in any eventuality

## Examples

```C#
public static Task Main(string[] args)
{
    // .... Other service setup code

    // Registers a serializer service for responses of ResponseCode
    builder.Services.AddRESTObjectSerializer(new JsonRESTSerializer<ResponseCode>());

    // Registers the Type table for responses of ResponseCode
    builder.Services.AddRESTObjectTypeTable(RESTResponseTypeTable.Instance);

    // Registers the service for RESTObject validation, to permit the usage of `ValidateRESTObjectAttribute` in controllers and actions
    builder.Services.UseRESTObjectValidationFilter<ResponseCode>();

    // Registers a response factory for model validation errors
    builder.Services.UseRESTInvalidModelStateResponse(
        (c, e) => new RESTObjectResult<ResponseCode>(new ErrorResponse(e))
        {
            StatusCode = (int)HttpStatusCode.BadRequest
        }
    );

    // .... Other service setup code

    var app = builder.Build();
       
    // Registers Middleware to handle action exceptions
    app.UseRESTExceptionHandler(
            (r, e, serv, con) => Task.FromResult<ExceptionRESTResponse<ResponseCode>>(
                new(
                    new ErrorResponse(e),
                    HttpStatusCode.InternalServerError
                )
            )
        );

    // .... Other app setup code
}
```