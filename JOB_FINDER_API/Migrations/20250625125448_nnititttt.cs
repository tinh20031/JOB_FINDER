using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JOB_FINDER_API.Migrations
{
    /// <inheritdoc />
    public partial class nnititttt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Industries",
                keyColumn: "IndustryId",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 25, 12, 54, 47, 677, DateTimeKind.Utc).AddTicks(3733), new DateTime(2025, 6, 25, 12, 54, 47, 677, DateTimeKind.Utc).AddTicks(3734) });

            migrationBuilder.UpdateData(
                table: "Industries",
                keyColumn: "IndustryId",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 25, 12, 54, 47, 677, DateTimeKind.Utc).AddTicks(3736), new DateTime(2025, 6, 25, 12, 54, 47, 677, DateTimeKind.Utc).AddTicks(3737) });

            migrationBuilder.UpdateData(
                table: "JobTypes",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 25, 12, 54, 47, 677, DateTimeKind.Utc).AddTicks(3763), new DateTime(2025, 6, 25, 12, 54, 47, 677, DateTimeKind.Utc).AddTicks(3765) });

            migrationBuilder.UpdateData(
                table: "JobTypes",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 25, 12, 54, 47, 677, DateTimeKind.Utc).AddTicks(3768), new DateTime(2025, 6, 25, 12, 54, 47, 677, DateTimeKind.Utc).AddTicks(3768) });

            migrationBuilder.UpdateData(
                table: "JobTypes",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 25, 12, 54, 47, 677, DateTimeKind.Utc).AddTicks(3770), new DateTime(2025, 6, 25, 12, 54, 47, 677, DateTimeKind.Utc).AddTicks(3770) });

            migrationBuilder.UpdateData(
                table: "Levels",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 25, 12, 54, 47, 677, DateTimeKind.Utc).AddTicks(3801), new DateTime(2025, 6, 25, 12, 54, 47, 677, DateTimeKind.Utc).AddTicks(3802) });

            migrationBuilder.UpdateData(
                table: "Levels",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 25, 12, 54, 47, 677, DateTimeKind.Utc).AddTicks(3804), new DateTime(2025, 6, 25, 12, 54, 47, 677, DateTimeKind.Utc).AddTicks(3804) });

            migrationBuilder.UpdateData(
                table: "Levels",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 25, 12, 54, 47, 677, DateTimeKind.Utc).AddTicks(3839), new DateTime(2025, 6, 25, 12, 54, 47, 677, DateTimeKind.Utc).AddTicks(3840) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 25, 12, 54, 47, 677, DateTimeKind.Utc).AddTicks(3576), new DateTime(2025, 6, 25, 12, 54, 47, 677, DateTimeKind.Utc).AddTicks(3577) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 25, 12, 54, 47, 677, DateTimeKind.Utc).AddTicks(3581), new DateTime(2025, 6, 25, 12, 54, 47, 677, DateTimeKind.Utc).AddTicks(3582) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 25, 12, 54, 47, 677, DateTimeKind.Utc).AddTicks(3582), new DateTime(2025, 6, 25, 12, 54, 47, 677, DateTimeKind.Utc).AddTicks(3583) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Industries",
                keyColumn: "IndustryId",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 25, 12, 53, 10, 866, DateTimeKind.Utc).AddTicks(4997), new DateTime(2025, 6, 25, 12, 53, 10, 866, DateTimeKind.Utc).AddTicks(4997) });

            migrationBuilder.UpdateData(
                table: "Industries",
                keyColumn: "IndustryId",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 25, 12, 53, 10, 866, DateTimeKind.Utc).AddTicks(4999), new DateTime(2025, 6, 25, 12, 53, 10, 866, DateTimeKind.Utc).AddTicks(5000) });

            migrationBuilder.UpdateData(
                table: "JobTypes",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 25, 12, 53, 10, 866, DateTimeKind.Utc).AddTicks(5033), new DateTime(2025, 6, 25, 12, 53, 10, 866, DateTimeKind.Utc).AddTicks(5034) });

            migrationBuilder.UpdateData(
                table: "JobTypes",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 25, 12, 53, 10, 866, DateTimeKind.Utc).AddTicks(5035), new DateTime(2025, 6, 25, 12, 53, 10, 866, DateTimeKind.Utc).AddTicks(5036) });

            migrationBuilder.UpdateData(
                table: "JobTypes",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 25, 12, 53, 10, 866, DateTimeKind.Utc).AddTicks(5038), new DateTime(2025, 6, 25, 12, 53, 10, 866, DateTimeKind.Utc).AddTicks(5038) });

            migrationBuilder.UpdateData(
                table: "Levels",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 25, 12, 53, 10, 866, DateTimeKind.Utc).AddTicks(5101), new DateTime(2025, 6, 25, 12, 53, 10, 866, DateTimeKind.Utc).AddTicks(5101) });

            migrationBuilder.UpdateData(
                table: "Levels",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 25, 12, 53, 10, 866, DateTimeKind.Utc).AddTicks(5103), new DateTime(2025, 6, 25, 12, 53, 10, 866, DateTimeKind.Utc).AddTicks(5103) });

            migrationBuilder.UpdateData(
                table: "Levels",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 25, 12, 53, 10, 866, DateTimeKind.Utc).AddTicks(5105), new DateTime(2025, 6, 25, 12, 53, 10, 866, DateTimeKind.Utc).AddTicks(5105) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 25, 12, 53, 10, 866, DateTimeKind.Utc).AddTicks(4822), new DateTime(2025, 6, 25, 12, 53, 10, 866, DateTimeKind.Utc).AddTicks(4825) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 25, 12, 53, 10, 866, DateTimeKind.Utc).AddTicks(4830), new DateTime(2025, 6, 25, 12, 53, 10, 866, DateTimeKind.Utc).AddTicks(4830) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 25, 12, 53, 10, 866, DateTimeKind.Utc).AddTicks(4831), new DateTime(2025, 6, 25, 12, 53, 10, 866, DateTimeKind.Utc).AddTicks(4831) });
        }
    }
}
