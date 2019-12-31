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

                    //删除Client中User中cliam，但是id_token中不会被删除
                    //相当于在把本地的id_token至User的映射取消了
                    //options.ClaimActions.DeleteClaim("amr");
                    options.ClaimActions.DeleteClaim("sid");
                    options.ClaimActions.DeleteClaim("sub");
                    options.ClaimActions.DeleteClaim("auth");

                    //将原本被默认删除的cliam恢复
                    //options.ClaimActions.Remove("iat");

                    /* Claim map 有时候这个操作是必要的，map("role","role")
                     * 当Configuration中AlwaysIncludeUserClaimsInIdToken = false时
                     * 任然想要在User Claims和id_token中获取Claims
                     * 则需要用到下面Map
                     */
                    //options.ClaimActions.MapUniqueJsonKey("RawCoding.Color", "rc.Color");
                    //options.ClaimActions.MapUniqueJsonKey("role", "role");

                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        NameClaimType = JwtClaimTypes.Name,
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
            //else
            //{
            //    app.UseExceptionHandler("/Home/Error");
            //    app.UseHsts();
            //}
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
