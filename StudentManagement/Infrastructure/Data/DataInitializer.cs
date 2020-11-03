using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using StudentManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StudentManagement.Infrastructure.Data
{
    public static class DataInitializer
    {
        public static IApplicationBuilder UseDataInitializer(this IApplicationBuilder builder)
        {
            using (var scope = builder.ApplicationServices.CreateScope())
            {
                var dbcontext = scope.ServiceProvider.GetService<AppDbContext>();
                var userMaager = scope.ServiceProvider.GetService<UserManager<ApplicationUser>>();
                var roleManager = scope.ServiceProvider.GetService<RoleManager<IdentityRole>>();


                #region 学生种子数据
                if (dbcontext.students.Any())
                {
                    return builder;
                }

                var students = new[] {
                    new Student{ Name="鲁小冲",ClassName=ClassName.ClassFour,Email="luchong1999@outlook.com",EnrollmentDate=DateTime.Parse("2017-09-02")},
                    new Student{ Name="刘小彤",ClassName=ClassName.ClassFour,Email="liuxiaotong@outlook.com",EnrollmentDate=DateTime.Parse("2017-09-02")},
                    new Student{ Name="张小三",ClassName=ClassName.ClassFive,Email="zhangxiaosan@outlook.com",EnrollmentDate=DateTime.Parse("2018-09-01")},
                    new Student{ Name="李小四",ClassName=ClassName.ClassSix,Email="lixiaosi@outlook.com",EnrollmentDate=DateTime.Parse("2019-09-02")},
                    new Student{ Name="王小五",ClassName=ClassName.ClassFive,Email="wangxiaowu@outlook.com",EnrollmentDate=DateTime.Parse("2020-09-01")}
                };

                foreach (var item in students)
                {
                    dbcontext.students.Add(item);
                }
                dbcontext.SaveChanges();
                #endregion

                #region 课程种子数据
                if (dbcontext.Course.Any())
                {
                    return builder;
                }

                var courses = new[] {
                    new Course{ CourseId=1050,Title="毛泽东思想概论",Credits=3},
                    new Course{ CourseId=4022,Title="微积分",Credits=3},
                    new Course{ CourseId=4041,Title="高等数学",Credits=3},
                    new Course{ CourseId=1045,Title="大学英语",Credits=4},
                    new Course{ CourseId=3141,Title="心理健康教育",Credits=4},
                    new Course{ CourseId=2021,Title="计算机科学",Credits=3},
                    new Course{ CourseId=2042,Title="C语言程序设计",Credits=4}
                };
                foreach (var item in courses)
                {
                    dbcontext.Course.Add(item);
                }
                dbcontext.SaveChanges();
                #endregion

                #region 学生课程关联种子数据
                var studentCourses = new[] {
                    new StudentCourse{ CourseId=1050,StudentId=6},
                    new StudentCourse{ CourseId=4022,StudentId=7},
                    new StudentCourse{ CourseId=3141,StudentId=9},
                    new StudentCourse{ CourseId=2021,StudentId=8},
                    new StudentCourse{ CourseId=2042,StudentId=10}
               };
                foreach (var item in studentCourses)
                {
                    dbcontext.StudentCourses.Add(item);
                }
                dbcontext.SaveChanges();
                #endregion

                #region 用户种子数据
                if (dbcontext.Users.Any())
                {
                    return builder;
                }

                var user = new ApplicationUser { Email = "2076155011@qq.com", UserName = "2076155011@qq.com", EmailConfirmed = true, City = "广东深圳" };
                userMaager.CreateAsync(user, "123456").Wait();   //等待异步方法执行完毕
                dbcontext.SaveChanges();

                var adminRole = "Admin";

                var role = new IdentityRole { Name = adminRole };

                dbcontext.Roles.Add(role);
                dbcontext.SaveChanges();

                dbcontext.UserRoles.Add(new IdentityUserRole<string>
                {
                    RoleId = role.Id,
                    UserId = user.Id
                });
                dbcontext.SaveChanges();
                #endregion
            }
            return builder;
        }
    }
}
