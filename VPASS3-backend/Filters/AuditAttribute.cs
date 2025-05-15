using Microsoft.AspNetCore.Mvc.Filters;
using VPASS3_backend.Interfaces;

namespace VPASS3_backend.Filters
{
    public class AuditAttribute : ActionFilterAttribute
    {
        private readonly string _actionName;

        public AuditAttribute(string actionName)
        {
            _actionName = actionName;
        }

        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var executedContext = await next(); // Ejecuta el endpoint

            var httpContext = context.HttpContext;
            var auditService = httpContext.RequestServices.GetRequiredService<IAuditLogService>();

            var statusCode = executedContext.HttpContext.Response.StatusCode;

            await auditService.LogAsync(httpContext, _actionName, statusCode);
        }
    }
}