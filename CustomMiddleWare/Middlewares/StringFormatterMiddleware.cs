using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace CustomMiddleWare.Middlewares
{
    public class StringFormatterMiddleware
    {
        //AspNetCore.Http altında bulunur ve next ifadesini tanımlayan yapıdır.
        private readonly RequestDelegate _next;

        public StringFormatterMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {

        }
    }
}
