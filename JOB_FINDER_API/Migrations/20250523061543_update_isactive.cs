using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JOB_FINDER_API.Migrations
{
    /// <inheritdoc />
    public partial class update_isactive : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "CompanyProfile",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 5, 23, 6, 15, 40, 501, DateTimeKind.Utc).AddTicks(2299), new DateTime(2025, 5, 23, 6, 15, 40, 501, DateTimeKind.Utc).AddTicks(2303) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 5, 23, 6, 15, 40, 501, DateTimeKind.Utc).AddTicks(2308), new DateTime(2025, 5, 23, 6, 15, 40, 501, DateTimeKind.Utc).AddTicks(2308) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 5, 23, 6, 15, 40, 501, DateTimeKind.Utc).AddTicks(2309), new DateTime(2025, 5, 23, 6, 15, 40, 501, DateTimeKind.Utc).AddTicks(2310) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "CompanyProfile");

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 5, 22, 16, 42, 40, 443, DateTimeKind.Utc).AddTicks(3517), new DateTime(2025, 5, 22, 16, 42, 40, 443, DateTimeKind.Utc).AddTicks(3522) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 5, 22, 16, 42, 40, 443, DateTimeKind.Utc).AddTicks(3526), new DateTime(2025, 5, 22, 16, 42, 40, 443, DateTimeKind.Utc).AddTicks(3526) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 5, 22, 16, 42, 40, 443, DateTimeKind.Utc).AddTicks(3527), new DateTime(2025, 5, 22, 16, 42, 40, 443, DateTimeKind.Utc).AddTicks(3527) });
        }
    }
}
