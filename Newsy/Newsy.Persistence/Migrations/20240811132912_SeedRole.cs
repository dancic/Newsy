using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Newsy.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class SeedRole : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "e8c24ef1-c5cd-495e-9cd7-646c065c5b55", null, "Author", "AUTHOR" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "e8c24ef1-c5cd-495e-9cd7-646c065c5b55");
        }
    }
}
