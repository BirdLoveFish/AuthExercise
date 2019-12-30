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
                    //policy错误后返回403的重定向地址，同时query会传入returnUrl
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

                    //清除Claims，不是全部的claim，只是自定义请求的部分(openid，profile，scope)
                    //但是openid是不能删除的，删除了会报错
                    options.Scope.Clear();

                    //options.ClaimActions.Remove("idp");
                    options.Scope.Add("rc.scope");
                    //添加Api请求权限
                    options.Scope.Add("ApiOne");
                    options.Scope.Add("ApiTwo");

                    options.Scope.Add("openid");
                    options.Scope.Add("profile");
                    options.Scope.Add("email");
                    options.Scope.Add("phone");
                    options.Scope.Add("roles");

                    //获取refresh_token
                    options.Scope.Add("offline_access");

                    //获取cliam的另外的方法，只能在user中获得一个name属性不能对id_token造成影响
                    options.GetClaimsFromUserInfoEndpoint = true;

                    //删除cliam，不起作用
                    options.ClaimActions.DeleteClaim("nbf");
                    //将原本被默认删除的cliam恢复
                    //options.ClaimActions.Remove("");

                    //Claim map 有时候这个操作是必要的，map("role","role")
                    //options.ClaimActions.MapUniqueJsonKey("RawCoding.Color", "rc.Color");
                    //options.ClaimActions.MapUniqueJsonKey("role", "role");

                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        NameClaimType = JwtClaimTypes.GivenName,
                        //将claims中role赋值给identity的role
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
