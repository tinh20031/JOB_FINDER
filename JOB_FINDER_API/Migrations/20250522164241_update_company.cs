using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JOB_FINDER_API.Migrations
{
    /// <inheritdoc />
    public partial class update_company : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Image",
                table: "CandidateProfiles");

            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "Jobs",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<int>(
                name: "IndustryId",
                table: "CompanyProfile",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 5, 22, 16, 42, 40, 443, DateTimeKind.Utc).AddTicks(3517), new DateTime(2025, 5, 22, 16, 42, 40, 443, DateTimeKind.Utc).AddTicks(3522) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 5, 22, 16, 42, 40, 443, DateTimeKind.Utc).AddTicks(3526), new DateTime(2025, 5, 22, 16, 42, 40, 443, DateTimeKind.Utc).AddTicks(3526) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 5, 22, 16, 42, 40, 443, DateTimeKind.Utc).AddTicks(3527), new DateTime(2025, 5, 22, 16, 42, 40, 443, DateTimeKind.Utc).AddTicks(3527) });

            migrationBuilder.CreateIndex(
                name: "IX_CompanyProfile_IndustryId",
                table: "CompanyProfile",
                column: "IndustryId");

            migrationBuilder.AddForeignKey(
                name: "FK_CompanyProfile_Industries_IndustryId",
                table: "CompanyProfile",
                column: "IndustryId",
                principalTable: "Industries",
                principalColumn: "IndustryId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CompanyProfile_Industries_IndustryId",
                table: "CompanyProfile");

            migrationBuilder.DropIndex(
                name: "IX_CompanyProfile_IndustryId",
                table: "CompanyProfile");

            migrationBuilder.DropColumn(
                name: "IndustryId",
                table: "CompanyProfile");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Jobs",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "Image",
                table: "CandidateProfiles",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 5, 22, 12, 41, 55, 704, DateTimeKind.Utc).AddTicks(4219), new DateTime(2025, 5, 22, 12, 41, 55, 704, DateTimeKind.Utc).AddTicks(4222) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 5, 22, 12, 41, 55, 704, DateTimeKind.Utc).AddTicks(4226), new DateTime(2025, 5, 22, 12, 41, 55, 704, DateTimeKind.Utc).AddTicks(4226) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 5, 22, 12, 41, 55, 704, DateTimeKind.Utc).AddTicks(4227), new DateTime(2025, 5, 22, 12, 41, 55, 704, DateTimeKind.Utc).AddTicks(4228) });
        }
    }
}
