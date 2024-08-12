using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Newsy.Persistence.Migrations
{
    public partial class ChangeType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "e8c24ef1-c5cd-495e-9cd7-646c065c5b55");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "LastPublishedDateTime",
                table: "Articles",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "96ea371f-3029-45a0-bd0e-e6c383d9975d", "2ba907f0-30b0-4969-a7de-5c1d2d705049", "Author", "AUTHOR" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "96ea371f-3029-45a0-bd0e-e6c383d9975d");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "LastPublishedDateTime",
                table: "Articles",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)),
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone",
                oldNullable: true);

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "e8c24ef1-c5cd-495e-9cd7-646c065c5b55", null, "Author", "AUTHOR" });
        }
    }
}
