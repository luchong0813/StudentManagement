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

                #region 教师种子数据
                var teachers = new[] {
                    new Teacher{Name="张老师",HireDate=DateTime.Parse("1995-03-11")},
                    new Teacher{Name="王老师",HireDate=DateTime.Parse("2005-02-28")},
                    new Teacher{Name="李老师",HireDate=DateTime.Parse("1985-09-26")},
                    new Teacher{Name="刘老师",HireDate=DateTime.Parse("2011-03-13")},
                    new Teacher{Name="赵老师",HireDate=DateTime.Parse("2017-04-13")},
                    new Teacher{Name="胡老师",HireDate=DateTime.Parse("2019-05-23")}
                };
                foreach (var item in teachers)
                {
                    dbcontext.Teachers.Add(item);
                    dbcontext.SaveChanges();
                }
                #endregion

                #region 学院种子数据
                var departments = new[] {
                    new Department{Name="信息传媒学院",Budget=35000,StartDate=DateTime.Parse("2017-09-01"),TeacherId=teachers.Single(t=>t.Name=="刘老师").Id},
                     new Department{Name="生物工程学院",Budget=12000,StartDate=DateTime.Parse("2018-09-01"),TeacherId=teachers.Single(t=>t.Name=="赵老师").Id},
                      new Department{Name="经济管理学院",Budget=8000,StartDate=DateTime.Parse("2019-09-01"),TeacherId=teachers.Single(t=>t.Name=="胡老师").Id},
                       new Department{Name="土木工程学院",Budget=18000,StartDate=DateTime.Parse("2019-09-01"),TeacherId=teachers.Single(t=>t.Name=="王老师").Id}
                };
                foreach (var item in departments)
                    dbcontext.Departments.Add(item);
                dbcontext.SaveChanges();

                #endregion

                #region 课程种子数据
                if (dbcontext.Course.Any())
                {
                    return builder;
                }

                var courses = new[] {
                    new Course{ CourseId=1050,Title="毛泽东思想概论",Credits=3,DepartmentId=departments.Single(d=>d.Name=="信息传媒学院").DepartmentId},
                    new Course{ CourseId=4022,Title="微积分",Credits=3,DepartmentId=departments.Single(d=>d.Name=="生物工程学院").DepartmentId},
                    new Course{ CourseId=1045,Title="大学英语",Credits=4,DepartmentId=departments.Single(d=>d.Name=="土木工程学院").DepartmentId},
                    new Course{ CourseId=3141,Title="心理健康教育",Credits=4,DepartmentId=departments.Single(d=>d.Name=="生物工程学院").DepartmentId},
                };
                foreach (var item in courses)
                {
                    dbcontext.Course.Add(item);
                }
                dbcontext.SaveChanges();
                #endregion

                #region 办公室分配的种子数据
                var officeLocations = new[]
                {
                    new OfficeLocation{TeacherId=teachers.Single(t=>t.Name=="刘老师").Id,Location="F2203"},
                    new OfficeLocation{TeacherId=teachers.Single(t=>t.Name=="王老师").Id,Location="F3301"},
                    new OfficeLocation{TeacherId=teachers.Single(t=>t.Name=="胡老师").Id,Location="F5902"},
                    new OfficeLocation{TeacherId=teachers.Single(t=>t.Name=="赵老师").Id,Location="F3304"},
                    new OfficeLocation{TeacherId=teachers.Single(t=>t.Name=="李老师").Id,Location="F1101"},
                };
                foreach (var item in officeLocations)
                {
                    dbcontext.OfficeLocations.Add(item);
                    dbcontext.SaveChanges();
                }
                #endregion

                #region 为教师分配课程的种子数据
                var courseTeachers = new[]
                {
                    new CourseAssignment{CourseId=courses.Single(c=>c.Title=="毛泽东思想概论").CourseId,TeacherId=teachers.Single(t=>t.Name=="王老师").Id},
                     new CourseAssignment{CourseId=courses.Single(c=>c.Title=="微积分").CourseId,TeacherId=teachers.Single(t=>t.Name=="张老师").Id},
   
                       new CourseAssignment{CourseId=courses.Single(c=>c.Title=="大学英语").CourseId,TeacherId=teachers.Single(t=>t.Name=="赵老师").Id},
                          new CourseAssignment{CourseId=courses.Single(c=>c.Title=="心理健康教育").CourseId,TeacherId=teachers.Single(t=>t.Name=="胡老师").Id}
                };
                foreach (var item in courseTeachers)
                {
                    dbcontext.CourseAssignments.Add(item);
                    dbcontext.SaveChanges();
                }

                #endregion

                #region 学生课程关联种子数据

                var studentCourses = new[] {
                    new StudentCourse{StudentId=students.Single(s=>s.Name=="鲁小冲").Id,CourseId=courses.Single(c=>c.Title=="毛泽东思想概论").CourseId ,Grade=Grade.A},
                     new StudentCourse{StudentId=students.Single(s=>s.Name=="刘小彤").Id,CourseId=courses.Single(c=>c.Title=="大学英语").CourseId ,Grade=Grade.B},
                      new StudentCourse{StudentId=students.Single(s=>s.Name=="张小三").Id,CourseId=courses.Single(c=>c.Title=="心理健康教育").CourseId ,Grade=Grade.C},
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
