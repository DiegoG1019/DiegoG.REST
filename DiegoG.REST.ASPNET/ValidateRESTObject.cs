using System.Collections.Concurrent;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace DiegoG.REST.ASPNET;

#if NET7_0_OR_GREATER
/// <summary>
/// Adds an <see cref="IActionFilter"/> to the controller or action that ensures all <see cref="RESTObject{TObjectCode}"/>s received and sent from it are coded correctly in reference to the <see cref="RESTObjectTypeTable{TRESTObjectCode}"/> of the same code-type registered in this app's services
/// </summary>
/// <remarks>
/// Models that are not of type <see cref="RESTObject{TObjectCode}"/> are ignored and thus not validated
/// </remarks>
public class ValidateRESTObjectAttribute<TRESTObjectCode> : ServiceFilterAttribute
    where TRESTObjectCode : struct, IEquatable<TRESTObjectCode>
{
    public ValidateRESTObjectAttribute(Type objectCodeType) : base(typeof(ValidateRESTObjectActionFilter<TRESTObjectCode>)) { }
}
#endif

/// <summary>
/// Adds an <see cref="IActionFilter"/> to the controller or action that ensures all <see cref="RESTObject{TObjectCode}"/>s received and sent from it are coded correctly in reference to the <see cref="RESTObjectTypeTable{TRESTObjectCode}"/> of the same code-type registered in this app's services
/// </summary>
/// <remarks>
/// Models that are not of type <see cref="RESTObject{TObjectCode}"/> are ignored and thus not validated
/// </remarks>
public class ValidateRESTObjectAttribute : ServiceFilterAttribute
{
    private static readonly ConcurrentDictionary<Type, Type> ClosedGenerics = new();
    private static Type ConstructType(Type objectCodeType)
    {
        try
        {
            return ClosedGenerics.GetOrAdd(objectCodeType, oct => typeof(ValidateRESTObjectActionFilter<>).MakeGenericType(objectCodeType));
        }
        catch(Exception e)
        {
            throw new AggregateException($"The type {objectCodeType} is invalid as a Generic Type Argument for a TRESTObjectCode, which must be unmanaged and implement the IEquatable interface for its respective type", e);
        }
    }

    /// <summary>
    /// Instances a new object of type <see cref="ValidateRESTObjectAttribute"/>
    /// </summary>
    public ValidateRESTObjectAttribute(Type objectCodeType) : base(ConstructType(objectCodeType)) { }
}

/// <summary>
/// An <see cref="IActionFilter"/> to the controller or action that ensures all <see cref="RESTObject{TObjectCode}"/>s received and sent from it are coded correctly in reference to the <see cref="RESTObjectTypeTable{TRESTObjectCode}"/> of the same code-type registered in this app's services
/// </summary>
/// <remarks>
/// Models that are not of type <see cref="RESTObject{TObjectCode}"/> are ignored and thus not validated
/// </remarks>
public class ValidateRESTObjectActionFilter<TRESTObjectCode> : IActionFilter
    where TRESTObjectCode : struct, IEquatable<TRESTObjectCode>
{
    private readonly RESTObjectTypeTable<TRESTObjectCode> TypeTable;

    /// <summary>
    /// Instances a new object of type <see cref="ValidateRESTObjectAttribute"/>
    /// </summary>
    public ValidateRESTObjectActionFilter(RESTObjectTypeTable<TRESTObjectCode> table)
    {
        TypeTable = table;
    }

    /// <inheritdoc/>
    public void OnActionExecuting(ActionExecutingContext context)
    {
        foreach (var (key, val) in context.ActionArguments)
        {
            if (val is RESTObject<TRESTObjectCode> restobject)
            {
                if (TypeTable.GetTypeFor(restobject.Code) != restobject.GetType())
                    context.ModelState.AddModelError(key, "The type of the object is not valid for the given code");
            }
        }
    }

    /// <inheritdoc/>
    public void OnActionExecuted(ActionExecutedContext context) { }
}
