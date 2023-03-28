using Microsoft.AspNetCore.Mvc;

namespace DiegoG.REST.ASPNET;

public class RESTObjectResult<TObjectCode> : ObjectResult where TObjectCode : struct, IEquatable<TObjectCode>
{
    public RESTObjectResult(RESTObject<TObjectCode> value) : base(value) { }

    public override void ExecuteResult(ActionContext context)
    {
        if (Value is null or RESTObject<TObjectCode>)
        {
            base.ExecuteResult(context);
            return;
        }
        throw new InvalidOperationException($"Cannot Execute the result of this {typeof(RESTObjectResult<TObjectCode>)} when Value is not a {typeof(RESTObject<TObjectCode>)}");
    }

    public override Task ExecuteResultAsync(ActionContext context)
    {
        if (Value is null or RESTObject<TObjectCode>)
            return base.ExecuteResultAsync(context);
        throw new InvalidOperationException($"Cannot Execute the result of this {typeof(RESTObjectResult<TObjectCode>)} when Value is not a {typeof(RESTObject<TObjectCode>)}");
    }

    public override void OnFormatting(ActionContext context)
    {
        if (Value is null or RESTObject<TObjectCode>)
        {
            base.OnFormatting(context);
            return;
        }
        throw new InvalidOperationException($"Cannot Execute the result of this {typeof(RESTObjectResult<TObjectCode>)} when Value is not a {typeof(RESTObject<TObjectCode>)}");
    }
}
