using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JOB_FINDER_API.Migrations
{
    /// <inheritdoc />
    public partial class update222 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EmployerProfiles_Users_UserId",
                table: "EmployerProfiles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EmployerProfiles",
                table: "EmployerProfiles");

            migrationBuilder.RenameTable(
                name: "EmployerProfiles",
                newName: "CompanyProfile");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CompanyProfile",
                table: "CompanyProfile",
                column: "UserId");

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 5, 21, 8, 41, 50, 891, DateTimeKind.Utc).AddTicks(7698), new DateTime(2025, 5, 21, 8, 41, 50, 891, DateTimeKind.Utc).AddTicks(7700) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 5, 21, 8, 41, 50, 891, DateTimeKind.Utc).AddTicks(7704), new DateTime(2025, 5, 21, 8, 41, 50, 891, DateTimeKind.Utc).AddTicks(7704) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 5, 21, 8, 41, 50, 891, DateTimeKind.Utc).AddTicks(7705), new DateTime(2025, 5, 21, 8, 41, 50, 891, DateTimeKind.Utc).AddTicks(7706) });

            migrationBuilder.AddForeignKey(
                name: "FK_CompanyProfile_Users_UserId",
                table: "CompanyProfile",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CompanyProfile_Users_UserId",
                table: "CompanyProfile");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CompanyProfile",
                table: "CompanyProfile");

            migrationBuilder.RenameTable(
                name: "CompanyProfile",
                newName: "EmployerProfiles");

            migrationBuilder.AddPrimaryKey(
                name: "PK_EmployerProfiles",
                table: "EmployerProfiles",
                column: "UserId");

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 5, 21, 8, 37, 42, 801, DateTimeKind.Utc).AddTicks(1003), new DateTime(2025, 5, 21, 8, 37, 42, 801, DateTimeKind.Utc).AddTicks(1006) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 5, 21, 8, 37, 42, 801, DateTimeKind.Utc).AddTicks(1010), new DateTime(2025, 5, 21, 8, 37, 42, 801, DateTimeKind.Utc).AddTicks(1011) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 5, 21, 8, 37, 42, 801, DateTimeKind.Utc).AddTicks(1012), new DateTime(2025, 5, 21, 8, 37, 42, 801, DateTimeKind.Utc).AddTicks(1012) });

            migrationBuilder.AddForeignKey(
                name: "FK_EmployerProfiles_Users_UserId",
                table: "EmployerProfiles",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");
        }
    }
}
