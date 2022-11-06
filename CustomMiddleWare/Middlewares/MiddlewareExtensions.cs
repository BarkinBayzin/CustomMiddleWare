using Microsoft.AspNetCore.Builder;

namespace CustomMiddleWare.Middlewares
{
    public static class MiddlewareExtensions
    {
        public static IApplicationBuilder UseIntegerQS(this IApplicationBuilder app)
        {
            return app.UseMiddleware<IntegerQueryStringMiddleware>();
        }

        public static IApplicationBuilder UseStringFormatter(this IApplicationBuilder app)
        {
            return app.UseMiddleware<StringFormatterMiddleware>();
        }
    }
}
