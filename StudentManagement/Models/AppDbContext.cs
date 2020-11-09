using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StudentManagement.Models
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {

        }

        public DbSet<Student> students { get; set; }
        public DbSet<Course> Course { get; set; }
        public DbSet<StudentCourse> StudentCourses { get; set; }

        public DbSet<Department> Departments { get; set; }
        public DbSet<Teacher> Teachers { get; set; }
        public DbSet<OfficeLocation> OfficeLocations { get; set; }
        public DbSet<CourseAssignment> CourseAssignments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //modelBuilder.InsertSeedData();
            base.OnModelCreating(modelBuilder);
            modelBuilder.InsertSeedData();

            //指定实体在数据库中生成的名称
            //ToTable中，第二个参数指定表前缀名称
            modelBuilder.Entity<Course>().ToTable("Course", "School");
            modelBuilder.Entity<Student>().ToTable("Student");
            modelBuilder.Entity<StudentCourse>().ToTable("StudentCourse", "School");
            modelBuilder.Entity<CourseAssignment>().HasKey(c => new { c.CourseId, c.TeacherId });

            //获取当前系统中所有领域模型上的外键列表
            var foreignKeys = modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys());


            foreach (var item in foreignKeys)
            {
                //将它们的删除行为配置为无操作
                item.DeleteBehavior = DeleteBehavior.Restrict;
            }
        }
    }
}
