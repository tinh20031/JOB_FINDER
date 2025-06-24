using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JOB_FINDER_API.Migrations
{
    /// <inheritdoc />
    public partial class updateDbProjectHighlight : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Achievements",
                table: "HighlightProjects",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Responsibilities",
                table: "HighlightProjects",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Technologies",
                table: "HighlightProjects",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Industries",
                keyColumn: "IndustryId",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 24, 16, 37, 7, 140, DateTimeKind.Utc).AddTicks(6800), new DateTime(2025, 6, 24, 16, 37, 7, 140, DateTimeKind.Utc).AddTicks(6800) });

            migrationBuilder.UpdateData(
                table: "Industries",
                keyColumn: "IndustryId",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 24, 16, 37, 7, 140, DateTimeKind.Utc).AddTicks(6803), new DateTime(2025, 6, 24, 16, 37, 7, 140, DateTimeKind.Utc).AddTicks(6804) });

            migrationBuilder.UpdateData(
                table: "JobTypes",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 24, 16, 37, 7, 140, DateTimeKind.Utc).AddTicks(6839), new DateTime(2025, 6, 24, 16, 37, 7, 140, DateTimeKind.Utc).AddTicks(6840) });

            migrationBuilder.UpdateData(
                table: "JobTypes",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 24, 16, 37, 7, 140, DateTimeKind.Utc).AddTicks(6842), new DateTime(2025, 6, 24, 16, 37, 7, 140, DateTimeKind.Utc).AddTicks(6843) });

            migrationBuilder.UpdateData(
                table: "JobTypes",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 24, 16, 37, 7, 140, DateTimeKind.Utc).AddTicks(6844), new DateTime(2025, 6, 24, 16, 37, 7, 140, DateTimeKind.Utc).AddTicks(6845) });

            migrationBuilder.UpdateData(
                table: "Levels",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 24, 16, 37, 7, 140, DateTimeKind.Utc).AddTicks(6873), new DateTime(2025, 6, 24, 16, 37, 7, 140, DateTimeKind.Utc).AddTicks(6873) });

            migrationBuilder.UpdateData(
                table: "Levels",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 24, 16, 37, 7, 140, DateTimeKind.Utc).AddTicks(6875), new DateTime(2025, 6, 24, 16, 37, 7, 140, DateTimeKind.Utc).AddTicks(6875) });

            migrationBuilder.UpdateData(
                table: "Levels",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 24, 16, 37, 7, 140, DateTimeKind.Utc).AddTicks(6877), new DateTime(2025, 6, 24, 16, 37, 7, 140, DateTimeKind.Utc).AddTicks(6878) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 24, 16, 37, 7, 140, DateTimeKind.Utc).AddTicks(6651), new DateTime(2025, 6, 24, 16, 37, 7, 140, DateTimeKind.Utc).AddTicks(6654) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 24, 16, 37, 7, 140, DateTimeKind.Utc).AddTicks(6659), new DateTime(2025, 6, 24, 16, 37, 7, 140, DateTimeKind.Utc).AddTicks(6659) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 24, 16, 37, 7, 140, DateTimeKind.Utc).AddTicks(6661), new DateTime(2025, 6, 24, 16, 37, 7, 140, DateTimeKind.Utc).AddTicks(6661) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Achievements",
                table: "HighlightProjects");

            migrationBuilder.DropColumn(
                name: "Responsibilities",
                table: "HighlightProjects");

            migrationBuilder.DropColumn(
                name: "Technologies",
                table: "HighlightProjects");

            migrationBuilder.UpdateData(
                table: "Industries",
                keyColumn: "IndustryId",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 23, 13, 15, 42, 823, DateTimeKind.Utc).AddTicks(2953), new DateTime(2025, 6, 23, 13, 15, 42, 823, DateTimeKind.Utc).AddTicks(2954) });

            migrationBuilder.UpdateData(
                table: "Industries",
                keyColumn: "IndustryId",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 23, 13, 15, 42, 823, DateTimeKind.Utc).AddTicks(2957), new DateTime(2025, 6, 23, 13, 15, 42, 823, DateTimeKind.Utc).AddTicks(2957) });

            migrationBuilder.UpdateData(
                table: "JobTypes",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 23, 13, 15, 42, 823, DateTimeKind.Utc).AddTicks(2998), new DateTime(2025, 6, 23, 13, 15, 42, 823, DateTimeKind.Utc).AddTicks(2998) });

            migrationBuilder.UpdateData(
                table: "JobTypes",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 23, 13, 15, 42, 823, DateTimeKind.Utc).AddTicks(3001), new DateTime(2025, 6, 23, 13, 15, 42, 823, DateTimeKind.Utc).AddTicks(3001) });

            migrationBuilder.UpdateData(
                table: "JobTypes",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 23, 13, 15, 42, 823, DateTimeKind.Utc).AddTicks(3003), new DateTime(2025, 6, 23, 13, 15, 42, 823, DateTimeKind.Utc).AddTicks(3003) });

            migrationBuilder.UpdateData(
                table: "Levels",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 23, 13, 15, 42, 823, DateTimeKind.Utc).AddTicks(3036), new DateTime(2025, 6, 23, 13, 15, 42, 823, DateTimeKind.Utc).AddTicks(3037) });

            migrationBuilder.UpdateData(
                table: "Levels",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 23, 13, 15, 42, 823, DateTimeKind.Utc).AddTicks(3038), new DateTime(2025, 6, 23, 13, 15, 42, 823, DateTimeKind.Utc).AddTicks(3039) });

            migrationBuilder.UpdateData(
                table: "Levels",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 23, 13, 15, 42, 823, DateTimeKind.Utc).AddTicks(3040), new DateTime(2025, 6, 23, 13, 15, 42, 823, DateTimeKind.Utc).AddTicks(3041) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 23, 13, 15, 42, 823, DateTimeKind.Utc).AddTicks(2798), new DateTime(2025, 6, 23, 13, 15, 42, 823, DateTimeKind.Utc).AddTicks(2800) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 23, 13, 15, 42, 823, DateTimeKind.Utc).AddTicks(2804), new DateTime(2025, 6, 23, 13, 15, 42, 823, DateTimeKind.Utc).AddTicks(2805) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 23, 13, 15, 42, 823, DateTimeKind.Utc).AddTicks(2806), new DateTime(2025, 6, 23, 13, 15, 42, 823, DateTimeKind.Utc).AddTicks(2806) });
        }
    }
}
