using Microsoft.EntityFrameworkCore.Migrations;

namespace StudentManagement.Migrations
{
    public partial class InsertSeedData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "students",
                columns: new[] { "Id", "ClassName", "Email", "Name" },
                values: new object[,]
                {
                    { 1, 1, "84512211@outlook.com", "张三" },
                    { 2, 3, "451515jshjd@outlook.com", "李四" },
                    { 3, 1, "sghdha52@qq.com", "王五" },
                    { 4, 3, "45xshxdjn22@outlook.com", "赵六" },
                    { 5, 2, "dxshjc1515251@163.com", "鲁班" }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "students",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "students",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "students",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "students",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "students",
                keyColumn: "Id",
                keyValue: 5);
        }
    }
}
