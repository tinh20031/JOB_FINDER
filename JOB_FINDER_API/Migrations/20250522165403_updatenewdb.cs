using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JOB_FINDER_API.Migrations
{
    /// <inheritdoc />
    public partial class updatenewdb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 5, 22, 16, 54, 1, 998, DateTimeKind.Utc).AddTicks(6840), new DateTime(2025, 5, 22, 16, 54, 1, 998, DateTimeKind.Utc).AddTicks(6844) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 5, 22, 16, 54, 1, 998, DateTimeKind.Utc).AddTicks(6851), new DateTime(2025, 5, 22, 16, 54, 1, 998, DateTimeKind.Utc).AddTicks(6851) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 5, 22, 16, 54, 1, 998, DateTimeKind.Utc).AddTicks(6852), new DateTime(2025, 5, 22, 16, 54, 1, 998, DateTimeKind.Utc).AddTicks(6853) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 5, 21, 16, 2, 22, 124, DateTimeKind.Utc).AddTicks(172), new DateTime(2025, 5, 21, 16, 2, 22, 124, DateTimeKind.Utc).AddTicks(176) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 5, 21, 16, 2, 22, 124, DateTimeKind.Utc).AddTicks(179), new DateTime(2025, 5, 21, 16, 2, 22, 124, DateTimeKind.Utc).AddTicks(179) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 5, 21, 16, 2, 22, 124, DateTimeKind.Utc).AddTicks(180), new DateTime(2025, 5, 21, 16, 2, 22, 124, DateTimeKind.Utc).AddTicks(181) });
        }
    }
}
