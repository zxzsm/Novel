using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Novel.Entity;
using Novel.Entity.Models;

namespace Novel
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
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            StringCommon.ConnectionString = Configuration.GetConnectionString("BookDatabase");
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            services.AddDbContext<BookContext>(options => options.UseSqlServer(StringCommon.ConnectionString));
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
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                             name: "novel",
                            template: "novel/{id}.html",
                            defaults: new { controller = "Index", action = "Novel" });
                routes.MapRoute(
                             name: "content",
                            template: "content/{itemId}.html",
                            defaults: new { controller = "Index", action = "Content" });
                routes.MapRoute(
                            name: "default",
                            template: "/",
                            defaults: new { controller = "Index", action = "Index" });
                routes.MapRoute(
                            name: "default2",
                            template: "search/",
                            defaults: new { controller = "Index", action = "Search" });
                routes.MapRoute(
                            name: "default3",
                            template: "searchkeyword/",
                            defaults: new { controller = "Index", action = "SearchKeyword" });
            });
        }
    }
}
