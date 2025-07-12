using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JOB_FINDER_API.Migrations
{
    /// <inheritdoc />
    public partial class reupdateDB : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Industries",
                keyColumn: "IndustryId",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 12, 10, 25, 16, 119, DateTimeKind.Utc).AddTicks(2539), new DateTime(2025, 7, 12, 10, 25, 16, 119, DateTimeKind.Utc).AddTicks(2540) });

            migrationBuilder.UpdateData(
                table: "Industries",
                keyColumn: "IndustryId",
                keyValue: 4,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 12, 10, 25, 16, 119, DateTimeKind.Utc).AddTicks(2543), new DateTime(2025, 7, 12, 10, 25, 16, 119, DateTimeKind.Utc).AddTicks(2544) });

            migrationBuilder.UpdateData(
                table: "Industries",
                keyColumn: "IndustryId",
                keyValue: 5,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 12, 10, 25, 16, 119, DateTimeKind.Utc).AddTicks(2545), new DateTime(2025, 7, 12, 10, 25, 16, 119, DateTimeKind.Utc).AddTicks(2545) });

            migrationBuilder.UpdateData(
                table: "Industries",
                keyColumn: "IndustryId",
                keyValue: 6,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 12, 10, 25, 16, 119, DateTimeKind.Utc).AddTicks(2547), new DateTime(2025, 7, 12, 10, 25, 16, 119, DateTimeKind.Utc).AddTicks(2547) });

            migrationBuilder.UpdateData(
                table: "Industries",
                keyColumn: "IndustryId",
                keyValue: 7,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 12, 10, 25, 16, 119, DateTimeKind.Utc).AddTicks(2549), new DateTime(2025, 7, 12, 10, 25, 16, 119, DateTimeKind.Utc).AddTicks(2549) });

            migrationBuilder.UpdateData(
                table: "Industries",
                keyColumn: "IndustryId",
                keyValue: 8,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 12, 10, 25, 16, 119, DateTimeKind.Utc).AddTicks(2551), new DateTime(2025, 7, 12, 10, 25, 16, 119, DateTimeKind.Utc).AddTicks(2551) });

            migrationBuilder.UpdateData(
                table: "Industries",
                keyColumn: "IndustryId",
                keyValue: 9,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 12, 10, 25, 16, 119, DateTimeKind.Utc).AddTicks(2552), new DateTime(2025, 7, 12, 10, 25, 16, 119, DateTimeKind.Utc).AddTicks(2552) });

            migrationBuilder.UpdateData(
                table: "JobTypes",
                keyColumn: "JobTypeId",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 12, 10, 25, 16, 119, DateTimeKind.Utc).AddTicks(2597), new DateTime(2025, 7, 12, 10, 25, 16, 119, DateTimeKind.Utc).AddTicks(2599) });

            migrationBuilder.UpdateData(
                table: "JobTypes",
                keyColumn: "JobTypeId",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 12, 10, 25, 16, 119, DateTimeKind.Utc).AddTicks(2601), new DateTime(2025, 7, 12, 10, 25, 16, 119, DateTimeKind.Utc).AddTicks(2601) });

            migrationBuilder.UpdateData(
                table: "JobTypes",
                keyColumn: "JobTypeId",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 12, 10, 25, 16, 119, DateTimeKind.Utc).AddTicks(2603), new DateTime(2025, 7, 12, 10, 25, 16, 119, DateTimeKind.Utc).AddTicks(2603) });

            migrationBuilder.UpdateData(
                table: "Levels",
                keyColumn: "LevelId",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 12, 10, 25, 16, 119, DateTimeKind.Utc).AddTicks(2639), new DateTime(2025, 7, 12, 10, 25, 16, 119, DateTimeKind.Utc).AddTicks(2640) });

            migrationBuilder.UpdateData(
                table: "Levels",
                keyColumn: "LevelId",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 12, 10, 25, 16, 119, DateTimeKind.Utc).AddTicks(2643), new DateTime(2025, 7, 12, 10, 25, 16, 119, DateTimeKind.Utc).AddTicks(2643) });

            migrationBuilder.UpdateData(
                table: "Levels",
                keyColumn: "LevelId",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 12, 10, 25, 16, 119, DateTimeKind.Utc).AddTicks(2645), new DateTime(2025, 7, 12, 10, 25, 16, 119, DateTimeKind.Utc).AddTicks(2646) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 12, 10, 25, 16, 119, DateTimeKind.Utc).AddTicks(2310), new DateTime(2025, 7, 12, 10, 25, 16, 119, DateTimeKind.Utc).AddTicks(2312) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 12, 10, 25, 16, 119, DateTimeKind.Utc).AddTicks(2317), new DateTime(2025, 7, 12, 10, 25, 16, 119, DateTimeKind.Utc).AddTicks(2318) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 12, 10, 25, 16, 119, DateTimeKind.Utc).AddTicks(2319), new DateTime(2025, 7, 12, 10, 25, 16, 119, DateTimeKind.Utc).AddTicks(2319) });
        }
    }
}
