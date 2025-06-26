using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JOB_FINDER_API.Migrations
{
    /// <inheritdoc />
    public partial class updateDbUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "EmailVerificationCode",
                table: "Users",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "EmailVerificationCodeExpiry",
                table: "Users",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsEmailVerified",
                table: "Users",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "Industries",
                keyColumn: "IndustryId",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 25, 9, 47, 7, 101, DateTimeKind.Utc).AddTicks(9452), new DateTime(2025, 6, 25, 9, 47, 7, 101, DateTimeKind.Utc).AddTicks(9453) });

            migrationBuilder.UpdateData(
                table: "Industries",
                keyColumn: "IndustryId",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 25, 9, 47, 7, 101, DateTimeKind.Utc).AddTicks(9456), new DateTime(2025, 6, 25, 9, 47, 7, 101, DateTimeKind.Utc).AddTicks(9456) });

            migrationBuilder.UpdateData(
                table: "JobTypes",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 25, 9, 47, 7, 101, DateTimeKind.Utc).AddTicks(9485), new DateTime(2025, 6, 25, 9, 47, 7, 101, DateTimeKind.Utc).AddTicks(9486) });

            migrationBuilder.UpdateData(
                table: "JobTypes",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 25, 9, 47, 7, 101, DateTimeKind.Utc).AddTicks(9488), new DateTime(2025, 6, 25, 9, 47, 7, 101, DateTimeKind.Utc).AddTicks(9488) });

            migrationBuilder.UpdateData(
                table: "JobTypes",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 25, 9, 47, 7, 101, DateTimeKind.Utc).AddTicks(9490), new DateTime(2025, 6, 25, 9, 47, 7, 101, DateTimeKind.Utc).AddTicks(9490) });

            migrationBuilder.UpdateData(
                table: "Levels",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 25, 9, 47, 7, 101, DateTimeKind.Utc).AddTicks(9516), new DateTime(2025, 6, 25, 9, 47, 7, 101, DateTimeKind.Utc).AddTicks(9516) });

            migrationBuilder.UpdateData(
                table: "Levels",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 25, 9, 47, 7, 101, DateTimeKind.Utc).AddTicks(9518), new DateTime(2025, 6, 25, 9, 47, 7, 101, DateTimeKind.Utc).AddTicks(9519) });

            migrationBuilder.UpdateData(
                table: "Levels",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 25, 9, 47, 7, 101, DateTimeKind.Utc).AddTicks(9547), new DateTime(2025, 6, 25, 9, 47, 7, 101, DateTimeKind.Utc).AddTicks(9547) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 25, 9, 47, 7, 101, DateTimeKind.Utc).AddTicks(9319), new DateTime(2025, 6, 25, 9, 47, 7, 101, DateTimeKind.Utc).AddTicks(9321) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 25, 9, 47, 7, 101, DateTimeKind.Utc).AddTicks(9329), new DateTime(2025, 6, 25, 9, 47, 7, 101, DateTimeKind.Utc).AddTicks(9329) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 25, 9, 47, 7, 101, DateTimeKind.Utc).AddTicks(9330), new DateTime(2025, 6, 25, 9, 47, 7, 101, DateTimeKind.Utc).AddTicks(9331) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EmailVerificationCode",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "EmailVerificationCodeExpiry",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "IsEmailVerified",
                table: "Users");

            migrationBuilder.UpdateData(
                table: "Industries",
                keyColumn: "IndustryId",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 25, 9, 22, 54, 153, DateTimeKind.Utc).AddTicks(1693), new DateTime(2025, 6, 25, 9, 22, 54, 153, DateTimeKind.Utc).AddTicks(1694) });

            migrationBuilder.UpdateData(
                table: "Industries",
                keyColumn: "IndustryId",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 25, 9, 22, 54, 153, DateTimeKind.Utc).AddTicks(1697), new DateTime(2025, 6, 25, 9, 22, 54, 153, DateTimeKind.Utc).AddTicks(1697) });

            migrationBuilder.UpdateData(
                table: "JobTypes",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 25, 9, 22, 54, 153, DateTimeKind.Utc).AddTicks(1759), new DateTime(2025, 6, 25, 9, 22, 54, 153, DateTimeKind.Utc).AddTicks(1760) });

            migrationBuilder.UpdateData(
                table: "JobTypes",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 25, 9, 22, 54, 153, DateTimeKind.Utc).AddTicks(1761), new DateTime(2025, 6, 25, 9, 22, 54, 153, DateTimeKind.Utc).AddTicks(1762) });

            migrationBuilder.UpdateData(
                table: "JobTypes",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 25, 9, 22, 54, 153, DateTimeKind.Utc).AddTicks(1764), new DateTime(2025, 6, 25, 9, 22, 54, 153, DateTimeKind.Utc).AddTicks(1764) });

            migrationBuilder.UpdateData(
                table: "Levels",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 25, 9, 22, 54, 153, DateTimeKind.Utc).AddTicks(1805), new DateTime(2025, 6, 25, 9, 22, 54, 153, DateTimeKind.Utc).AddTicks(1805) });

            migrationBuilder.UpdateData(
                table: "Levels",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 25, 9, 22, 54, 153, DateTimeKind.Utc).AddTicks(1853), new DateTime(2025, 6, 25, 9, 22, 54, 153, DateTimeKind.Utc).AddTicks(1853) });

            migrationBuilder.UpdateData(
                table: "Levels",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 25, 9, 22, 54, 153, DateTimeKind.Utc).AddTicks(1855), new DateTime(2025, 6, 25, 9, 22, 54, 153, DateTimeKind.Utc).AddTicks(1855) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 25, 9, 22, 54, 153, DateTimeKind.Utc).AddTicks(1504), new DateTime(2025, 6, 25, 9, 22, 54, 153, DateTimeKind.Utc).AddTicks(1507) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 25, 9, 22, 54, 153, DateTimeKind.Utc).AddTicks(1513), new DateTime(2025, 6, 25, 9, 22, 54, 153, DateTimeKind.Utc).AddTicks(1513) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 25, 9, 22, 54, 153, DateTimeKind.Utc).AddTicks(1515), new DateTime(2025, 6, 25, 9, 22, 54, 153, DateTimeKind.Utc).AddTicks(1515) });
        }
    }
}
