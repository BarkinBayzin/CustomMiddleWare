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
              //App use i�erisinde asenktron olarak bir context nesnesi olu�tural�m. Ayr�ca birde next yap�s�n� tan�ml�yoruz
            app.Use(async (context, next) =>
            {
                //S�re� i�erisine kullan�c�n�n girmeye �al��t��� sayfa url' /profile ise 
                //Kullan�c�n�n girmeye �al��t��� sayfa /profile ise a���daki komutta olu�turdu�umuz response d�n�lecektir.
                //E�er kullan�n�n girmi� oldu�u sayfa /profile de�il ise kulland���m�z middleware'da a�a��dak� i�lem yap�lmayacakt�r.
                //Ayn� zamanda bu middleware ile birlikte i�lem sonland���ndan dolay� sayfa g�sterilmeyecektir. ��nk� bu middleare'den sonra ba�ka bir middleware yap�s�n�n �al��aca��n� belirtmedik.

                if (context.Request.Path == "/profile")
                {
                    //Bir cevap nesnesi olu�tuyuroruz. Cevap i�erisine isted�imiz bilgiyi ekleyebiliriz.
                    await context.Response.WriteAsync("kullan�c�n bilgileri");
                }
                //E�er kullan�c�n�n girdi�i sayfa /profile de�ilse bu i�lemi bir sonraki middleware'e aktaraca��n� s�ylememiz gerekir.
                else
                {
                    //Next metodu ile i�lemi bir sonraki middleware'e aktar�yoruz.
                    await next();
                }
            });

            //User metoduna alternatif olarak kullan�l�r ve bu metot kullan�ld���nda yukar�daki gibi if else yaps�na ihtiya� duymay�z
            app.Map("/orders",
                config =>
                config.Use(async (context, next) =>
                    await context.Response.WriteAsync("kullan�c� sipari�leri.")));


            app.MapWhen(
                //Gelen iste�i QueryString, Http Method gibi yap�lar�n� kontrol etmemizi sa�lar. A�a��da kullan�c� POST iste�i yapt�ysa i�lem yapmak istiyorsak yapaca��m�z i�lem �rnek olarak verilmi�tir.
                context => context.Request.Method == "POST" &&
                            context.Request.Path == "/account",
                config =>
                            config.Use(async (context, next) =>
                            await context.Response.WriteAsync("account a post yap�ld���nda �al��cakt�r."))
                );


            app.Run(async (context) =>
            {
                await context.Response.WriteAsync("Hello world!");
            });
             */
            #endregion

            #region Custom Middleware Exercises

            //kullanabilmek i�in namespace eklemeniz gerekmektedir.
            app.UseIntegerQS();
            app.UseStringFormatter();

            //�ncelikle s�reci ba�lat�yoruz.
            app.Use(async (context, next) =>
            {
                //�rne�imizde kullan�c� �r�n lisleteme sayfas�n� a�arken CategoryID g�ndermek zorunda fakat categoryID bilgisi say�sal bir ifadedir. G�nderilmedi�inde veya say�sal ifade d���nda ifade g�nderilme durumunu y�netmek istiyoruz.
                if (context.Request.Path == "/products")
                {
                    //Query metodu ile kullan�c�n category isimli parametresini yakal�yoruz.
                    var value = context.Request.Query["category"].ToString();
                    //int.TryParse metodu value isimli de�i�keni int'e convert etemeye �al��acak e�er ba�ar�l� olursa category de�erinin say�sal oldu�unu anlayabilece�iz.
                    if (int.TryParse(value, out int intValue))
                    {
                        //s�reci dolduruyoruz
                        await context.Response.WriteAsync($"Kategori say�sal bir ifadedir. {value}");
                    }
                    else
                    {
                        //ifade int'e �evrilmeyen bir ifadeyse, context.Items isimli, koleksiyon i�erisine reques s�resinde request ile bilgi payla�mak istedi�imizde bu Dictionary listesinin kullanmaktay�z.
                        //K�sac� uygulamada her�ey istei�imiz gibiyse querystring'den girilen de�eri request nesnesine ekleyebiliriz.
                        context.Items["value"] = value;
                        //Request i�erisinde de�eri yerle�tirdikten sonra di�er middleware yap�s�na ge�ebiliriz. Bir sonraki middleware'de ise request'in Items koleksiyonuna ula�arak istedi�imiz de�eri kullanabiliriz.
                        await next();
                    }
                }
                //E�er yukar�daki s�re� devreye giremezse s�re� tamamen bitecektir. Bizler bu s�rece girilmedi�inde s�reci devam etmesini isteid�imizden dolay� else blo�unu a��yoruz.
                else
                {
                    //Sonraki middleware yap�s�na ge�iyoruz.
                    await next();
                }
            });


            //�kinci middleware yap�m�z�
            app.Use(async (context, next) => {
            //value ifadesinin ilk middleware'de ald���m�zdan tekrar querystring'den almam�za gerek yoktur. Koleksiyon i�erisinden buna eri�ebilir.
            //value isimli bir bilgi varsa
            if (context.Items["value"] != null)
            {
                //Koleksiyondaki value'yi elde ediyoruz.
                var value = context.Items["value"].ToString();

                //elde etit�imiz value ile istei�imiz i�lemi yapabiliriz. �rne�in metinsel bir ifade ise SEO uyumlulu�u i�in replace gibi i�lemlerde yap�labilir.
                context.Items["value"] = value.ToLower();
            }
                //context'in null olup olmad���na bak�lmaks�z�n s�reci bir sonraki middleware yap�s�na aktar�yoruz.
                await next();
            });


            //Olu�turulan querystring'i ekrana yazd�rmak i�in bir middleware daha kullan�yoruz. Fakat bu durum i�in yeni bir middleware yazmak yerine istersek app.Run i�eriside s�re� sonlan�rken bu i�lemi kontrol edebilirsiniz.

            app.Run(async (context) =>
            {
                if (context.Items["value"] != null)
                {
                    var value = context.Items["value"].ToString();
                    await context.Response.WriteAsync($"Category : {value}");
                }
                else await context.Response.WriteAsync("Query String Bulunamad�!");
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
