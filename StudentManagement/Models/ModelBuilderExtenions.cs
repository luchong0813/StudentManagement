using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StudentManagement.Models
{
    public static class ModelBuilderExtenions
    {
        public static void InsertSeedData(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Student>().HasData(
                new Student { Id = 1, Name = "张三", ClassName = ClassName.ClassFour, Email = "84512211@outlook.com" },
                new Student { Id = 2, Name = "李四", ClassName = ClassName.ClassSix, Email = "451515jshjd@outlook.com" },
                new Student { Id = 3, Name = "王五", ClassName = ClassName.ClassFour, Email = "sghdha52@qq.com" },
                new Student { Id = 4, Name = "赵六", ClassName = ClassName.ClassSix, Email = "45xshxdjn22@outlook.com" },
                new Student { Id = 5, Name = "鲁班", ClassName = ClassName.ClassFive, Email = "dxshjc1515251@163.com" });
        }
    }
}
