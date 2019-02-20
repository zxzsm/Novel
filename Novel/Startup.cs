using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.WebEncoders;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Novel.Entity;
using Novel.Entity.Models;
using System;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Unicode;
using System.Threading.Tasks;

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
                options.CheckConsentNeeded = context => false;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });
            services.Configure<WebEncoderOptions>(options =>
            {
                options.TextEncoderSettings = new TextEncoderSettings(UnicodeRanges.BasicLatin, UnicodeRanges.CjkUnifiedIdeographs);
            });

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
              .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options => {
                  //options.AccessDeniedPath = "/Account/Forbidden/";
                  options.LoginPath = "/login.html";
              });

            StringCommon.ConnectionString = Configuration.GetConnectionString("BookDatabase");
            services.AddMvc(
                options =>
                {
                    options.Filters.Add(typeof(GlobalExceptionFilter)); // an instance
                }
                ).SetCompatibilityVersion(CompatibilityVersion.Version_2_2).AddJsonOptions(options =>
            {
                options.SerializerSettings.ContractResolver = new DefaultContractResolver();
                //忽略循环引用
                options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            });
            services.AddDbContext<BookContext>(options => options.UseSqlServer(StringCommon.ConnectionString));
            services.AddRouting(options => options.LowercaseUrls = true);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            Func<StatusCodeContext, Task> handler = async context =>
            {
                var response = context.HttpContext.Response;
                if (response.StatusCode < 500)
                {
                    await response.WriteAsync($"Client error ({response.StatusCode})");
                }
                else
                {
                    await response.WriteAsync($"Server error ({response.StatusCode})");
                }   
            };
            app.UseStatusCodePages(handler);
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();
            //注意app.UseAuthentication方法一定要放在下面的app.UseMvc方法前面，否者后面就算调用HttpContext.SignInAsync进行用户登录后，使用
            //HttpContext.User还是会显示用户没有登录，并且HttpContext.User.Claims读取不到登录用户的任何信息。
            //这说明Asp.Net OWIN框架中MiddleWare的调用顺序会对系统功能产生很大的影响，各个MiddleWare的调用顺序一定不能反
            app.UseAuthentication();
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                             name: "novel",
                            template: "book/{id}.html",
                            defaults: new { controller = "Index", action = "Novel" });
                routes.MapRoute(
                             name: "content",
                            template: "content/{itemId}.html",
                            defaults: new { controller = "Index", action = "Content" });
                routes.MapRoute(
                        name: "category",
                        template: "/{action}/{category}.html",
                        defaults: new { controller = "Index", action = "Category" });
                routes.MapRoute(
                            name: "default",
                            template: "/",
                            defaults: new { controller = "Index", action = "Index" });

                routes.MapRoute(
                            name: "default3",
                            template: "{action}.html/",
                            defaults: new { controller = "Index" });
               
                routes.MapRoute(
                           name: "default4",
                           template: "{controller}/{action}",
                           defaults: new { controller = "Index" });
               
            });
        }
    }
}
