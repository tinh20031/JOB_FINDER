using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JOB_FINDER_API.Migrations
{
    /// <inheritdoc />
    public partial class updateDBhighlightProject : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TeamSize",
                table: "HighlightProjects",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Industries",
                keyColumn: "IndustryId",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 24, 16, 49, 25, 110, DateTimeKind.Utc).AddTicks(9481), new DateTime(2025, 6, 24, 16, 49, 25, 110, DateTimeKind.Utc).AddTicks(9482) });

            migrationBuilder.UpdateData(
                table: "Industries",
                keyColumn: "IndustryId",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 24, 16, 49, 25, 110, DateTimeKind.Utc).AddTicks(9485), new DateTime(2025, 6, 24, 16, 49, 25, 110, DateTimeKind.Utc).AddTicks(9485) });

            migrationBuilder.UpdateData(
                table: "JobTypes",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 24, 16, 49, 25, 110, DateTimeKind.Utc).AddTicks(9521), new DateTime(2025, 6, 24, 16, 49, 25, 110, DateTimeKind.Utc).AddTicks(9522) });

            migrationBuilder.UpdateData(
                table: "JobTypes",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 24, 16, 49, 25, 110, DateTimeKind.Utc).AddTicks(9525), new DateTime(2025, 6, 24, 16, 49, 25, 110, DateTimeKind.Utc).AddTicks(9525) });

            migrationBuilder.UpdateData(
                table: "JobTypes",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 24, 16, 49, 25, 110, DateTimeKind.Utc).AddTicks(9527), new DateTime(2025, 6, 24, 16, 49, 25, 110, DateTimeKind.Utc).AddTicks(9527) });

            migrationBuilder.UpdateData(
                table: "Levels",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 24, 16, 49, 25, 110, DateTimeKind.Utc).AddTicks(9562), new DateTime(2025, 6, 24, 16, 49, 25, 110, DateTimeKind.Utc).AddTicks(9563) });

            migrationBuilder.UpdateData(
                table: "Levels",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 24, 16, 49, 25, 110, DateTimeKind.Utc).AddTicks(9565), new DateTime(2025, 6, 24, 16, 49, 25, 110, DateTimeKind.Utc).AddTicks(9565) });

            migrationBuilder.UpdateData(
                table: "Levels",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 24, 16, 49, 25, 110, DateTimeKind.Utc).AddTicks(9567), new DateTime(2025, 6, 24, 16, 49, 25, 110, DateTimeKind.Utc).AddTicks(9567) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 24, 16, 49, 25, 110, DateTimeKind.Utc).AddTicks(9338), new DateTime(2025, 6, 24, 16, 49, 25, 110, DateTimeKind.Utc).AddTicks(9340) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 24, 16, 49, 25, 110, DateTimeKind.Utc).AddTicks(9346), new DateTime(2025, 6, 24, 16, 49, 25, 110, DateTimeKind.Utc).AddTicks(9346) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 24, 16, 49, 25, 110, DateTimeKind.Utc).AddTicks(9347), new DateTime(2025, 6, 24, 16, 49, 25, 110, DateTimeKind.Utc).AddTicks(9347) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TeamSize",
                table: "HighlightProjects");

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
    }
}
