using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JOB_FINDER_API.Migrations
{
    /// <inheritdoc />
    public partial class newupdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 5, 25, 7, 57, 37, 847, DateTimeKind.Utc).AddTicks(4927), new DateTime(2025, 5, 25, 7, 57, 37, 847, DateTimeKind.Utc).AddTicks(4929) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 5, 25, 7, 57, 37, 847, DateTimeKind.Utc).AddTicks(4936), new DateTime(2025, 5, 25, 7, 57, 37, 847, DateTimeKind.Utc).AddTicks(4936) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 5, 25, 7, 57, 37, 847, DateTimeKind.Utc).AddTicks(4937), new DateTime(2025, 5, 25, 7, 57, 37, 847, DateTimeKind.Utc).AddTicks(4938) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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
    }
}
