using CustomMiddleWare.Middlewares;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Server.IIS.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CustomMiddleWare
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRazorPages();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            #region Middleware Exercises
            /*
              //App use içerisinde asenktron olarak bir context nesnesi oluþturalým. Ayrýca birde next yapýsýný tanýmlýyoruz
            app.Use(async (context, next) =>
            {
                //Süreç içerisine kullanýcýnýn girmeye çalýþtýðý sayfa url' /profile ise 
                //Kullanýcýnýn girmeye çalýþtýðý sayfa /profile ise aþðýdaki komutta oluþturduðumuz response dönülecektir.
                //Eðer kullanýnýn girmiþ olduðu sayfa /profile deðil ise kullandýðýmýz middleware'da aþaðýdaký iþlem yapýlmayacaktýr.
                //Ayný zamanda bu middleware ile birlikte iþlem sonlandýðýndan dolayý sayfa gösterilmeyecektir. Çünkü bu middleare'den sonra baþka bir middleware yapýsýnýn çalýþacaðýný belirtmedik.

                if (context.Request.Path == "/profile")
                {
                    //Bir cevap nesnesi oluþtuyuroruz. Cevap içerisine istedðimiz bilgiyi ekleyebiliriz.
                    await context.Response.WriteAsync("kullanýcýn bilgileri");
                }
                //Eðer kullanýcýnýn girdiði sayfa /profile deðilse bu iþlemi bir sonraki middleware'e aktaracaðýný söylememiz gerekir.
                else
                {
                    //Next metodu ile iþlemi bir sonraki middleware'e aktarýyoruz.
                    await next();
                }
            });

            //User metoduna alternatif olarak kullanýlýr ve bu metot kullanýldýðýnda yukarýdaki gibi if else yapsýna ihtiyaç duymayýz
            app.Map("/orders",
                config =>
                config.Use(async (context, next) =>
                    await context.Response.WriteAsync("kullanýcý sipariþleri.")));


            app.MapWhen(
                //Gelen isteði QueryString, Http Method gibi yapýlarýný kontrol etmemizi saðlar. Aþaðýda kullanýcý POST isteði yaptýysa iþlem yapmak istiyorsak yapacaðýmýz iþlem örnek olarak verilmiþtir.
                context => context.Request.Method == "POST" &&
                            context.Request.Path == "/account",
                config =>
                            config.Use(async (context, next) =>
                            await context.Response.WriteAsync("account a post yapýldýðýnda çalýþcaktýr."))
                );


            app.Run(async (context) =>
            {
                await context.Response.WriteAsync("Hello world!");
            });
             */
            #endregion

            #region Custom Middleware Exercises

            //kullanabilmek için namespace eklemeniz gerekmektedir.
            app.UseIntegerQS();
            app.UseStringFormatter();

            //öncelikle süreci baþlatýyoruz.
            app.Use(async (context, next) =>
            {
                //Örneðimizde kullanýcý ürün lisleteme sayfasýný açarken CategoryID göndermek zorunda fakat categoryID bilgisi sayýsal bir ifadedir. Gönderilmediðinde veya sayýsal ifade dýþýnda ifade gönderilme durumunu yönetmek istiyoruz.
                if (context.Request.Path == "/products")
                {
                    //Query metodu ile kullanýcýn category isimli parametresini yakalýyoruz.
                    var value = context.Request.Query["category"].ToString();
                    //int.TryParse metodu value isimli deðiþkeni int'e convert etemeye çalýþacak eðer baþarýlý olursa category deðerinin sayýsal olduðunu anlayabileceðiz.
                    if (int.TryParse(value, out int intValue))
                    {
                        //süreci dolduruyoruz
                        await context.Response.WriteAsync($"Kategori sayýsal bir ifadedir. {value}");
                    }
                    else
                    {
                        //ifade int'e çevrilmeyen bir ifadeyse, context.Items isimli, koleksiyon içerisine reques süresinde request ile bilgi paylaþmak istediðimizde bu Dictionary listesinin kullanmaktayýz.
                        //Kýsacý uygulamada herþey isteiðimiz gibiyse querystring'den girilen deðeri request nesnesine ekleyebiliriz.
                        context.Items["value"] = value;
                        //Request içerisinde deðeri yerleþtirdikten sonra diðer middleware yapýsýna geçebiliriz. Bir sonraki middleware'de ise request'in Items koleksiyonuna ulaþarak istediðimiz deðeri kullanabiliriz.
                        await next();
                    }
                }
                //Eðer yukarýdaki süreç devreye giremezse süreç tamamen bitecektir. Bizler bu sürece girilmediðinde süreci devam etmesini isteidðimizden dolayý else bloðunu açýyoruz.
                else
                {
                    //Sonraki middleware yapýsýna geçiyoruz.
                    await next();
                }
            });


            //Ýkinci middleware yapýmýzý
            app.Use(async (context, next) => {
            //value ifadesinin ilk middleware'de aldýðýmýzdan tekrar querystring'den almamýza gerek yoktur. Koleksiyon içerisinden buna eriþebilir.
            //value isimli bir bilgi varsa
            if (context.Items["value"] != null)
            {
                //Koleksiyondaki value'yi elde ediyoruz.
                var value = context.Items["value"].ToString();

                //elde etitðimiz value ile isteiðimiz iþlemi yapabiliriz. Örneðin metinsel bir ifade ise SEO uyumluluðu için replace gibi iþlemlerde yapýlabilir.
                context.Items["value"] = value.ToLower();
            }
                //context'in null olup olmadýðýna bakýlmaksýzýn süreci bir sonraki middleware yapýsýna aktarýyoruz.
                await next();
            });


            //Oluþturulan querystring'i ekrana yazdýrmak için bir middleware daha kullanýyoruz. Fakat bu durum için yeni bir middleware yazmak yerine istersek app.Run içeriside süreç sonlanýrken bu iþlemi kontrol edebilirsiniz.

            app.Run(async (context) =>
            {
                if (context.Items["value"] != null)
                {
                    var value = context.Items["value"].ToString();
                    await context.Response.WriteAsync($"Category : {value}");
                }
                else await context.Response.WriteAsync("Query String Bulunamadý!");
            });

            #endregion


            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
            });
        }
    }
}
