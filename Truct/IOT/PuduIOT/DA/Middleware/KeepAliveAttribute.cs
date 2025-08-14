using Microsoft.AspNetCore.Mvc.Filters;

namespace PuduIOT.DA.Middleware
{
    public class KeepAliveAttribute : ActionFilterAttribute
    {
        public override void OnResultExecuting(ResultExecutingContext context)
        {
            // Set the Connection header to keep-alive in the response
            //context.HttpContext.Response.Headers.Add("Connection", "keep-alive");
            //context.HttpContext.Response.Headers.Add("Keep-Alive", "timeout=5");

            base.OnResultExecuting(context);
        }
    }
}
