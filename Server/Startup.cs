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
                #region ��query�л�ȡtoken
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

                #region ��֤token
                //��Կ
                var securityKey = new SymmetricSecurityKey
                    (Encoding.UTF8.GetBytes(JwtInformation.SecurityKey));

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ClockSkew = TimeSpan.Zero,
                    //������
                    ValidIssuer = JwtInformation.Issure,
                    ValidateIssuer = true,
                    //������
                    ValidAudience = JwtInformation.Audience,
                    ValidateAudience = true,
                    //��Կ
                    IssuerSigningKey = securityKey,
                    ValidateIssuerSigningKey = true,
                    //ʱ��
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
