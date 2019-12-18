using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AuthExercise.Auth;
using AuthExercise.Database;
using AuthExercise.Model;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace AuthExercise
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
            services.AddControllers(options=>
            {
                //定义授权过滤器
                //options.Filters.Add()
            });

            #region 认证
            #region 定义认证方案
            //services.AddAuthentication("Cookie")
            //    .AddCookie("Cookie1", options =>
            //    {
            //        options.Cookie.Name = "Auth.Ex";
            //        options.LoginPath = "/Identity/Login";
            //    });
            #endregion

            #region 定义多种认证方案
            //AddAuthentication中的名字只是一个方案而已，可以自定义
            //可以自己指定多种方案

            //services.AddAuthentication("Cookie")
            //    .AddCookie("Cookie1", options =>
            //    {
            //        options.Cookie.Name = "Auth.Ex";
            //        options.LoginPath = "/Identity/Login";
            //    })
            //    .AddCookie("Cookie", options =>
            //    {
            //        options.Cookie.Name = "Auth.Ex2";
            //        options.LoginPath = "/Identity/Login";
            //    });
            #endregion

            #region 使用系统默认认证方案名
            // JwtBearerDefaults.AuthenticationScheme ==> "Bearer"
            // CookieAuthenticationDefaults.AuthenticationScheme = "Cookie"
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
                {
                    options.Cookie.Name = "Auth.Ex";
                    options.LoginPath = "/Identity/Login";
                });
            #endregion

            #endregion

            #region 授权

            #region 修改默认授权
            //针对[Authorize的情况]
            //services.AddAuthorization(options =>
            //{
            //    var policyBuilder = new AuthorizationPolicyBuilder();
            //    //policyBuilder.RequireClaim("height");
            //    //policyBuilder.RequireClaim(ClaimTypes.Name, "zhang");
            //    //policyBuilder.RequireAuthenticatedUser();
            //    options.DefaultPolicy = policyBuilder.Build();
            //});
            #endregion

            #region 自定义授权
            services.AddAuthorization(options =>
            {
                //需要Name
                options.AddPolicy("needName", policy =>
                {
                    policy.RequireClaim(ClaimTypes.Name);
                });

                //角色
                options.AddPolicy("policyAdmin", policy =>
                {
                    policy.RequireRole("Admin");
                });

                //自定义策略授权
                options.AddPolicy("customAgePolicy", policy =>
                {
                    //采用New一个类的方式
                    //policy.AddRequirements(new CustomAgeRequirement(15));
                    //采用扩展方法的模式，这个age只能是一个固定的数
                    policy.AddCustomAgeRequirement(15);
                });

            });
            //使用自定义Requirement，必须注册RequirementHandler
            //scope和singleton都是可以的
            services.AddScoped<IAuthorizationHandler, CustomAgeRequirementHandler>();
            //基于资源的授权
            services.AddSingleton<IAuthorizationHandler, DocumentAuthorizationCrudHandler>();
            #endregion
            #endregion

            #region Identity User
            //注入DbContext
            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseSqlite("Data Source=app.db");
            });

            //注入UserManager,SignManager,RoleManager
            //AddIdentity之后使用HttpContetx.SignInAsync给User赋值的方式就会失效
            //AddIdentity会返回自己的Cookie

            //services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            // {
            //     options.Password.RequireDigit = false;
            //     options.Password.RequireLowercase = false;
            //     options.Password.RequiredLength = 4;
            //     options.Password.RequiredUniqueChars = 0;
            //     options.Password.RequireNonAlphanumeric = false;
            //     options.Password.RequireUppercase = false;
            // })
            //    .AddEntityFrameworkStores<AppDbContext>()
            //    .AddDefaultTokenProviders();
            #endregion

            #region 自定义策略提供程序
            services.AddSingleton<IAuthorizationPolicyProvider, MinimumAgePolicyProvider>();
            services.AddSingleton<IAuthorizationHandler, MinAgeRequirementHandler>();
            #endregion

            #region Claims转换
            //会在授权处理程序之前运行
            //services.AddSingleton<IClaimsTransformation, ClaimsTransformation>();
            #endregion
        }

        public void Configure(
            IApplicationBuilder app, 
            IWebHostEnvironment env,
            AppDbContext dbContext)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();
            //身份认证中间件必须在UseRouting之后，UseEndpoints之前调用
            app.UseAuthentication();
            //授权
            app.UseAuthorization();

            dbContext.Database.EnsureCreated();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
