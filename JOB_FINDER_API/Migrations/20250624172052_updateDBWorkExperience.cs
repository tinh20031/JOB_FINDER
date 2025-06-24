using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JOB_FINDER_API.Migrations
{
    /// <inheritdoc />
    public partial class updateDBWorkExperience : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ProJects",
                table: "WorkExperiences",
                newName: "Technologies");

            migrationBuilder.AddColumn<string>(
                name: "Achievements",
                table: "WorkExperiences",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProjectName",
                table: "WorkExperiences",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Responsibilities",
                table: "WorkExperiences",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Industries",
                keyColumn: "IndustryId",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 24, 17, 20, 52, 585, DateTimeKind.Utc).AddTicks(994), new DateTime(2025, 6, 24, 17, 20, 52, 585, DateTimeKind.Utc).AddTicks(998) });

            migrationBuilder.UpdateData(
                table: "Industries",
                keyColumn: "IndustryId",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 24, 17, 20, 52, 585, DateTimeKind.Utc).AddTicks(1001), new DateTime(2025, 6, 24, 17, 20, 52, 585, DateTimeKind.Utc).AddTicks(1001) });

            migrationBuilder.UpdateData(
                table: "JobTypes",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 24, 17, 20, 52, 585, DateTimeKind.Utc).AddTicks(1063), new DateTime(2025, 6, 24, 17, 20, 52, 585, DateTimeKind.Utc).AddTicks(1064) });

            migrationBuilder.UpdateData(
                table: "JobTypes",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 24, 17, 20, 52, 585, DateTimeKind.Utc).AddTicks(1066), new DateTime(2025, 6, 24, 17, 20, 52, 585, DateTimeKind.Utc).AddTicks(1066) });

            migrationBuilder.UpdateData(
                table: "JobTypes",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 24, 17, 20, 52, 585, DateTimeKind.Utc).AddTicks(1068), new DateTime(2025, 6, 24, 17, 20, 52, 585, DateTimeKind.Utc).AddTicks(1068) });

            migrationBuilder.UpdateData(
                table: "Levels",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 24, 17, 20, 52, 585, DateTimeKind.Utc).AddTicks(1109), new DateTime(2025, 6, 24, 17, 20, 52, 585, DateTimeKind.Utc).AddTicks(1109) });

            migrationBuilder.UpdateData(
                table: "Levels",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 24, 17, 20, 52, 585, DateTimeKind.Utc).AddTicks(1111), new DateTime(2025, 6, 24, 17, 20, 52, 585, DateTimeKind.Utc).AddTicks(1111) });

            migrationBuilder.UpdateData(
                table: "Levels",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 24, 17, 20, 52, 585, DateTimeKind.Utc).AddTicks(1113), new DateTime(2025, 6, 24, 17, 20, 52, 585, DateTimeKind.Utc).AddTicks(1113) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 24, 17, 20, 52, 585, DateTimeKind.Utc).AddTicks(830), new DateTime(2025, 6, 24, 17, 20, 52, 585, DateTimeKind.Utc).AddTicks(832) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 24, 17, 20, 52, 585, DateTimeKind.Utc).AddTicks(836), new DateTime(2025, 6, 24, 17, 20, 52, 585, DateTimeKind.Utc).AddTicks(836) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 24, 17, 20, 52, 585, DateTimeKind.Utc).AddTicks(838), new DateTime(2025, 6, 24, 17, 20, 52, 585, DateTimeKind.Utc).AddTicks(838) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Achievements",
                table: "WorkExperiences");

            migrationBuilder.DropColumn(
                name: "ProjectName",
                table: "WorkExperiences");

            migrationBuilder.DropColumn(
                name: "Responsibilities",
                table: "WorkExperiences");

            migrationBuilder.RenameColumn(
                name: "Technologies",
                table: "WorkExperiences",
                newName: "ProJects");

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
    }
}
