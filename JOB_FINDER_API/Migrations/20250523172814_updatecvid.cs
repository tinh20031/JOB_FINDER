using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JOB_FINDER_API.Migrations
{
    /// <inheritdoc />
    public partial class updatecvid : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Applications_CVs_UniqueCvId",
                table: "Applications");

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "CompanyProfile",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "CVId",
                table: "Applications",
                type: "int",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 5, 23, 17, 28, 12, 989, DateTimeKind.Utc).AddTicks(7009), new DateTime(2025, 5, 23, 17, 28, 12, 989, DateTimeKind.Utc).AddTicks(7010) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 5, 23, 17, 28, 12, 989, DateTimeKind.Utc).AddTicks(7015), new DateTime(2025, 5, 23, 17, 28, 12, 989, DateTimeKind.Utc).AddTicks(7015) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 5, 23, 17, 28, 12, 989, DateTimeKind.Utc).AddTicks(7016), new DateTime(2025, 5, 23, 17, 28, 12, 989, DateTimeKind.Utc).AddTicks(7016) });

            migrationBuilder.CreateIndex(
                name: "IX_Applications_CVId",
                table: "Applications",
                column: "CVId");

            migrationBuilder.AddForeignKey(
                name: "FK_Applications_CVs_CVId",
                table: "Applications",
                column: "CVId",
                principalTable: "CVs",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Applications_CVs_UniqueCvId",
                table: "Applications",
                column: "CvId",
                principalTable: "CVs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Applications_CVs_CVId",
                table: "Applications");

            migrationBuilder.DropForeignKey(
                name: "FK_Applications_CVs_UniqueCvId",
                table: "Applications");

            migrationBuilder.DropIndex(
                name: "IX_Applications_CVId",
                table: "Applications");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "CompanyProfile");

            migrationBuilder.DropColumn(
                name: "CVId",
                table: "Applications");

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 5, 23, 6, 15, 40, 501, DateTimeKind.Utc).AddTicks(2299), new DateTime(2025, 5, 23, 6, 15, 40, 501, DateTimeKind.Utc).AddTicks(2303) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 5, 23, 6, 15, 40, 501, DateTimeKind.Utc).AddTicks(2308), new DateTime(2025, 5, 23, 6, 15, 40, 501, DateTimeKind.Utc).AddTicks(2308) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 5, 23, 6, 15, 40, 501, DateTimeKind.Utc).AddTicks(2309), new DateTime(2025, 5, 23, 6, 15, 40, 501, DateTimeKind.Utc).AddTicks(2310) });

            migrationBuilder.AddForeignKey(
                name: "FK_Applications_CVs_UniqueCvId",
                table: "Applications",
                column: "CvId",
                principalTable: "CVs",
                principalColumn: "Id");
        }
    }
}
