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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //modelBuilder.InsertSeedData();
            base.OnModelCreating(modelBuilder);
            modelBuilder.InsertSeedData();

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
