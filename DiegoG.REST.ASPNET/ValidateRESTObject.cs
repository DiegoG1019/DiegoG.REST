using System.Collections.Concurrent;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace DiegoG.REST.ASPNET;

#if NET7_0_OR_GREATER
public class ValidateRESTObjectAttribute<TRESTObjectCode> : ServiceFilterAttribute
    where TRESTObjectCode : struct, IEquatable<TRESTObjectCode>
{
    public ValidateRESTObjectAttribute(Type objectCodeType) : base(typeof(ValidateRESTObjectActionFilter<TRESTObjectCode>)) { }
}
#endif

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

    public ValidateRESTObjectAttribute(Type objectCodeType) : base(ConstructType(objectCodeType)) { }
}

public class ValidateRESTObjectActionFilter<TRESTObjectCode> : IActionFilter
    where TRESTObjectCode : struct, IEquatable<TRESTObjectCode>
{
    private readonly RESTObjectTypeTable<TRESTObjectCode> TypeTable;
    public ValidateRESTObjectActionFilter(RESTObjectTypeTable<TRESTObjectCode> table)
    {
        TypeTable = table;
    }

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

    public void OnActionExecuted(ActionExecutedContext context) { }
}
