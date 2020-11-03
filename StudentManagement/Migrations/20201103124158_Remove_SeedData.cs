using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace StudentManagement.Migrations
{
    public partial class Remove_SeedData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Student",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Student",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Student",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Student",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Student",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.RenameTable(
                name: "StudentCourse",
                newName: "StudentCourse",
                newSchema: "School");

            migrationBuilder.RenameTable(
                name: "Student",
                newName: "Student",
                newSchema: "School");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "StudentCourse",
                schema: "School",
                newName: "StudentCourse");

            migrationBuilder.RenameTable(
                name: "Student",
                schema: "School",
                newName: "Student");

            migrationBuilder.InsertData(
                table: "Student",
                columns: new[] { "Id", "ClassName", "Email", "EnrollmentDate", "Name", "Photo" },
                values: new object[,]
                {
                    { 1, 1, "84512211@outlook.com", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "张三", null },
                    { 2, 3, "451515jshjd@outlook.com", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "李四", null },
                    { 3, 1, "sghdha52@qq.com", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "王五", null },
                    { 4, 3, "45xshxdjn22@outlook.com", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "赵六", null },
                    { 5, 2, "dxshjc1515251@163.com", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "鲁班", null }
                });
        }
    }
}
