using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JOB_FINDER_API.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Industries",
                keyColumn: "IndustryId",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 24, 4, 28, 10, 48, DateTimeKind.Utc).AddTicks(8830), new DateTime(2025, 6, 24, 4, 28, 10, 48, DateTimeKind.Utc).AddTicks(8831) });

            migrationBuilder.UpdateData(
                table: "Industries",
                keyColumn: "IndustryId",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 24, 4, 28, 10, 48, DateTimeKind.Utc).AddTicks(8832), new DateTime(2025, 6, 24, 4, 28, 10, 48, DateTimeKind.Utc).AddTicks(8833) });

            migrationBuilder.UpdateData(
                table: "JobTypes",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 24, 4, 28, 10, 48, DateTimeKind.Utc).AddTicks(8865), new DateTime(2025, 6, 24, 4, 28, 10, 48, DateTimeKind.Utc).AddTicks(8869) });

            migrationBuilder.UpdateData(
                table: "JobTypes",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 24, 4, 28, 10, 48, DateTimeKind.Utc).AddTicks(8871), new DateTime(2025, 6, 24, 4, 28, 10, 48, DateTimeKind.Utc).AddTicks(8871) });

            migrationBuilder.UpdateData(
                table: "JobTypes",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 24, 4, 28, 10, 48, DateTimeKind.Utc).AddTicks(8873), new DateTime(2025, 6, 24, 4, 28, 10, 48, DateTimeKind.Utc).AddTicks(8873) });

            migrationBuilder.UpdateData(
                table: "Levels",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 24, 4, 28, 10, 48, DateTimeKind.Utc).AddTicks(8900), new DateTime(2025, 6, 24, 4, 28, 10, 48, DateTimeKind.Utc).AddTicks(8900) });

            migrationBuilder.UpdateData(
                table: "Levels",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 24, 4, 28, 10, 48, DateTimeKind.Utc).AddTicks(8902), new DateTime(2025, 6, 24, 4, 28, 10, 48, DateTimeKind.Utc).AddTicks(8902) });

            migrationBuilder.UpdateData(
                table: "Levels",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 24, 4, 28, 10, 48, DateTimeKind.Utc).AddTicks(8903), new DateTime(2025, 6, 24, 4, 28, 10, 48, DateTimeKind.Utc).AddTicks(8904) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 24, 4, 28, 10, 48, DateTimeKind.Utc).AddTicks(8658), new DateTime(2025, 6, 24, 4, 28, 10, 48, DateTimeKind.Utc).AddTicks(8661) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 24, 4, 28, 10, 48, DateTimeKind.Utc).AddTicks(8668), new DateTime(2025, 6, 24, 4, 28, 10, 48, DateTimeKind.Utc).AddTicks(8668) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 24, 4, 28, 10, 48, DateTimeKind.Utc).AddTicks(8669), new DateTime(2025, 6, 24, 4, 28, 10, 48, DateTimeKind.Utc).AddTicks(8669) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Industries",
                keyColumn: "IndustryId",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 23, 11, 41, 58, 772, DateTimeKind.Utc).AddTicks(8539), new DateTime(2025, 6, 23, 11, 41, 58, 772, DateTimeKind.Utc).AddTicks(8541) });

            migrationBuilder.UpdateData(
                table: "Industries",
                keyColumn: "IndustryId",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 23, 11, 41, 58, 772, DateTimeKind.Utc).AddTicks(8543), new DateTime(2025, 6, 23, 11, 41, 58, 772, DateTimeKind.Utc).AddTicks(8543) });

            migrationBuilder.UpdateData(
                table: "JobTypes",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 23, 11, 41, 58, 772, DateTimeKind.Utc).AddTicks(8626), new DateTime(2025, 6, 23, 11, 41, 58, 772, DateTimeKind.Utc).AddTicks(8628) });

            migrationBuilder.UpdateData(
                table: "JobTypes",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 23, 11, 41, 58, 772, DateTimeKind.Utc).AddTicks(8630), new DateTime(2025, 6, 23, 11, 41, 58, 772, DateTimeKind.Utc).AddTicks(8630) });

            migrationBuilder.UpdateData(
                table: "JobTypes",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 23, 11, 41, 58, 772, DateTimeKind.Utc).AddTicks(8632), new DateTime(2025, 6, 23, 11, 41, 58, 772, DateTimeKind.Utc).AddTicks(8632) });

            migrationBuilder.UpdateData(
                table: "Levels",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 23, 11, 41, 58, 772, DateTimeKind.Utc).AddTicks(8662), new DateTime(2025, 6, 23, 11, 41, 58, 772, DateTimeKind.Utc).AddTicks(8663) });

            migrationBuilder.UpdateData(
                table: "Levels",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 23, 11, 41, 58, 772, DateTimeKind.Utc).AddTicks(8665), new DateTime(2025, 6, 23, 11, 41, 58, 772, DateTimeKind.Utc).AddTicks(8665) });

            migrationBuilder.UpdateData(
                table: "Levels",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 23, 11, 41, 58, 772, DateTimeKind.Utc).AddTicks(8667), new DateTime(2025, 6, 23, 11, 41, 58, 772, DateTimeKind.Utc).AddTicks(8668) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 23, 11, 41, 58, 772, DateTimeKind.Utc).AddTicks(8381), new DateTime(2025, 6, 23, 11, 41, 58, 772, DateTimeKind.Utc).AddTicks(8383) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 23, 11, 41, 58, 772, DateTimeKind.Utc).AddTicks(8388), new DateTime(2025, 6, 23, 11, 41, 58, 772, DateTimeKind.Utc).AddTicks(8388) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 23, 11, 41, 58, 772, DateTimeKind.Utc).AddTicks(8389), new DateTime(2025, 6, 23, 11, 41, 58, 772, DateTimeKind.Utc).AddTicks(8389) });
        }
    }
}
