using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using IdentityServer.Database;
using IdentityServer.Model;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Hosting.Internal;
using Microsoft.IdentityModel.Tokens;

namespace IdentityServer
{
    public class Startup
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _env;

        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            _configuration = configuration;
            _env = env;
        }
        public void ConfigureServices(IServiceCollection services)
        {
            //ʹ�����ݿ������ַ���
            var migrationsAssembly = typeof(Startup).GetTypeInfo()
                .Assembly.GetName().Name;
            string connectionString = _configuration.GetConnectionString("AuthExercise");

            //ע��DbContext
            services.AddDbContext<AppDbContext>(options =>
            {
                //use memory database
                //options.UseInMemoryDatabase("memory");
                //use sqlserver database
                options.UseSqlServer(connectionString);
            });

            //use Microsoft identity
            services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                 options.Password.RequireDigit = false;
                 options.Password.RequireLowercase = false;
                 options.Password.RequiredLength = 4;
                 options.Password.RequiredUniqueChars = 0;
                 options.Password.RequireNonAlphanumeric = false;
                 options.Password.RequireUppercase = false;
            })
                .AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders();

            //���֤���·��
            //var rootPath = _env.ContentRootPath;
            //var cerPath = Path.Combine(rootPath, _configuration["Certificates:CerPath"]);
            //var password = _configuration["Certificates:Password"];

            var rootPath = _env.WebRootPath;
            var cerPath = Path.Combine(rootPath, "idsrv4.pfx");
            var password = _configuration["Certificates:Password"];

            //˳�����д�������Ȼ�ᱨ��
            services.AddIdentityServer()
                //add temporary certificate
                //.AddDeveloperSigningCredential()
                //add custom certificate
                .AddSigningCredential(new X509Certificate2(cerPath, password))
                //add application user
                .AddAspNetIdentity<ApplicationUser>()
                //����ڴ�����
                //.AddInMemoryApiResources(Configuration.GetApis())
                //.AddInMemoryClients(Configuration.GetClients())
                //.AddInMemoryIdentityResources(Configuration.GetIdentity())
                //��������ӵ����ݿ�
                .AddConfigurationStore(options =>
                {
                    options.ConfigureDbContext = b => b.UseSqlServer(connectionString,
                        sql => sql.MigrationsAssembly(migrationsAssembly));
                })
                .AddOperationalStore(options =>
                {
                    options.ConfigureDbContext = b => b.UseSqlServer(connectionString,
                        sql => sql.MigrationsAssembly(migrationsAssembly));
                })
                ;

            //�Զ���IdentityServer�ĵ�¼·��
            services.ConfigureApplicationCookie(options =>
            {
                options.Cookie.Name = "IdentityServer.Cookie";
                options.LoginPath = "/Auth/Login";
            });

            services.AddControllersWithViews();
        }

        public void Configure(IApplicationBuilder app)
        {
            if (_env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseRouting();
            app.UseIdentityServer();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
            });
        }
    }
}
