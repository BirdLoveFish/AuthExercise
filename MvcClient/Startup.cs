using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace MvcClient
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();

            services.AddAuthentication(options=>
            {
                options.DefaultScheme = "Cookie";
                options.DefaultChallengeScheme = "oidc";
            })
                .AddCookie("Cookie")
                .AddOpenIdConnect("oidc",options =>
                {
                    options.ClientId = "client_id_mvc";
                    options.ClientSecret = "secret";
                    options.SaveTokens = true;
                    options.RequireHttpsMetadata = false;
                    options.Authority = "http://localhost:5000";
                    options.ResponseType = "code";

                    //ɾ��cliam
                    options.ClaimActions.DeleteClaim("amr");
                    //Claim map
                    options.ClaimActions.MapUniqueJsonKey("RawCoding.Color","rc.Color");

                    //���Claims������ȫ����claim��ֻ���Զ�������Ĳ���(openid��profile��scope)
                    //����openid�ǲ���ɾ���ģ�ɾ���˻ᱨ��
                    //options.ClaimActions.Clear();
                    //��ȡcliam������ķ���
                    options.GetClaimsFromUserInfoEndpoint = true;

                    options.Scope.Add("rc.scope");
                    options.Scope.Add("ApiOne");
                    options.Scope.Add("offline_access");
                });

            services.AddHttpClient();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
