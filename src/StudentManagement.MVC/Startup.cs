using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
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
using NetCore.AutoRegisterDi;
using StudentManagement.Application.Courses;
using StudentManagement.Application.Dtos;
using StudentManagement.Application.Students;
using StudentManagement.CustomerMiddlewares;
using StudentManagement.Infrastructure.Data;
using StudentManagement.Infrastructure.Repositories;
using StudentManagement.Models;
using StudentManagement.Security;
using StudentManagement.Security.CustomTokenProvider;

namespace StudentManagement
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
                .AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders()
                .AddTokenProvider<CustomEmailConfirmationTokenProvider<ApplicationUser>>("CustomEmailConfirmation");

            services.AddHttpContextAccessor();
            services.AddSingleton<IAuthorizationHandler, SuperAdminHander>();


            #region 策略声明
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

            #endregion

            #region 注册Swagger文档
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
                {
                    Title = "校园管理系统",
                    Version = "v1",
                    Description = "深入浅出ASP.NET Core学习实践",
                    Contact = new Microsoft.OpenApi.Models.OpenApiContact
                    {
                        Name = "傲慢与偏见",
                        Email = "luchong1999@outlook.com",
                        Url = new Uri("https://github.com/luchong0813")
                    }
                });
                if (_env.IsDevelopment())
                {
                    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

                    c.IncludeXmlComments(xmlPath);
                }
            });
            #endregion

            #region 授权验证
            services.AddAuthentication().AddMicrosoftAccount(microsoftOptions =>
            {
                microsoftOptions.ClientId = _configuration["Authentication:Microsoft:ClientId"];
                microsoftOptions.ClientSecret = _configuration["Authentication:Microsoft:ClientSecret"];
            }).AddGitHub(options =>
            {
                options.ClientId = _configuration["Authentication:GitHub:ClientId"];
                options.ClientSecret = _configuration["Authentication:GitHub:ClientSecret"];
            });

            //将所有令牌类型的有效期设置为30分钟
            services.Configure<DataProtectionTokenProviderOptions>(o =>
            {
                o.TokenLifespan = TimeSpan.FromMinutes(30);
            });

            //使用自定义令牌有效期将邮箱验证有效期设置为5分钟
            services.Configure<CustomEmailConfirmationTokenProviderOptions>(o =>
            {
                o.TokenLifespan = TimeSpan.FromMinutes(5);
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

                //启用邮箱验证
                options.SignIn.RequireConfirmedEmail = true;

                //通过自定义的CustomEmailconfirmationmin名称覆盖旧有token名称，使它与AddTokenProvider<CustomEmailConfirmationTokenProvider<ApplicationUser>>("CustomEmailConfirmation")关联在一起
                options.Tokens.EmailConfirmationTokenProvider = "CustomEmailConfirmation";

                //设置登录失败次数和时间
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
            });

            #endregion



            #region 依赖注入仓储到容器中
            var assembliesToScan = new[]
            {
                Assembly.GetExecutingAssembly(),
                //因为PageResultDto在Application类库中，所以通过PageResultDto获取程序集信息
                Assembly.GetAssembly(typeof(PageResultDto<>))
           };

            //自动注入服务到依赖注入容器
            services.RegisterAssemblyPublicNonGenericClasses(assembliesToScan)
                .Where(c => c.Name.EndsWith("Service"))
                .AsPublicImplementedInterfaces(ServiceLifetime.Scoped);


            services.AddTransient(typeof(IRepository<,>), typeof(RepositoryBase<,>));
            services.AddSingleton<DataProtectionPurposeStrings>();

            //services.AddScoped<IStudentRepository, StudentRepository>();
            //services.AddScoped<IStudentService, StudentService>();
            //services.AddScoped<ICourseService, CourseService>();

            #endregion

            services.AddControllersWithViews(config =>
            {
                var poicy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
                config.Filters.Add(new AuthorizeFilter(poicy));
            }).AddXmlDataContractSerializerFormatters();




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

            //运行时编译==>会降低性能，不建议在生产环境中使用
            var builder = services.AddControllersWithViews(config =>
             {
                 var policy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
                 config.Filters.Add(new AuthorizeFilter(policy));
             }).AddXmlSerializerFormatters();

            //如果市开发环境则启用运行时编译
            if (_env.IsDevelopment())
            {
                builder.AddRazorRuntimeCompilation();
            }


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
            //添加种子数据
            app.UseDataInitializer();

            //添加静态文件中间件
            app.UseStaticFiles();


            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "校园管理系统 API V1");
            });

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
