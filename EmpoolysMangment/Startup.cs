using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EmpoolysMangment.Data;
using EmpoolysMangment.Models;
using EmpoolysMangment.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EmpoolysMangment
{
    public class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().AddXmlSerializerFormatters();
            services.AddDbContext<AppDbContext>(options => 
            options.UseSqlServer(_configuration.GetConnectionString("DevConnectionString")));

            services.AddIdentity<ApplicationUser, IdentityRole>(option =>
            {
                option.Password.RequiredLength = 10;
                option.Password.RequiredUniqueChars = 3;
            }).AddEntityFrameworkStores<AppDbContext>();

            /*  services.AddMvc(options =>
              {
                  var policy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .Build();
                  options.Filters.Add(new AuthorizeFilter(policy));
              }).AddXmlSerializerFormatters();*/
            services.AddAuthentication()
                .AddGoogle(option =>
                {
                    option.ClientId = "186633041467-2g2tep9jjn04c6sgtkenrihndnebqafu.apps.googleusercontent.com";
                    option.ClientSecret = "CQaazfRVqiodJz87u6Rky21Q";


                }).AddFacebook(option =>
                {
                    option.AppId = "284265502572900";
                    option.AppSecret = "110ad9977ac5f8b0e248a3001f1b09f7";
                });

            services.AddScoped<IEmpoyleeRepository, SQLEmpoyleeRepository>();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseStatusCodePagesWithRedirects("/Error/{0}");
            }
            app.UseStaticFiles();
            app.UseAuthentication();
            //app.UseMvcWithDefaultRoute();
            app.UseDatabaseErrorPage();
            app.UseMvc(routes =>
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}"));
        }
    }
}
