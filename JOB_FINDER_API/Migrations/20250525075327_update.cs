using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JOB_FINDER_API.Migrations
{
    /// <inheritdoc />
    public partial class update : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 5, 25, 7, 53, 27, 88, DateTimeKind.Utc).AddTicks(1532), new DateTime(2025, 5, 25, 7, 53, 27, 88, DateTimeKind.Utc).AddTicks(1533) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 5, 25, 7, 53, 27, 88, DateTimeKind.Utc).AddTicks(1538), new DateTime(2025, 5, 25, 7, 53, 27, 88, DateTimeKind.Utc).AddTicks(1538) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 5, 25, 7, 53, 27, 88, DateTimeKind.Utc).AddTicks(1539), new DateTime(2025, 5, 25, 7, 53, 27, 88, DateTimeKind.Utc).AddTicks(1540) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 5, 25, 7, 48, 56, 705, DateTimeKind.Utc).AddTicks(6777), new DateTime(2025, 5, 25, 7, 48, 56, 705, DateTimeKind.Utc).AddTicks(6779) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 5, 25, 7, 48, 56, 705, DateTimeKind.Utc).AddTicks(6783), new DateTime(2025, 5, 25, 7, 48, 56, 705, DateTimeKind.Utc).AddTicks(6783) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 5, 25, 7, 48, 56, 705, DateTimeKind.Utc).AddTicks(6784), new DateTime(2025, 5, 25, 7, 48, 56, 705, DateTimeKind.Utc).AddTicks(6784) });
        }
    }
}
