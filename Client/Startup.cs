using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Client
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

            services.AddAuthentication(options =>
            {

                options.DefaultAuthenticateScheme = "ClientCookie";
                options.DefaultSignInScheme = "ClientCookie";
                options.DefaultChallengeScheme = "OurServer";
            })
                .AddCookie("ClientCookie")
                .AddOAuth("OurServer", options =>
                {
                    //这里必须要用iis express来启动，否则会出错
                    options.CallbackPath = "/home/callback";
                    options.ClientId = "client_id";
                    options.ClientSecret = "client_secret";
                    options.AuthorizationEndpoint = Configuration["AuthorizationEndpoint"];
                    options.TokenEndpoint = Configuration["TokenEndpoint"];
                    
                    options.SaveTokens = true;

                    options.Events = new OAuthEvents()
                    {
                        OnCreatingTicket = context =>
                        {
                            var accessToken = context.AccessToken;
                            var securityToken = new JwtSecurityTokenHandler().ReadJwtToken(accessToken);
                            //这里用Convert.FromBase64String解析时出错
                            //var base64payload = accessToken.Split('.')[1];
                            //var bytes = Convert.FromBase64String(base64payload);
                            //var jsonPayload = Encoding.UTF8.GetString(bytes);
                            var jsonPayload = securityToken.Payload.ToDictionary(a=>a.Key,b=>b.Value);
                            //var claims = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonPayload);
                            foreach (var claim in jsonPayload)
                            {
                                context.Identity.AddClaim(new Claim(claim.Key, claim.Value.ToString()));
                            }

                            return Task.CompletedTask;
                        }
                    };
                });

            services.AddHttpClient();
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
