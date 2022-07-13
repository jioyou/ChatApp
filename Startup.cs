using ChatApp.Database;
using ChatApp.Models;
using ChatApp.Hubs;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using NETCore.MailKit.Extensions;
using NETCore.MailKit.Infrastructure.Internal;

namespace ChatApp
{
    public class Startup
    {
        private readonly IConfiguration _config;

        public Startup(IConfiguration config)
        {
            this._config = config;
        }
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            //数据库连接字符串
            var conectionString = _config.GetConnectionString("DefautConnection");
            //email
            var mailKitOptions = _config.GetSection("EmailOptions").Get<MailKitOptions>();
            services.AddMvc();
            //链接数据库
            services.AddDbContext<AppDbContext>(options => options.UseSqlServer(conectionString));
            //用户
            services.AddIdentity<User, IdentityRole>(options =>
            {
                options.Password.RequireDigit = false;//要求密码中的数字介于0-9 之间。
                options.Password.RequireLowercase = false;//密码中需要小写字符。
                options.Password.RequireNonAlphanumeric = false;//密码中需要一个非字母数字字符。
                options.Password.RequireUppercase = false; //密码中需要大写字符。
                options.Password.RequiredLength = 6;//密码的最小长度。
                options.SignIn.RequireConfirmedEmail = false;//登录加邮箱验证（netcore.mailkit）
                options.SignIn.RequireConfirmedPhoneNumber = false;//登录加手机验证
            })
                .AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders();
            //验证登录权限
            services.ConfigureApplicationCookie(config =>
            {
                config.Cookie.Name = "Identity.Cookie";
                config.LoginPath = "/Account/Login";
            });
            services.AddMailKit(config => config.UseMailKit(mailKitOptions));//需要本地开启stmp服务
            //使用SignalR
            services.AddSignalR();
            //允许跨域访问的跨域规则
            services.AddCors(options =>
            {
                //options.AddDefaultPolicy(
                //    builder =>
                //    {
                //        builder//.WithOrigins("https://example.com")  //允许特定来源的CORS请求
                //            .AllowAnyOrigin()
                //            .AllowAnyHeader()                       //允许任何请求头
                //            .WithMethods("GET", "POST")             //允许特定来源的HTTP方法
                //            .AllowCredentials();                    //允许所有来源跨域请求中的凭据
                //    });
                options.AddPolicy("DefaultPolicy",
                    builder =>
                    {
                        builder//.WithOrigins("https://example.com")  //允许特定来源的CORS请求
                            .SetIsOriginAllowed(_ => true)
                            .AllowAnyHeader()                       //允许任何请求头
                            .WithMethods("GET", "POST")             //允许特定来源的HTTP方法
                            .AllowCredentials();                    //允许所有来源跨域请求中的凭据
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

            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();

            // UseCors must be called before MapHub.
            app.UseCors();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHub<ChatHub>("/chatHub");
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapControllers().RequireCors("DefaultPolicy");
            });

            //app.UseEndpoints(endpoints =>
            //{
            //    endpoints.MapGet("/", async context =>
            //    {
            //        await context.Response.WriteAsync("Hello World!");
            //    });
            //});
        }
    }
}
