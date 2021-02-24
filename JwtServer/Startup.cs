using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using JwtServer.Models;
using JwtServer.Data;
using Pomelo.EntityFrameworkCore;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using JwtServer.Services;
using System;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;

namespace JwtServer
{
    public class Startup
    {
        readonly string VueClientOrigin = "_myAllowSpecificOrigins";
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<UserDBContext>(opt =>
                                   opt.UseMySql(Configuration.
                                   GetConnectionString("UserInfoDBConnection"), 
                                   new MySqlServerVersion(new Version(10, 1, 40)),
                                   mySqlOptions => mySqlOptions.CharSetBehavior(CharSetBehavior.NeverAppend)));

            services.AddIdentity<User, IdentityRole>(opts =>
            {
                opts.Password.RequireDigit = false;
                opts.Password.RequiredLength = 6;
                opts.Password.RequireNonAlphanumeric = false;
                opts.Password.RequireUppercase = false;
                opts.Password.RequireLowercase = false;
            })
                .AddEntityFrameworkStores<UserDBContext>();

            services.AddDatabaseDeveloperPageExceptionFilter();

            services.AddCors(options =>
            {
                options.AddPolicy(name: VueClientOrigin,
                                  builder =>
                                  {
                                      builder.WithOrigins("http://localhost:8080", 
                                          "https://featherwhite.github.io",
                                          "https://thinkingme.xyz:443",
                                          "https://www.thinkingme.xyz:443",
                                          "https://thinkingme.xyz:44302",
                                          "https://www.thinkingme.xyz:44302")
                                      .AllowAnyHeader()
                                      .AllowAnyMethod();
                                  });
            });

            services.Configure<JwtSetting>(Configuration.GetSection("JwtSetting"));
            services.AddScoped<ITokenService, TokenService>();

            services.AddControllers();
            services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders =
                    ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseForwardedHeaders();
            }


            //app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors(VueClientOrigin);

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
