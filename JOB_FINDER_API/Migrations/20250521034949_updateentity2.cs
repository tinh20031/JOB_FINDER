using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JOB_FINDER_API.Migrations
{
    /// <inheritdoc />
    public partial class updateentity2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 5, 21, 3, 49, 49, 414, DateTimeKind.Utc).AddTicks(322), new DateTime(2025, 5, 21, 3, 49, 49, 414, DateTimeKind.Utc).AddTicks(325) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 5, 21, 3, 49, 49, 414, DateTimeKind.Utc).AddTicks(331), new DateTime(2025, 5, 21, 3, 49, 49, 414, DateTimeKind.Utc).AddTicks(331) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 5, 21, 3, 49, 49, 414, DateTimeKind.Utc).AddTicks(332), new DateTime(2025, 5, 21, 3, 49, 49, 414, DateTimeKind.Utc).AddTicks(332) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 5, 21, 3, 28, 7, 931, DateTimeKind.Utc).AddTicks(339), new DateTime(2025, 5, 21, 3, 28, 7, 931, DateTimeKind.Utc).AddTicks(342) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 5, 21, 3, 28, 7, 931, DateTimeKind.Utc).AddTicks(345), new DateTime(2025, 5, 21, 3, 28, 7, 931, DateTimeKind.Utc).AddTicks(345) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 5, 21, 3, 28, 7, 931, DateTimeKind.Utc).AddTicks(346), new DateTime(2025, 5, 21, 3, 28, 7, 931, DateTimeKind.Utc).AddTicks(347) });
        }
    }
}
