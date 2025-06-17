using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JOB_FINDER_API.Migrations
{
    /// <inheritdoc />
    public partial class updateJob : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "YourSkillAndExperience",
                table: "Jobs",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "Industries",
                keyColumn: "IndustryId",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 17, 8, 52, 38, 690, DateTimeKind.Utc).AddTicks(4742), new DateTime(2025, 6, 17, 8, 52, 38, 690, DateTimeKind.Utc).AddTicks(4743) });

            migrationBuilder.UpdateData(
                table: "Industries",
                keyColumn: "IndustryId",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 17, 8, 52, 38, 690, DateTimeKind.Utc).AddTicks(4745), new DateTime(2025, 6, 17, 8, 52, 38, 690, DateTimeKind.Utc).AddTicks(4745) });

            migrationBuilder.UpdateData(
                table: "JobTypes",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 17, 8, 52, 38, 690, DateTimeKind.Utc).AddTicks(4780), new DateTime(2025, 6, 17, 8, 52, 38, 690, DateTimeKind.Utc).AddTicks(4780) });

            migrationBuilder.UpdateData(
                table: "JobTypes",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 17, 8, 52, 38, 690, DateTimeKind.Utc).AddTicks(4782), new DateTime(2025, 6, 17, 8, 52, 38, 690, DateTimeKind.Utc).AddTicks(4782) });

            migrationBuilder.UpdateData(
                table: "JobTypes",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 17, 8, 52, 38, 690, DateTimeKind.Utc).AddTicks(4784), new DateTime(2025, 6, 17, 8, 52, 38, 690, DateTimeKind.Utc).AddTicks(4784) });

            migrationBuilder.UpdateData(
                table: "Levels",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 17, 8, 52, 38, 690, DateTimeKind.Utc).AddTicks(4848), new DateTime(2025, 6, 17, 8, 52, 38, 690, DateTimeKind.Utc).AddTicks(4848) });

            migrationBuilder.UpdateData(
                table: "Levels",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 17, 8, 52, 38, 690, DateTimeKind.Utc).AddTicks(4850), new DateTime(2025, 6, 17, 8, 52, 38, 690, DateTimeKind.Utc).AddTicks(4851) });

            migrationBuilder.UpdateData(
                table: "Levels",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 17, 8, 52, 38, 690, DateTimeKind.Utc).AddTicks(4852), new DateTime(2025, 6, 17, 8, 52, 38, 690, DateTimeKind.Utc).AddTicks(4853) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 17, 8, 52, 38, 690, DateTimeKind.Utc).AddTicks(4574), new DateTime(2025, 6, 17, 8, 52, 38, 690, DateTimeKind.Utc).AddTicks(4577) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 17, 8, 52, 38, 690, DateTimeKind.Utc).AddTicks(4581), new DateTime(2025, 6, 17, 8, 52, 38, 690, DateTimeKind.Utc).AddTicks(4581) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 17, 8, 52, 38, 690, DateTimeKind.Utc).AddTicks(4582), new DateTime(2025, 6, 17, 8, 52, 38, 690, DateTimeKind.Utc).AddTicks(4582) });

            migrationBuilder.UpdateData(
                table: "Skills",
                keyColumn: "SkillId",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 17, 8, 52, 38, 690, DateTimeKind.Utc).AddTicks(4885), new DateTime(2025, 6, 17, 8, 52, 38, 690, DateTimeKind.Utc).AddTicks(4887) });

            migrationBuilder.UpdateData(
                table: "Skills",
                keyColumn: "SkillId",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 17, 8, 52, 38, 690, DateTimeKind.Utc).AddTicks(4889), new DateTime(2025, 6, 17, 8, 52, 38, 690, DateTimeKind.Utc).AddTicks(4890) });

            migrationBuilder.UpdateData(
                table: "Skills",
                keyColumn: "SkillId",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 17, 8, 52, 38, 690, DateTimeKind.Utc).AddTicks(4891), new DateTime(2025, 6, 17, 8, 52, 38, 690, DateTimeKind.Utc).AddTicks(4892) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "YourSkillAndExperience",
                table: "Jobs");

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
    }
}
