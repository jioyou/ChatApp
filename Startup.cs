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
            //���ݿ������ַ���
            var conectionString = _config.GetConnectionString("DefautConnection");
            //email
            var mailKitOptions = _config.GetSection("EmailOptions").Get<MailKitOptions>();
            services.AddMvc();
            //�������ݿ�
            services.AddDbContext<AppDbContext>(options => options.UseSqlServer(conectionString));
            //�û�
            services.AddIdentity<User, IdentityRole>(options =>
            {
                options.Password.RequireDigit = false;//Ҫ�������е����ֽ���0-9 ֮�䡣
                options.Password.RequireLowercase = false;//��������ҪСд�ַ���
                options.Password.RequireNonAlphanumeric = false;//��������Ҫһ������ĸ�����ַ���
                options.Password.RequireUppercase = false; //��������Ҫ��д�ַ���
                options.Password.RequiredLength = 6;//�������С���ȡ�
                options.SignIn.RequireConfirmedEmail = false;//��¼��������֤��netcore.mailkit��
                options.SignIn.RequireConfirmedPhoneNumber = false;//��¼���ֻ���֤
            })
                .AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders();
            //��֤��¼Ȩ��
            services.ConfigureApplicationCookie(config =>
            {
                config.Cookie.Name = "Identity.Cookie";
                config.LoginPath = "/Account/Login";
            });
            services.AddMailKit(config => config.UseMailKit(mailKitOptions));//��Ҫ���ؿ���stmp����
            //ʹ��SignalR
            services.AddSignalR();
            //���������ʵĿ������
            services.AddCors(options =>
            {
                //options.AddDefaultPolicy(
                //    builder =>
                //    {
                //        builder//.WithOrigins("https://example.com")  //�����ض���Դ��CORS����
                //            .AllowAnyOrigin()
                //            .AllowAnyHeader()                       //�����κ�����ͷ
                //            .WithMethods("GET", "POST")             //�����ض���Դ��HTTP����
                //            .AllowCredentials();                    //����������Դ���������е�ƾ��
                //    });
                options.AddPolicy("DefaultPolicy",
                    builder =>
                    {
                        builder//.WithOrigins("https://example.com")  //�����ض���Դ��CORS����
                            .SetIsOriginAllowed(_ => true)
                            .AllowAnyHeader()                       //�����κ�����ͷ
                            .WithMethods("GET", "POST")             //�����ض���Դ��HTTP����
                            .AllowCredentials();                    //����������Դ���������е�ƾ��
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
