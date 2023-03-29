using Microsoft.AspNetCore.Mvc;

namespace DiegoG.REST.ASPNET;

/// <summary>
/// Represents an <see cref="ObjectResult"/> that may only have a <see cref="RESTObject{TObjectCode}"/> for a value
/// </summary>
/// <remarks>
/// While it's not possible to verify the value when its set, setting it to an object that is not a <see cref="RESTObject{TObjectCode}"/> will cause an exception when attempting to format, or otherwise execute
/// </remarks>
public class RESTObjectResult<TObjectCode> : ObjectResult where TObjectCode : struct, IEquatable<TObjectCode>
{
    /// <summary>
    /// Instances a new object of type <see cref="RESTObjectResult{TObjectCode}"/> with <paramref name="value"/> as its value
    /// </summary>
    /// <param name="value"></param>
    public RESTObjectResult(RESTObject<TObjectCode> value) : base(value) { }

    /// <inheritdoc/>
    public override void ExecuteResult(ActionContext context)
    {
        if (Value is null or RESTObject<TObjectCode>)
        {
            base.ExecuteResult(context);
            return;
        }
        throw new InvalidOperationException($"Cannot Execute the result of this {typeof(RESTObjectResult<TObjectCode>)} when Value is not a {typeof(RESTObject<TObjectCode>)}");
    }

    /// <inheritdoc/>
    public override Task ExecuteResultAsync(ActionContext context)
    {
        if (Value is null or RESTObject<TObjectCode>)
            return base.ExecuteResultAsync(context);
        throw new InvalidOperationException($"Cannot Execute the result of this {typeof(RESTObjectResult<TObjectCode>)} when Value is not a {typeof(RESTObject<TObjectCode>)}");
    }

    /// <inheritdoc/>
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
