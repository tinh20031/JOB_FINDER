using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JOB_FINDER_API.Migrations
{
    /// <inheritdoc />
    public partial class intntt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 5, 25, 14, 9, 32, 285, DateTimeKind.Utc).AddTicks(8708), new DateTime(2025, 5, 25, 14, 9, 32, 285, DateTimeKind.Utc).AddTicks(8710) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 5, 25, 14, 9, 32, 285, DateTimeKind.Utc).AddTicks(8713), new DateTime(2025, 5, 25, 14, 9, 32, 285, DateTimeKind.Utc).AddTicks(8713) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 5, 25, 14, 9, 32, 285, DateTimeKind.Utc).AddTicks(8743), new DateTime(2025, 5, 25, 14, 9, 32, 285, DateTimeKind.Utc).AddTicks(8744) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 5, 25, 8, 1, 52, 218, DateTimeKind.Utc).AddTicks(1275), new DateTime(2025, 5, 25, 8, 1, 52, 218, DateTimeKind.Utc).AddTicks(1278) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 5, 25, 8, 1, 52, 218, DateTimeKind.Utc).AddTicks(1281), new DateTime(2025, 5, 25, 8, 1, 52, 218, DateTimeKind.Utc).AddTicks(1282) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 5, 25, 8, 1, 52, 218, DateTimeKind.Utc).AddTicks(1283), new DateTime(2025, 5, 25, 8, 1, 52, 218, DateTimeKind.Utc).AddTicks(1283) });
        }
    }
}
