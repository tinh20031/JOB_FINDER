using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JOB_FINDER_API.Migrations
{
    /// <inheritdoc />
    public partial class updateDbMess : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Industries",
                keyColumn: "IndustryId",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 17, 12, 29, 37, 275, DateTimeKind.Utc).AddTicks(2140), new DateTime(2025, 6, 17, 12, 29, 37, 275, DateTimeKind.Utc).AddTicks(2141) });

            migrationBuilder.UpdateData(
                table: "Industries",
                keyColumn: "IndustryId",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 17, 12, 29, 37, 275, DateTimeKind.Utc).AddTicks(2144), new DateTime(2025, 6, 17, 12, 29, 37, 275, DateTimeKind.Utc).AddTicks(2144) });

            migrationBuilder.UpdateData(
                table: "JobTypes",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 17, 12, 29, 37, 275, DateTimeKind.Utc).AddTicks(2188), new DateTime(2025, 6, 17, 12, 29, 37, 275, DateTimeKind.Utc).AddTicks(2188) });

            migrationBuilder.UpdateData(
                table: "JobTypes",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 17, 12, 29, 37, 275, DateTimeKind.Utc).AddTicks(2191), new DateTime(2025, 6, 17, 12, 29, 37, 275, DateTimeKind.Utc).AddTicks(2191) });

            migrationBuilder.UpdateData(
                table: "JobTypes",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 17, 12, 29, 37, 275, DateTimeKind.Utc).AddTicks(2193), new DateTime(2025, 6, 17, 12, 29, 37, 275, DateTimeKind.Utc).AddTicks(2194) });

            migrationBuilder.UpdateData(
                table: "Levels",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 17, 12, 29, 37, 275, DateTimeKind.Utc).AddTicks(2228), new DateTime(2025, 6, 17, 12, 29, 37, 275, DateTimeKind.Utc).AddTicks(2228) });

            migrationBuilder.UpdateData(
                table: "Levels",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 17, 12, 29, 37, 275, DateTimeKind.Utc).AddTicks(2231), new DateTime(2025, 6, 17, 12, 29, 37, 275, DateTimeKind.Utc).AddTicks(2231) });

            migrationBuilder.UpdateData(
                table: "Levels",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 17, 12, 29, 37, 275, DateTimeKind.Utc).AddTicks(2233), new DateTime(2025, 6, 17, 12, 29, 37, 275, DateTimeKind.Utc).AddTicks(2233) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 17, 12, 29, 37, 275, DateTimeKind.Utc).AddTicks(1949), new DateTime(2025, 6, 17, 12, 29, 37, 275, DateTimeKind.Utc).AddTicks(1952) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 17, 12, 29, 37, 275, DateTimeKind.Utc).AddTicks(1957), new DateTime(2025, 6, 17, 12, 29, 37, 275, DateTimeKind.Utc).AddTicks(1957) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 17, 12, 29, 37, 275, DateTimeKind.Utc).AddTicks(1958), new DateTime(2025, 6, 17, 12, 29, 37, 275, DateTimeKind.Utc).AddTicks(1958) });

            migrationBuilder.UpdateData(
                table: "Skills",
                keyColumn: "SkillId",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 17, 12, 29, 37, 275, DateTimeKind.Utc).AddTicks(2276), new DateTime(2025, 6, 17, 12, 29, 37, 275, DateTimeKind.Utc).AddTicks(2277) });

            migrationBuilder.UpdateData(
                table: "Skills",
                keyColumn: "SkillId",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 17, 12, 29, 37, 275, DateTimeKind.Utc).AddTicks(2279), new DateTime(2025, 6, 17, 12, 29, 37, 275, DateTimeKind.Utc).AddTicks(2279) });

            migrationBuilder.UpdateData(
                table: "Skills",
                keyColumn: "SkillId",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 17, 12, 29, 37, 275, DateTimeKind.Utc).AddTicks(2281), new DateTime(2025, 6, 17, 12, 29, 37, 275, DateTimeKind.Utc).AddTicks(2281) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Industries",
                keyColumn: "IndustryId",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 17, 7, 42, 30, 305, DateTimeKind.Utc).AddTicks(3199), new DateTime(2025, 6, 17, 7, 42, 30, 305, DateTimeKind.Utc).AddTicks(3200) });

            migrationBuilder.UpdateData(
                table: "Industries",
                keyColumn: "IndustryId",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 17, 7, 42, 30, 305, DateTimeKind.Utc).AddTicks(3202), new DateTime(2025, 6, 17, 7, 42, 30, 305, DateTimeKind.Utc).AddTicks(3202) });

            migrationBuilder.UpdateData(
                table: "JobTypes",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 17, 7, 42, 30, 305, DateTimeKind.Utc).AddTicks(3277), new DateTime(2025, 6, 17, 7, 42, 30, 305, DateTimeKind.Utc).AddTicks(3278) });

            migrationBuilder.UpdateData(
                table: "JobTypes",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 17, 7, 42, 30, 305, DateTimeKind.Utc).AddTicks(3279), new DateTime(2025, 6, 17, 7, 42, 30, 305, DateTimeKind.Utc).AddTicks(3280) });

            migrationBuilder.UpdateData(
                table: "JobTypes",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 17, 7, 42, 30, 305, DateTimeKind.Utc).AddTicks(3282), new DateTime(2025, 6, 17, 7, 42, 30, 305, DateTimeKind.Utc).AddTicks(3282) });

            migrationBuilder.UpdateData(
                table: "Levels",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 17, 7, 42, 30, 305, DateTimeKind.Utc).AddTicks(3306), new DateTime(2025, 6, 17, 7, 42, 30, 305, DateTimeKind.Utc).AddTicks(3307) });

            migrationBuilder.UpdateData(
                table: "Levels",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 17, 7, 42, 30, 305, DateTimeKind.Utc).AddTicks(3309), new DateTime(2025, 6, 17, 7, 42, 30, 305, DateTimeKind.Utc).AddTicks(3309) });

            migrationBuilder.UpdateData(
                table: "Levels",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 17, 7, 42, 30, 305, DateTimeKind.Utc).AddTicks(3310), new DateTime(2025, 6, 17, 7, 42, 30, 305, DateTimeKind.Utc).AddTicks(3311) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 17, 7, 42, 30, 305, DateTimeKind.Utc).AddTicks(3027), new DateTime(2025, 6, 17, 7, 42, 30, 305, DateTimeKind.Utc).AddTicks(3030) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 17, 7, 42, 30, 305, DateTimeKind.Utc).AddTicks(3035), new DateTime(2025, 6, 17, 7, 42, 30, 305, DateTimeKind.Utc).AddTicks(3035) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 17, 7, 42, 30, 305, DateTimeKind.Utc).AddTicks(3036), new DateTime(2025, 6, 17, 7, 42, 30, 305, DateTimeKind.Utc).AddTicks(3036) });

            migrationBuilder.UpdateData(
                table: "Skills",
                keyColumn: "SkillId",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 17, 7, 42, 30, 305, DateTimeKind.Utc).AddTicks(3340), new DateTime(2025, 6, 17, 7, 42, 30, 305, DateTimeKind.Utc).AddTicks(3340) });

            migrationBuilder.UpdateData(
                table: "Skills",
                keyColumn: "SkillId",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 17, 7, 42, 30, 305, DateTimeKind.Utc).AddTicks(3342), new DateTime(2025, 6, 17, 7, 42, 30, 305, DateTimeKind.Utc).AddTicks(3342) });

            migrationBuilder.UpdateData(
                table: "Skills",
                keyColumn: "SkillId",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 17, 7, 42, 30, 305, DateTimeKind.Utc).AddTicks(3344), new DateTime(2025, 6, 17, 7, 42, 30, 305, DateTimeKind.Utc).AddTicks(3344) });
        }
    }
}
