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

                app.UseDeveloperExceptionPage();  //�����뷢���쳣ʱ������Ҫ��������Աʹ��



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

            //��Ӿ�̬�ļ��м��
            app.UseStaticFiles();

            //�����֤�м��
            app.UseAuthentication();

            //���·���м��
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
