﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using StudentManagement.Models;

namespace StudentManagement.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20200914134536_InsertSeedData")]
    partial class InsertSeedData
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.8")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("StudentManagement.Models.Student", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("ClassName")
                        .HasColumnType("int");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(10)")
                        .HasMaxLength(10);

                    b.HasKey("Id");

                    b.ToTable("students");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            ClassName = 1,
                            Email = "84512211@outlook.com",
                            Name = "张三"
                        },
                        new
                        {
                            Id = 2,
                            ClassName = 3,
                            Email = "451515jshjd@outlook.com",
                            Name = "李四"
                        },
                        new
                        {
                            Id = 3,
                            ClassName = 1,
                            Email = "sghdha52@qq.com",
                            Name = "王五"
                        },
                        new
                        {
                            Id = 4,
                            ClassName = 3,
                            Email = "45xshxdjn22@outlook.com",
                            Name = "赵六"
                        },
                        new
                        {
                            Id = 5,
                            ClassName = 2,
                            Email = "dxshjc1515251@163.com",
                            Name = "鲁班"
                        });
                });
#pragma warning restore 612, 618
        }
    }
}