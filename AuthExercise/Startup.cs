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
                //������Ȩ������
                //options.Filters.Add()
            });

            #region ��֤
            #region ������֤����
            //services.AddAuthentication("Cookie")
            //    .AddCookie("Cookie1", options =>
            //    {
            //        options.Cookie.Name = "Auth.Ex";
            //        options.LoginPath = "/Identity/Login";
            //    });
            #endregion

            #region ���������֤����
            //AddAuthentication�е�����ֻ��һ���������ѣ������Զ���
            //�����Լ�ָ�����ַ���

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

            #region ʹ��ϵͳĬ����֤������
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

            #region ��Ȩ

            #region �޸�Ĭ����Ȩ
            //���[Authorize�����]
            //services.AddAuthorization(options =>
            //{
            //    var policyBuilder = new AuthorizationPolicyBuilder();
            //    //policyBuilder.RequireClaim("height");
            //    //policyBuilder.RequireClaim(ClaimTypes.Name, "zhang");
            //    //policyBuilder.RequireAuthenticatedUser();
            //    options.DefaultPolicy = policyBuilder.Build();
            //});
            #endregion

            #region �Զ�����Ȩ
            services.AddAuthorization(options =>
            {
                //��ҪName
                options.AddPolicy("needName", policy =>
                {
                    policy.RequireClaim(ClaimTypes.Name);
                });

                //��ɫ
                options.AddPolicy("policyAdmin", policy =>
                {
                    policy.RequireRole("Admin");
                });

                //�Զ��������Ȩ
                options.AddPolicy("customAgePolicy", policy =>
                {
                    //����Newһ����ķ�ʽ
                    //policy.AddRequirements(new CustomAgeRequirement(15));
                    //������չ������ģʽ�����ageֻ����һ���̶�����
                    policy.AddCustomAgeRequirement(15);
                });

            });
            //ʹ���Զ���Requirement������ע��RequirementHandler
            //scope��singleton���ǿ��Ե�
            services.AddScoped<IAuthorizationHandler, CustomAgeRequirementHandler>();
            //������Դ����Ȩ
            services.AddSingleton<IAuthorizationHandler, DocumentAuthorizationCrudHandler>();
            #endregion
            #endregion

            #region Identity User
            //ע��DbContext
            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseSqlite("Data Source=app.db");
            });

            //ע��UserManager,SignManager,RoleManager
            //AddIdentity֮��ʹ��HttpContetx.SignInAsync��User��ֵ�ķ�ʽ�ͻ�ʧЧ
            //AddIdentity�᷵���Լ���Cookie

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

            #region �Զ�������ṩ����
            services.AddSingleton<IAuthorizationPolicyProvider, MinimumAgePolicyProvider>();
            services.AddSingleton<IAuthorizationHandler, MinAgeRequirementHandler>();
            #endregion

            #region Claimsת��
            //������Ȩ�������֮ǰ����
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
            //�����֤�м��������UseRouting֮��UseEndpoints֮ǰ����
            app.UseAuthentication();
            //��Ȩ
            app.UseAuthorization();

            dbContext.Database.EnsureCreated();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
