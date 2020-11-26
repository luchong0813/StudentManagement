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


            #region ��������
            //���Խ��������Ȩ
            services.AddAuthorization(options =>
            {
                //ֻ������ɾ����ɫ�������˲��Բ��ܳɹ�
                options.AddPolicy("DeleteRolePolicy", policy => policy.RequireClaim("ɾ����ɫ"));
                options.AddPolicy("AdminRolePolicy", policy => policy.RequireRole("Admin"));

                //���Խ�϶����ɫ������Ȩ
                options.AddPolicy("SuperAdminPolicy", policy => policy.RequireRole("Admin", "User", "SuperManager"));

                //��������༭��ɫ������Ϊtrue���˲��Բ��ܳɹ�
                //options.AddPolicy("EditRolePolicy", policy =>
                //policy.RequireClaim("�༭��ɫ", "true", "yes")
                //.RequireRole("Admin"));


                options.AddPolicy("EditRolePolicy", policy => policy.RequireAssertion(context => AuthorizeAccess(context)));
            });

            #endregion

            #region ע��Swagger�ĵ�
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
                {
                    Title = "У԰����ϵͳ",
                    Version = "v1",
                    Description = "����ǳ��ASP.NET Coreѧϰʵ��",
                    Contact = new Microsoft.OpenApi.Models.OpenApiContact
                    {
                        Name = "������ƫ��",
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

            #region ��Ȩ��֤
            services.AddAuthentication().AddMicrosoftAccount(microsoftOptions =>
            {
                microsoftOptions.ClientId = _configuration["Authentication:Microsoft:ClientId"];
                microsoftOptions.ClientSecret = _configuration["Authentication:Microsoft:ClientSecret"];
            }).AddGitHub(options =>
            {
                options.ClientId = _configuration["Authentication:GitHub:ClientId"];
                options.ClientSecret = _configuration["Authentication:GitHub:ClientSecret"];
            });

            //�������������͵���Ч������Ϊ30����
            services.Configure<DataProtectionTokenProviderOptions>(o =>
            {
                o.TokenLifespan = TimeSpan.FromMinutes(30);
            });

            //ʹ���Զ���������Ч�ڽ�������֤��Ч������Ϊ5����
            services.Configure<CustomEmailConfirmationTokenProviderOptions>(o =>
            {
                o.TokenLifespan = TimeSpan.FromMinutes(5);
            });

            services.Configure<IdentityOptions>(options =>
            {
                //���������������ظ�����
                options.Password.RequiredUniqueChars = 3;
                //�������ٰ���һ������ĸ�����ֵ��ַ�
                options.Password.RequireNonAlphanumeric = false;
                //�����Ƿ�������Сд��ĸ
                options.Password.RequireLowercase = false;
                //�����Ƿ���������д��ĸ
                options.Password.RequireUppercase = false;

                //����������֤
                options.SignIn.RequireConfirmedEmail = true;

                //ͨ���Զ����CustomEmailconfirmationmin���Ƹ��Ǿ���token���ƣ�ʹ����AddTokenProvider<CustomEmailConfirmationTokenProvider<ApplicationUser>>("CustomEmailConfirmation")������һ��
                options.Tokens.EmailConfirmationTokenProvider = "CustomEmailConfirmation";

                //���õ�¼ʧ�ܴ�����ʱ��
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
            });

            #endregion



            #region ����ע��ִ���������
            var assembliesToScan = new[]
            {
                Assembly.GetExecutingAssembly(),
                //��ΪPageResultDto��Application����У�����ͨ��PageResultDto��ȡ������Ϣ
                Assembly.GetAssembly(typeof(PageResultDto<>))
           };

            //�Զ�ע���������ע������
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
                //�޸ľܾ����ʵ�·�ɵ�ַ
                options.AccessDeniedPath = new PathString("/Admin/AccessDenied");
                //ͳһϵͳ��ȫ��Cookie����
                options.Cookie.Name = "StudentMangementCookie";
                //��¼�û�Cookie����Ч��
                options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
                //�Ƿ��Cookie���û�������ʱ��
                options.SlidingExpiration = true;
            });

            //����ʱ����==>�ή�����ܣ�������������������ʹ��
            var builder = services.AddControllersWithViews(config =>
             {
                 var policy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
                 config.Filters.Add(new AuthorizeFilter(policy));
             }).AddXmlSerializerFormatters();

            //����п�����������������ʱ����
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

                app.UseDeveloperExceptionPage();  //�����뷢���쳣ʱ��������Ҫ��������Աʹ��



                //app.UseDeveloperExceptionPage();
                //app.UseStatusCodePagesWithRedirects("/Error/{0}");
                //�Ƽ�ʹ�����
                //app.UseStatusCodePagesWithReExecute("/Error/{0}");  //���� 404 NotFound
            }
            else
            {
                app.UseExceptionHandler("/Error");  //ȫ�����ش����쳣
                app.UseStatusCodePagesWithRedirects("/Error/{0}");
            }
            //������������
            app.UseDataInitializer();

            //���Ӿ�̬�ļ��м��
            app.UseStaticFiles();


            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "У԰����ϵͳ API V1");
            });

            //������֤�м��
            app.UseAuthentication();

            //����·���м��
            app.UseRouting();

            //��Ȩ�м��
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
            return context.User.IsInRole("Admin") && context.User.HasClaim(claim => claim.Type == "�༭��ɫ" && claim.Value == "�༭��ɫ") || context.User.IsInRole("Super Admin");
        }
    }
}