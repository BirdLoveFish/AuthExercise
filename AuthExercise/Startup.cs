using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AuthExercise.Auth;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
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
            services.AddControllers();

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
            services.AddAuthorization(options =>
            {
                var policyBuilder = new AuthorizationPolicyBuilder();
                policyBuilder.RequireClaim("height");
                options.DefaultPolicy = policyBuilder.Build();
            });
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
                    //������չ������ģʽ
                    policy.AddCustomAgeRequirement(15);
                });
            });
            //ʹ���Զ���Requirement������ע��RequirementHandler
            //scope��singleton���ǿ��Ե�
            services.AddScoped<IAuthorizationHandler, CustomAgeRequirementHandler>();
            #endregion
            #endregion

        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
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

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
