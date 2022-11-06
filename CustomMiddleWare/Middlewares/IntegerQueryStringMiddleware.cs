using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace CustomMiddleWare.Middlewares
{
    //Bir sınıf oluşturuyor
    public class IntegerQueryStringMiddleware
    {
        //AspNetCore.Http altında bulunur ve next ifadesini tanımlayan yapıdır.
        private readonly RequestDelegate _next;

        public IntegerQueryStringMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        //Startup içerisindeki süreci buradan yöneteceğiz. Bu metot içeriisne Startup dosyası içerisinde oluşturduğumuz ilk middleware yapısını alıyoruz.
        public async Task Invoke(HttpContext context)
        {
            if (context.Request.Path == "/products")
            {
                var value = context.Request.Query["category"].ToString();
                if (int.TryParse(value, out int intValue))
                {
                    await context.Response.WriteAsync($"Kategori sayısal bir ifadedir. {value}");
                }
                else
                {
                    context.Items["value"] = value;
                    //next'i _next olarak değiştiriyoruz.
                    //next hata verecektir. Next ile context'i göndermemiz gerekecektir. Burası değiştirilmelidir.
                    await _next(context);
                }
            }
            else
            {
                    //next'i _next olarak değiştiriyoruz.
                    //next hata verecektir. Next ile context'i göndermemiz gerekecektir. Burası değiştirilmelidir.
                await _next(context);
            }
        }
    }
}
