using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using StudentManagement.CustomerMiddlewares;
using StudentManagement.Models;

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

            services.AddIdentity<IdentityUser, IdentityRole>()
                .AddErrorDescriber<CustomerErrorDescriber>()
                .AddEntityFrameworkStores<AppDbContext>();

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
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {

                //app.UseDeveloperExceptionPage();  //�����뷢���쳣ʱ������Ҫ��������Աʹ��

                app.UseExceptionHandler("/Error");  //ȫ�����ش����쳣

                //app.UseDeveloperExceptionPage();
                //app.UseStatusCodePagesWithRedirects("/Error/{0}");
                //�Ƽ�ʹ�����
                app.UseStatusCodePagesWithReExecute("/Error/{0}");  //���� 404 NotFound
            }
            else
            {
                app.UseStatusCodePagesWithRedirects("/Error/{0}");
            }

            //��Ӿ�̬�ļ��м��
            app.UseStaticFiles();

            //�����֤�м��
            app.UseAuthentication();

            //���·���м��
            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{Controller=Home}/{action=Index}/{id?}");
            });


        }
    }
}
