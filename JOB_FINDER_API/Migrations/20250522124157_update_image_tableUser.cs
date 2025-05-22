using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JOB_FINDER_API.Migrations
{
    /// <inheritdoc />
    public partial class update_image_tableUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Image",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 5, 22, 12, 41, 55, 704, DateTimeKind.Utc).AddTicks(4219), new DateTime(2025, 5, 22, 12, 41, 55, 704, DateTimeKind.Utc).AddTicks(4222) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 5, 22, 12, 41, 55, 704, DateTimeKind.Utc).AddTicks(4226), new DateTime(2025, 5, 22, 12, 41, 55, 704, DateTimeKind.Utc).AddTicks(4226) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 5, 22, 12, 41, 55, 704, DateTimeKind.Utc).AddTicks(4227), new DateTime(2025, 5, 22, 12, 41, 55, 704, DateTimeKind.Utc).AddTicks(4228) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Image",
                table: "Users");

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 5, 22, 10, 20, 6, 526, DateTimeKind.Utc).AddTicks(987), new DateTime(2025, 5, 22, 10, 20, 6, 526, DateTimeKind.Utc).AddTicks(991) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 5, 22, 10, 20, 6, 526, DateTimeKind.Utc).AddTicks(993), new DateTime(2025, 5, 22, 10, 20, 6, 526, DateTimeKind.Utc).AddTicks(994) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 5, 22, 10, 20, 6, 526, DateTimeKind.Utc).AddTicks(995), new DateTime(2025, 5, 22, 10, 20, 6, 526, DateTimeKind.Utc).AddTicks(995) });
        }
    }
}
