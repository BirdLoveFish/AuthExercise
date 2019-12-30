using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using IdentityModel;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;

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

            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

            services.AddAuthentication(options=>
            {
                options.DefaultScheme = "Cookie";
                options.DefaultChallengeScheme = "oidc";
            })
                .AddCookie("Cookie",options=>
                {
                    //policy����󷵻�403���ض����ַ��ͬʱquery�ᴫ��returnUrl
                    options.AccessDeniedPath = "/home/denied";
                })
                .AddOpenIdConnect("oidc",options =>
                {
                    options.ClientId = "client_id_mvc";
                    options.ClientSecret = "secret";
                    options.SaveTokens = true;
                    options.RequireHttpsMetadata = false;
                    options.Authority = "http://localhost:5000";
                    options.ResponseType = "code";

                    //���Claims������ȫ����claim��ֻ���Զ�������Ĳ���(openid��profile��scope)
                    //����openid�ǲ���ɾ���ģ�ɾ���˻ᱨ��
                    options.Scope.Clear();

                    //options.ClaimActions.Remove("idp");
                    options.Scope.Add("rc.scope");
                    //���Api����Ȩ��
                    options.Scope.Add("ApiOne");
                    options.Scope.Add("ApiTwo");

                    options.Scope.Add("openid");
                    options.Scope.Add("profile");
                    options.Scope.Add("email");
                    options.Scope.Add("phone");
                    options.Scope.Add("roles");

                    //��ȡrefresh_token
                    options.Scope.Add("offline_access");

                    //��ȡcliam������ķ�����ֻ����user�л��һ��name���Բ��ܶ�id_token���Ӱ��
                    options.GetClaimsFromUserInfoEndpoint = true;

                    //ɾ��cliam����������
                    options.ClaimActions.DeleteClaim("nbf");
                    //��ԭ����Ĭ��ɾ����cliam�ָ�
                    //options.ClaimActions.Remove("");

                    //Claim map ��ʱ����������Ǳ�Ҫ�ģ�map("role","role")
                    //options.ClaimActions.MapUniqueJsonKey("RawCoding.Color", "rc.Color");
                    //options.ClaimActions.MapUniqueJsonKey("role", "role");

                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        NameClaimType = JwtClaimTypes.GivenName,
                        //��claims��role��ֵ��identity��role
                        RoleClaimType = JwtClaimTypes.Role,
                    };
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
