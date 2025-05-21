using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JOB_FINDER_API.Migrations
{
    /// <inheritdoc />
    public partial class init333 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 5, 21, 4, 13, 35, 539, DateTimeKind.Utc).AddTicks(6068), new DateTime(2025, 5, 21, 4, 13, 35, 539, DateTimeKind.Utc).AddTicks(6071) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 5, 21, 4, 13, 35, 539, DateTimeKind.Utc).AddTicks(6076), new DateTime(2025, 5, 21, 4, 13, 35, 539, DateTimeKind.Utc).AddTicks(6076) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 5, 21, 4, 13, 35, 539, DateTimeKind.Utc).AddTicks(6077), new DateTime(2025, 5, 21, 4, 13, 35, 539, DateTimeKind.Utc).AddTicks(6077) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 5, 21, 4, 12, 5, 567, DateTimeKind.Utc).AddTicks(9760), new DateTime(2025, 5, 21, 4, 12, 5, 567, DateTimeKind.Utc).AddTicks(9762) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 5, 21, 4, 12, 5, 567, DateTimeKind.Utc).AddTicks(9766), new DateTime(2025, 5, 21, 4, 12, 5, 567, DateTimeKind.Utc).AddTicks(9766) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 5, 21, 4, 12, 5, 567, DateTimeKind.Utc).AddTicks(9767), new DateTime(2025, 5, 21, 4, 12, 5, 567, DateTimeKind.Utc).AddTicks(9767) });
        }
    }
}
