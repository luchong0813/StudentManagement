using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.Cors;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using StudentManagement.CustomerMiddlewares;
using StudentManagement.Models;
using StudentManagement.Security;

namespace StudentManagement
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
            services.AddMvc();

            services.AddDbContextPool<AppDbContext>(
                options => options.UseSqlServer(_configuration.GetConnectionString("StudentDBConnection"))
                );

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddErrorDescriber<CustomerErrorDescriber>()
                .AddEntityFrameworkStores<AppDbContext>();

            services.AddHttpContextAccessor();
            services.AddSingleton<IAuthorizationHandler, SuperAdminHander>();

            //策略结合声明授权
            services.AddAuthorization(options =>
            {
                //只有满足删除角色声明，此策略才能成功
                options.AddPolicy("DeleteRolePolicy", policy => policy.RequireClaim("删除角色"));
                options.AddPolicy("AdminRolePolicy", policy => policy.RequireRole("Admin"));

                //策略结合多个角色进行授权
                options.AddPolicy("SuperAdminPolicy", policy => policy.RequireRole("Admin", "User", "SuperManager"));

                //必须包含编辑角色声明且为true，此策略才能成功
                //options.AddPolicy("EditRolePolicy", policy =>
                //policy.RequireClaim("编辑角色", "true", "yes")
                //.RequireRole("Admin"));


                options.AddPolicy("EditRolePolicy", policy => policy.RequireAssertion(context => AuthorizeAccess(context)));
            });



            services.ConfigureApplicationCookie(options =>
            {
                //修改拒绝访问的路由地址
                options.AccessDeniedPath = new PathString("/Admin/AccessDenied");
                //统一系统的全局Cookie名称
                options.Cookie.Name = "StudentMangementCookie";
                //登录用户Cookie的有效期
                options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
                //是否对Cookie启用滑动过期时间
                options.SlidingExpiration = true;
            });

            services.Configure<IdentityOptions>(options =>
            {
                //密码中允许最大的重复数字
                options.Password.RequiredUniqueChars = 3;
                //密码至少包含一个非字母的数字的字符
                options.Password.RequireNonAlphanumeric = false;
                //密码是否必须包含小写字母
                options.Password.RequireLowercase = false;
                //密码是否必须包含大写字母
                options.Password.RequireUppercase = false;
            });

            services.AddScoped<IStudentRepository, StudentRepository>();

            services.AddControllersWithViews(config =>
            {
                var poicy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
                config.Filters.Add(new AuthorizeFilter(poicy));
            }).AddXmlDataContractSerializerFormatters();

            services.AddAuthentication().AddMicrosoftAccount(microsoftOptions =>
            {
                microsoftOptions.ClientId = _configuration["Authentication:Microsoft:ClientId"];
                microsoftOptions.ClientSecret = _configuration["Authentication:Microsoft:ClientSecret"];
            }).AddGitHub(options =>
            {
                options.ClientId = _configuration["Authentication:GitHub:ClientId"];
                options.ClientSecret=_configuration["Authentication:GitHub:ClientSecret"];
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {

                app.UseDeveloperExceptionPage();  //当代码发生异常时处理，主要给开发人员使用



                //app.UseDeveloperExceptionPage();
                //app.UseStatusCodePagesWithRedirects("/Error/{0}");
                //推荐使用这个
                //app.UseStatusCodePagesWithReExecute("/Error/{0}");  //拦截 404 NotFound
            }
            else
            {
                app.UseExceptionHandler("/Error");  //全局拦截代码异常
                app.UseStatusCodePagesWithRedirects("/Error/{0}");
            }

            //添加静态文件中间件
            app.UseStaticFiles();

            //添加验证中间件
            app.UseAuthentication();

            //添加路由中间件
            app.UseRouting();

            //授权中间件
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{Controller=Home}/{action=Index}/{id?}");
            });


        }

        private bool AuthorizeAccess(AuthorizationHandlerContext context)
        {
            return context.User.IsInRole("Admin") && context.User.HasClaim(claim => claim.Type == "编辑角色" && claim.Value == "编辑角色") || context.User.IsInRole("Super Admin");
        }
    }
}
