using Microsoft.AspNetCore.Mvc.Filters;

namespace DiegoG.REST.ASPNET;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class AddModelStateFeatureAttribute : Attribute, IActionFilter
{
    public void OnActionExecuting(ActionExecutingContext context)
    {
        context.HttpContext.Features.Set(context.ModelState);
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
    }
}