namespace PuduIOT.DA.Middleware
{
    public class KeepAliveMiddleware
    {
        private readonly RequestDelegate _next;

        public KeepAliveMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
           

            // Call the next middleware in the pipeline
            await _next(context);
        }
    }
    public static class KeepAliveMiddlewareExtensions
    {
        public static IApplicationBuilder UseKeepAliveMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<KeepAliveMiddleware>();
        }
    }
}
