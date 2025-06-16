using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JOB_FINDER_API.Migrations
{
    /// <inheritdoc />
    public partial class updateJobDB : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Education",
                table: "Jobs",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "Industries",
                keyColumn: "IndustryId",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 16, 9, 25, 30, 711, DateTimeKind.Utc).AddTicks(8709), new DateTime(2025, 6, 16, 9, 25, 30, 711, DateTimeKind.Utc).AddTicks(8713) });

            migrationBuilder.UpdateData(
                table: "Industries",
                keyColumn: "IndustryId",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 16, 9, 25, 30, 711, DateTimeKind.Utc).AddTicks(8715), new DateTime(2025, 6, 16, 9, 25, 30, 711, DateTimeKind.Utc).AddTicks(8716) });

            migrationBuilder.UpdateData(
                table: "JobTypes",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 16, 9, 25, 30, 711, DateTimeKind.Utc).AddTicks(8757), new DateTime(2025, 6, 16, 9, 25, 30, 711, DateTimeKind.Utc).AddTicks(8759) });

            migrationBuilder.UpdateData(
                table: "JobTypes",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 16, 9, 25, 30, 711, DateTimeKind.Utc).AddTicks(8762), new DateTime(2025, 6, 16, 9, 25, 30, 711, DateTimeKind.Utc).AddTicks(8762) });

            migrationBuilder.UpdateData(
                table: "JobTypes",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 16, 9, 25, 30, 711, DateTimeKind.Utc).AddTicks(8764), new DateTime(2025, 6, 16, 9, 25, 30, 711, DateTimeKind.Utc).AddTicks(8764) });

            migrationBuilder.UpdateData(
                table: "Levels",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 16, 9, 25, 30, 711, DateTimeKind.Utc).AddTicks(8798), new DateTime(2025, 6, 16, 9, 25, 30, 711, DateTimeKind.Utc).AddTicks(8798) });

            migrationBuilder.UpdateData(
                table: "Levels",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 16, 9, 25, 30, 711, DateTimeKind.Utc).AddTicks(8800), new DateTime(2025, 6, 16, 9, 25, 30, 711, DateTimeKind.Utc).AddTicks(8800) });

            migrationBuilder.UpdateData(
                table: "Levels",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 16, 9, 25, 30, 711, DateTimeKind.Utc).AddTicks(8802), new DateTime(2025, 6, 16, 9, 25, 30, 711, DateTimeKind.Utc).AddTicks(8802) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 16, 9, 25, 30, 711, DateTimeKind.Utc).AddTicks(8532), new DateTime(2025, 6, 16, 9, 25, 30, 711, DateTimeKind.Utc).AddTicks(8536) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 16, 9, 25, 30, 711, DateTimeKind.Utc).AddTicks(8541), new DateTime(2025, 6, 16, 9, 25, 30, 711, DateTimeKind.Utc).AddTicks(8541) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 16, 9, 25, 30, 711, DateTimeKind.Utc).AddTicks(8542), new DateTime(2025, 6, 16, 9, 25, 30, 711, DateTimeKind.Utc).AddTicks(8542) });

            migrationBuilder.UpdateData(
                table: "Skills",
                keyColumn: "SkillId",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 16, 9, 25, 30, 711, DateTimeKind.Utc).AddTicks(8842), new DateTime(2025, 6, 16, 9, 25, 30, 711, DateTimeKind.Utc).AddTicks(8844) });

            migrationBuilder.UpdateData(
                table: "Skills",
                keyColumn: "SkillId",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 16, 9, 25, 30, 711, DateTimeKind.Utc).AddTicks(8847), new DateTime(2025, 6, 16, 9, 25, 30, 711, DateTimeKind.Utc).AddTicks(8848) });

            migrationBuilder.UpdateData(
                table: "Skills",
                keyColumn: "SkillId",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 16, 9, 25, 30, 711, DateTimeKind.Utc).AddTicks(8849), new DateTime(2025, 6, 16, 9, 25, 30, 711, DateTimeKind.Utc).AddTicks(8850) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Education",
                table: "Jobs");

            migrationBuilder.UpdateData(
                table: "Industries",
                keyColumn: "IndustryId",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 16, 4, 30, 54, 790, DateTimeKind.Utc).AddTicks(4675), new DateTime(2025, 6, 16, 4, 30, 54, 790, DateTimeKind.Utc).AddTicks(4676) });

            migrationBuilder.UpdateData(
                table: "Industries",
                keyColumn: "IndustryId",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 16, 4, 30, 54, 790, DateTimeKind.Utc).AddTicks(4678), new DateTime(2025, 6, 16, 4, 30, 54, 790, DateTimeKind.Utc).AddTicks(4678) });

            migrationBuilder.UpdateData(
                table: "JobTypes",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 16, 4, 30, 54, 790, DateTimeKind.Utc).AddTicks(4717), new DateTime(2025, 6, 16, 4, 30, 54, 790, DateTimeKind.Utc).AddTicks(4718) });

            migrationBuilder.UpdateData(
                table: "JobTypes",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 16, 4, 30, 54, 790, DateTimeKind.Utc).AddTicks(4720), new DateTime(2025, 6, 16, 4, 30, 54, 790, DateTimeKind.Utc).AddTicks(4721) });

            migrationBuilder.UpdateData(
                table: "JobTypes",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 16, 4, 30, 54, 790, DateTimeKind.Utc).AddTicks(4752), new DateTime(2025, 6, 16, 4, 30, 54, 790, DateTimeKind.Utc).AddTicks(4752) });

            migrationBuilder.UpdateData(
                table: "Levels",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 16, 4, 30, 54, 790, DateTimeKind.Utc).AddTicks(4787), new DateTime(2025, 6, 16, 4, 30, 54, 790, DateTimeKind.Utc).AddTicks(4788) });

            migrationBuilder.UpdateData(
                table: "Levels",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 16, 4, 30, 54, 790, DateTimeKind.Utc).AddTicks(4790), new DateTime(2025, 6, 16, 4, 30, 54, 790, DateTimeKind.Utc).AddTicks(4790) });

            migrationBuilder.UpdateData(
                table: "Levels",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 16, 4, 30, 54, 790, DateTimeKind.Utc).AddTicks(4792), new DateTime(2025, 6, 16, 4, 30, 54, 790, DateTimeKind.Utc).AddTicks(4792) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 16, 4, 30, 54, 790, DateTimeKind.Utc).AddTicks(4511), new DateTime(2025, 6, 16, 4, 30, 54, 790, DateTimeKind.Utc).AddTicks(4513) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 16, 4, 30, 54, 790, DateTimeKind.Utc).AddTicks(4517), new DateTime(2025, 6, 16, 4, 30, 54, 790, DateTimeKind.Utc).AddTicks(4518) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 16, 4, 30, 54, 790, DateTimeKind.Utc).AddTicks(4519), new DateTime(2025, 6, 16, 4, 30, 54, 790, DateTimeKind.Utc).AddTicks(4519) });

            migrationBuilder.UpdateData(
                table: "Skills",
                keyColumn: "SkillId",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 16, 4, 30, 54, 790, DateTimeKind.Utc).AddTicks(4832), new DateTime(2025, 6, 16, 4, 30, 54, 790, DateTimeKind.Utc).AddTicks(4833) });

            migrationBuilder.UpdateData(
                table: "Skills",
                keyColumn: "SkillId",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 16, 4, 30, 54, 790, DateTimeKind.Utc).AddTicks(4836), new DateTime(2025, 6, 16, 4, 30, 54, 790, DateTimeKind.Utc).AddTicks(4836) });

            migrationBuilder.UpdateData(
                table: "Skills",
                keyColumn: "SkillId",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 16, 4, 30, 54, 790, DateTimeKind.Utc).AddTicks(4838), new DateTime(2025, 6, 16, 4, 30, 54, 790, DateTimeKind.Utc).AddTicks(4838) });
        }
    }
}
