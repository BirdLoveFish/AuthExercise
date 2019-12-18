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
                    #region ��query�л�ȡtoken
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

                    #region ��֤token
                    //��Կ
                    var securityKey = new SymmetricSecurityKey
                        (Encoding.UTF8.GetBytes(JwtInformation.SecurityKey));

                    options.TokenValidationParameters = new TokenValidationParameters
                    {
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
                        ValidateLifetime = true,
                        RequireExpirationTime = true,
                    };
                    #endregion
                });

            services.AddAuthorization(options=>
            {
                //Ҫ�����Ҫ��gender claim
                options.AddPolicy("gender", policy =>
                {
                    //JwtRegisteredClaimNames.Gender�޷�����������
                    //policy.RequireClaim(JwtRegisteredClaimNames.Gender);
                    //��ClaimTypes.Gender���Ա�����
                    policy.RequireClaim(ClaimTypes.Gender);
                });
                //Ҫ������
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

            //��������
            app.UseCors(options =>
            {
                //�����κεĿͻ���
                options.AllowAnyOrigin();
                //�����κε�ͷ����Ϣ
                options.AllowAnyHeader();
            });

            app.UseRouting();
            //��Ȩ��֤
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
