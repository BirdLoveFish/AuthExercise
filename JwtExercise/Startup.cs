using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using JwtExercise.Extensions;
using System.Text;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace JwtExercise
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            services.AddAuthentication("bearer")
                .AddJwtBearer("bearer", options=>
                {
                    #region 从query中获取token
                    options.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = context =>
                        {
                            if (context.Request.Query.ContainsKey("access_token"))
                            {
                                context.Token = context.Request.Query["access_token"];
                            }
                            return Task.CompletedTask;
                        }
                    };
                    #endregion

                    #region 验证token
                    //秘钥
                    var securityKey = new SymmetricSecurityKey
                        (Encoding.UTF8.GetBytes(JwtInformation.SecurityKey));

                    options.TokenValidationParameters = new TokenValidationParameters
                    {
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
                        ValidateLifetime = true,
                        RequireExpirationTime = true,
                    };
                    #endregion
                });

            services.AddAuthorization(options=>
            {
                //要求必须要有gender claim
                options.AddPolicy("gender", policy =>
                {
                    //JwtRegisteredClaimNames.Gender无法被解析出来
                    //policy.RequireClaim(JwtRegisteredClaimNames.Gender);
                    //用ClaimTypes.Gender可以被解析
                    policy.RequireClaim(ClaimTypes.Gender);
                });
                //要求年龄
                options.AddPolicy("needAge", policy =>
                {
                    policy.RequireClaim("age");
                });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            //跨域请求
            app.UseCors(options =>
            {
                //允许任何的客户端
                options.AllowAnyOrigin();
                //允许任何的头部信息
                options.AllowAnyHeader();
            });

            app.UseRouting();
            //授权认证
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
