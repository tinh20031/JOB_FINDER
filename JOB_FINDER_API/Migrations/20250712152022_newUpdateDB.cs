using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JOB_FINDER_API.Migrations
{
    /// <inheritdoc />
    public partial class newUpdateDB : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Industries",
                keyColumn: "IndustryId",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 12, 15, 20, 20, 430, DateTimeKind.Utc).AddTicks(7596), new DateTime(2025, 7, 12, 15, 20, 20, 430, DateTimeKind.Utc).AddTicks(7597) });

            migrationBuilder.UpdateData(
                table: "Industries",
                keyColumn: "IndustryId",
                keyValue: 4,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 12, 15, 20, 20, 430, DateTimeKind.Utc).AddTicks(7599), new DateTime(2025, 7, 12, 15, 20, 20, 430, DateTimeKind.Utc).AddTicks(7599) });

            migrationBuilder.UpdateData(
                table: "Industries",
                keyColumn: "IndustryId",
                keyValue: 5,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 12, 15, 20, 20, 430, DateTimeKind.Utc).AddTicks(7600), new DateTime(2025, 7, 12, 15, 20, 20, 430, DateTimeKind.Utc).AddTicks(7601) });

            migrationBuilder.UpdateData(
                table: "Industries",
                keyColumn: "IndustryId",
                keyValue: 6,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 12, 15, 20, 20, 430, DateTimeKind.Utc).AddTicks(7602), new DateTime(2025, 7, 12, 15, 20, 20, 430, DateTimeKind.Utc).AddTicks(7602) });

            migrationBuilder.UpdateData(
                table: "Industries",
                keyColumn: "IndustryId",
                keyValue: 7,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 12, 15, 20, 20, 430, DateTimeKind.Utc).AddTicks(7604), new DateTime(2025, 7, 12, 15, 20, 20, 430, DateTimeKind.Utc).AddTicks(7604) });

            migrationBuilder.UpdateData(
                table: "Industries",
                keyColumn: "IndustryId",
                keyValue: 8,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 12, 15, 20, 20, 430, DateTimeKind.Utc).AddTicks(7606), new DateTime(2025, 7, 12, 15, 20, 20, 430, DateTimeKind.Utc).AddTicks(7606) });

            migrationBuilder.UpdateData(
                table: "Industries",
                keyColumn: "IndustryId",
                keyValue: 9,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 12, 15, 20, 20, 430, DateTimeKind.Utc).AddTicks(7607), new DateTime(2025, 7, 12, 15, 20, 20, 430, DateTimeKind.Utc).AddTicks(7608) });

            migrationBuilder.UpdateData(
                table: "JobTypes",
                keyColumn: "JobTypeId",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 12, 15, 20, 20, 430, DateTimeKind.Utc).AddTicks(7706), new DateTime(2025, 7, 12, 15, 20, 20, 430, DateTimeKind.Utc).AddTicks(7707) });

            migrationBuilder.UpdateData(
                table: "JobTypes",
                keyColumn: "JobTypeId",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 12, 15, 20, 20, 430, DateTimeKind.Utc).AddTicks(7709), new DateTime(2025, 7, 12, 15, 20, 20, 430, DateTimeKind.Utc).AddTicks(7709) });

            migrationBuilder.UpdateData(
                table: "JobTypes",
                keyColumn: "JobTypeId",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 12, 15, 20, 20, 430, DateTimeKind.Utc).AddTicks(7711), new DateTime(2025, 7, 12, 15, 20, 20, 430, DateTimeKind.Utc).AddTicks(7711) });

            migrationBuilder.UpdateData(
                table: "Levels",
                keyColumn: "LevelId",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 12, 15, 20, 20, 430, DateTimeKind.Utc).AddTicks(7747), new DateTime(2025, 7, 12, 15, 20, 20, 430, DateTimeKind.Utc).AddTicks(7747) });

            migrationBuilder.UpdateData(
                table: "Levels",
                keyColumn: "LevelId",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 12, 15, 20, 20, 430, DateTimeKind.Utc).AddTicks(7749), new DateTime(2025, 7, 12, 15, 20, 20, 430, DateTimeKind.Utc).AddTicks(7749) });

            migrationBuilder.UpdateData(
                table: "Levels",
                keyColumn: "LevelId",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 12, 15, 20, 20, 430, DateTimeKind.Utc).AddTicks(7751), new DateTime(2025, 7, 12, 15, 20, 20, 430, DateTimeKind.Utc).AddTicks(7751) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 12, 15, 20, 20, 430, DateTimeKind.Utc).AddTicks(7359), new DateTime(2025, 7, 12, 15, 20, 20, 430, DateTimeKind.Utc).AddTicks(7361) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 12, 15, 20, 20, 430, DateTimeKind.Utc).AddTicks(7366), new DateTime(2025, 7, 12, 15, 20, 20, 430, DateTimeKind.Utc).AddTicks(7366) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 12, 15, 20, 20, 430, DateTimeKind.Utc).AddTicks(7367), new DateTime(2025, 7, 12, 15, 20, 20, 430, DateTimeKind.Utc).AddTicks(7368) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Industries",
                keyColumn: "IndustryId",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 12, 10, 44, 40, 719, DateTimeKind.Utc).AddTicks(5516), new DateTime(2025, 7, 12, 10, 44, 40, 719, DateTimeKind.Utc).AddTicks(5516) });

            migrationBuilder.UpdateData(
                table: "Industries",
                keyColumn: "IndustryId",
                keyValue: 4,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 12, 10, 44, 40, 719, DateTimeKind.Utc).AddTicks(5519), new DateTime(2025, 7, 12, 10, 44, 40, 719, DateTimeKind.Utc).AddTicks(5519) });

            migrationBuilder.UpdateData(
                table: "Industries",
                keyColumn: "IndustryId",
                keyValue: 5,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 12, 10, 44, 40, 719, DateTimeKind.Utc).AddTicks(5521), new DateTime(2025, 7, 12, 10, 44, 40, 719, DateTimeKind.Utc).AddTicks(5521) });

            migrationBuilder.UpdateData(
                table: "Industries",
                keyColumn: "IndustryId",
                keyValue: 6,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 12, 10, 44, 40, 719, DateTimeKind.Utc).AddTicks(5523), new DateTime(2025, 7, 12, 10, 44, 40, 719, DateTimeKind.Utc).AddTicks(5523) });

            migrationBuilder.UpdateData(
                table: "Industries",
                keyColumn: "IndustryId",
                keyValue: 7,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 12, 10, 44, 40, 719, DateTimeKind.Utc).AddTicks(5524), new DateTime(2025, 7, 12, 10, 44, 40, 719, DateTimeKind.Utc).AddTicks(5525) });

            migrationBuilder.UpdateData(
                table: "Industries",
                keyColumn: "IndustryId",
                keyValue: 8,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 12, 10, 44, 40, 719, DateTimeKind.Utc).AddTicks(5526), new DateTime(2025, 7, 12, 10, 44, 40, 719, DateTimeKind.Utc).AddTicks(5526) });

            migrationBuilder.UpdateData(
                table: "Industries",
                keyColumn: "IndustryId",
                keyValue: 9,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 12, 10, 44, 40, 719, DateTimeKind.Utc).AddTicks(5527), new DateTime(2025, 7, 12, 10, 44, 40, 719, DateTimeKind.Utc).AddTicks(5528) });

            migrationBuilder.UpdateData(
                table: "JobTypes",
                keyColumn: "JobTypeId",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 12, 10, 44, 40, 719, DateTimeKind.Utc).AddTicks(5569), new DateTime(2025, 7, 12, 10, 44, 40, 719, DateTimeKind.Utc).AddTicks(5570) });

            migrationBuilder.UpdateData(
                table: "JobTypes",
                keyColumn: "JobTypeId",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 12, 10, 44, 40, 719, DateTimeKind.Utc).AddTicks(5573), new DateTime(2025, 7, 12, 10, 44, 40, 719, DateTimeKind.Utc).AddTicks(5573) });

            migrationBuilder.UpdateData(
                table: "JobTypes",
                keyColumn: "JobTypeId",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 12, 10, 44, 40, 719, DateTimeKind.Utc).AddTicks(5575), new DateTime(2025, 7, 12, 10, 44, 40, 719, DateTimeKind.Utc).AddTicks(5575) });

            migrationBuilder.UpdateData(
                table: "Levels",
                keyColumn: "LevelId",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 12, 10, 44, 40, 719, DateTimeKind.Utc).AddTicks(5606), new DateTime(2025, 7, 12, 10, 44, 40, 719, DateTimeKind.Utc).AddTicks(5607) });

            migrationBuilder.UpdateData(
                table: "Levels",
                keyColumn: "LevelId",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 12, 10, 44, 40, 719, DateTimeKind.Utc).AddTicks(5608), new DateTime(2025, 7, 12, 10, 44, 40, 719, DateTimeKind.Utc).AddTicks(5609) });

            migrationBuilder.UpdateData(
                table: "Levels",
                keyColumn: "LevelId",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 12, 10, 44, 40, 719, DateTimeKind.Utc).AddTicks(5610), new DateTime(2025, 7, 12, 10, 44, 40, 719, DateTimeKind.Utc).AddTicks(5611) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 12, 10, 44, 40, 719, DateTimeKind.Utc).AddTicks(5281), new DateTime(2025, 7, 12, 10, 44, 40, 719, DateTimeKind.Utc).AddTicks(5283) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 12, 10, 44, 40, 719, DateTimeKind.Utc).AddTicks(5289), new DateTime(2025, 7, 12, 10, 44, 40, 719, DateTimeKind.Utc).AddTicks(5289) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 12, 10, 44, 40, 719, DateTimeKind.Utc).AddTicks(5290), new DateTime(2025, 7, 12, 10, 44, 40, 719, DateTimeKind.Utc).AddTicks(5290) });
        }
    }
}
