using System.Collections.Immutable;
using System.Net;
using System.Text;
using System.Web.Mvc;
using DiegoG.REST.ASPNET;
using DiegoG.REST.Json;
using DiegoG.REST.TestAPI.RESTModels.Responses;
using DiegoG.REST.TestAPI.RESTModels.Responses.Data;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace DiegoG.REST.TestAPI;

public class Program
{
    public static Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.

        builder.Services.AddControllers(o => o.EnableEndpointRouting = false);
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        builder.Services.AddRESTObjectSerializer(new JsonRESTSerializer<ResponseCode>());
        builder.Services.AddRESTObjectTypeTable(RESTResponseTypeTable.Instance);

        builder.Services.UseRESTObjectValidationFilter<ResponseCode>();

        builder.Services.UseRESTInvalidModelStateResponse(
            (c, e) => new RESTObjectResult<ResponseCode>(new ErrorResponse(e))
            {
                StatusCode = (int)HttpStatusCode.BadRequest
            }
        );

        var app = builder.Build();
        
        app.UseRESTExceptionHandler(
                (r, e, serv, con) => Task.FromResult<ExceptionRESTResponse<ResponseCode>>(
                    new(
                        new ErrorResponse(e),
                        HttpStatusCode.InternalServerError
                    )
                )
            );


        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseMvc()
           //.UseExceptionHandler()
           .UseHttpsRedirection()
           .UseAuthorization();

        app.MapControllers();

        return app.RunAsync();
    }
}
