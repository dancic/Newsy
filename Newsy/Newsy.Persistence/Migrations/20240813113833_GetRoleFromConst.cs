using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Newsy.Persistence.Migrations
{
    public partial class GetRoleFromConst : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "96ea371f-3029-45a0-bd0e-e6c383d9975d");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "d0f68fd3-706c-4d04-bf10-d64f4f19352b", "daf2ac28-91a2-4dd4-b9ef-1329c49a3f53", "Author", "AUTHOR" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "d0f68fd3-706c-4d04-bf10-d64f4f19352b");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "96ea371f-3029-45a0-bd0e-e6c383d9975d", "2ba907f0-30b0-4969-a7de-5c1d2d705049", "Author", "AUTHOR" });
        }
    }
}
