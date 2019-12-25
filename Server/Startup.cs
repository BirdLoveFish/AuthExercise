using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace Server
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
            services.AddAuthentication("OAuth")
            .AddJwtBearer("OAuth", options =>
            {
                #region 从query中获取token
                //options.Events = new JwtBearerEvents
                //{
                //    OnMessageReceived = context =>
                //    {
                //        if (context.Request.Query.ContainsKey("access_token"))
                //        {
                //            context.Token = context.Request.Query["access_token"];
                //        }
                //        return Task.CompletedTask;
                //    }
                //};
                #endregion

                #region 验证token
                //秘钥
                var securityKey = new SymmetricSecurityKey
                    (Encoding.UTF8.GetBytes(JwtInformation.SecurityKey));

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ClockSkew = TimeSpan.Zero,
                    //发行者
                    ValidIssuer = JwtInformation.Issure,
                    ValidateIssuer = true,
                    //接受者
                    ValidAudience = JwtInformation.Audience,
                    ValidateAudience = true,
                    //秘钥
                    IssuerSigningKey = securityKey,
                    ValidateIssuerSigningKey = true,
                    //时间
                    //ValidateLifetime = true,
                    //RequireExpirationTime = true,
                };
                #endregion
            });

            services.AddControllersWithViews();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }


            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
            });
        }
    }
}
