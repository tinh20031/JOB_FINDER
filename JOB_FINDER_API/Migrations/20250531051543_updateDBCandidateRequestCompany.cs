using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JOB_FINDER_API.Migrations
{
    /// <inheritdoc />
    public partial class updateDBCandidateRequestCompany : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CandidateToCompanyRequests",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    CompanyName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CompanyProfileDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Location = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TeamSize = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Website = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Contact = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IndustryId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CandidateToCompanyRequests", x => x.Id);
                });

            migrationBuilder.UpdateData(
                table: "Experiences",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 5, 31, 5, 15, 41, 732, DateTimeKind.Utc).AddTicks(7149), new DateTime(2025, 5, 31, 5, 15, 41, 732, DateTimeKind.Utc).AddTicks(7150) });

            migrationBuilder.UpdateData(
                table: "Experiences",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 5, 31, 5, 15, 41, 732, DateTimeKind.Utc).AddTicks(7171), new DateTime(2025, 5, 31, 5, 15, 41, 732, DateTimeKind.Utc).AddTicks(7172) });

            migrationBuilder.UpdateData(
                table: "Experiences",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 5, 31, 5, 15, 41, 732, DateTimeKind.Utc).AddTicks(7173), new DateTime(2025, 5, 31, 5, 15, 41, 732, DateTimeKind.Utc).AddTicks(7173) });

            migrationBuilder.UpdateData(
                table: "Industries",
                keyColumn: "IndustryId",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 5, 31, 5, 15, 41, 732, DateTimeKind.Utc).AddTicks(7210), new DateTime(2025, 5, 31, 5, 15, 41, 732, DateTimeKind.Utc).AddTicks(7212) });

            migrationBuilder.UpdateData(
                table: "Industries",
                keyColumn: "IndustryId",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 5, 31, 5, 15, 41, 732, DateTimeKind.Utc).AddTicks(7213), new DateTime(2025, 5, 31, 5, 15, 41, 732, DateTimeKind.Utc).AddTicks(7214) });

            migrationBuilder.UpdateData(
                table: "JobTypes",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 5, 31, 5, 15, 41, 732, DateTimeKind.Utc).AddTicks(7249), new DateTime(2025, 5, 31, 5, 15, 41, 732, DateTimeKind.Utc).AddTicks(7250) });

            migrationBuilder.UpdateData(
                table: "JobTypes",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 5, 31, 5, 15, 41, 732, DateTimeKind.Utc).AddTicks(7252), new DateTime(2025, 5, 31, 5, 15, 41, 732, DateTimeKind.Utc).AddTicks(7252) });

            migrationBuilder.UpdateData(
                table: "JobTypes",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 5, 31, 5, 15, 41, 732, DateTimeKind.Utc).AddTicks(7254), new DateTime(2025, 5, 31, 5, 15, 41, 732, DateTimeKind.Utc).AddTicks(7254) });

            migrationBuilder.UpdateData(
                table: "Levels",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 5, 31, 5, 15, 41, 732, DateTimeKind.Utc).AddTicks(7283), new DateTime(2025, 5, 31, 5, 15, 41, 732, DateTimeKind.Utc).AddTicks(7283) });

            migrationBuilder.UpdateData(
                table: "Levels",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 5, 31, 5, 15, 41, 732, DateTimeKind.Utc).AddTicks(7286), new DateTime(2025, 5, 31, 5, 15, 41, 732, DateTimeKind.Utc).AddTicks(7286) });

            migrationBuilder.UpdateData(
                table: "Levels",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 5, 31, 5, 15, 41, 732, DateTimeKind.Utc).AddTicks(7288), new DateTime(2025, 5, 31, 5, 15, 41, 732, DateTimeKind.Utc).AddTicks(7288) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 5, 31, 5, 15, 41, 732, DateTimeKind.Utc).AddTicks(6978), new DateTime(2025, 5, 31, 5, 15, 41, 732, DateTimeKind.Utc).AddTicks(6981) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 5, 31, 5, 15, 41, 732, DateTimeKind.Utc).AddTicks(6985), new DateTime(2025, 5, 31, 5, 15, 41, 732, DateTimeKind.Utc).AddTicks(6986) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 5, 31, 5, 15, 41, 732, DateTimeKind.Utc).AddTicks(6987), new DateTime(2025, 5, 31, 5, 15, 41, 732, DateTimeKind.Utc).AddTicks(6987) });

            migrationBuilder.UpdateData(
                table: "Skills",
                keyColumn: "SkillId",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 5, 31, 5, 15, 41, 732, DateTimeKind.Utc).AddTicks(7327), new DateTime(2025, 5, 31, 5, 15, 41, 732, DateTimeKind.Utc).AddTicks(7328) });

            migrationBuilder.UpdateData(
                table: "Skills",
                keyColumn: "SkillId",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 5, 31, 5, 15, 41, 732, DateTimeKind.Utc).AddTicks(7329), new DateTime(2025, 5, 31, 5, 15, 41, 732, DateTimeKind.Utc).AddTicks(7330) });

            migrationBuilder.UpdateData(
                table: "Skills",
                keyColumn: "SkillId",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 5, 31, 5, 15, 41, 732, DateTimeKind.Utc).AddTicks(7331), new DateTime(2025, 5, 31, 5, 15, 41, 732, DateTimeKind.Utc).AddTicks(7332) });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 5, 31, 5, 15, 41, 732, DateTimeKind.Utc).AddTicks(7370), new DateTime(2025, 5, 31, 5, 15, 41, 732, DateTimeKind.Utc).AddTicks(7370) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CandidateToCompanyRequests");

            migrationBuilder.UpdateData(
                table: "Experiences",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 5, 26, 14, 5, 5, 696, DateTimeKind.Utc).AddTicks(4545), new DateTime(2025, 5, 26, 14, 5, 5, 696, DateTimeKind.Utc).AddTicks(4546) });

            migrationBuilder.UpdateData(
                table: "Experiences",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 5, 26, 14, 5, 5, 696, DateTimeKind.Utc).AddTicks(4547), new DateTime(2025, 5, 26, 14, 5, 5, 696, DateTimeKind.Utc).AddTicks(4548) });

            migrationBuilder.UpdateData(
                table: "Experiences",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 5, 26, 14, 5, 5, 696, DateTimeKind.Utc).AddTicks(4549), new DateTime(2025, 5, 26, 14, 5, 5, 696, DateTimeKind.Utc).AddTicks(4549) });

            migrationBuilder.UpdateData(
                table: "Industries",
                keyColumn: "IndustryId",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 5, 26, 14, 5, 5, 696, DateTimeKind.Utc).AddTicks(4621), new DateTime(2025, 5, 26, 14, 5, 5, 696, DateTimeKind.Utc).AddTicks(4622) });

            migrationBuilder.UpdateData(
                table: "Industries",
                keyColumn: "IndustryId",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 5, 26, 14, 5, 5, 696, DateTimeKind.Utc).AddTicks(4624), new DateTime(2025, 5, 26, 14, 5, 5, 696, DateTimeKind.Utc).AddTicks(4624) });

            migrationBuilder.UpdateData(
                table: "JobTypes",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 5, 26, 14, 5, 5, 696, DateTimeKind.Utc).AddTicks(4658), new DateTime(2025, 5, 26, 14, 5, 5, 696, DateTimeKind.Utc).AddTicks(4659) });

            migrationBuilder.UpdateData(
                table: "JobTypes",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 5, 26, 14, 5, 5, 696, DateTimeKind.Utc).AddTicks(4661), new DateTime(2025, 5, 26, 14, 5, 5, 696, DateTimeKind.Utc).AddTicks(4661) });

            migrationBuilder.UpdateData(
                table: "JobTypes",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 5, 26, 14, 5, 5, 696, DateTimeKind.Utc).AddTicks(4662), new DateTime(2025, 5, 26, 14, 5, 5, 696, DateTimeKind.Utc).AddTicks(4663) });

            migrationBuilder.UpdateData(
                table: "Levels",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 5, 26, 14, 5, 5, 696, DateTimeKind.Utc).AddTicks(4694), new DateTime(2025, 5, 26, 14, 5, 5, 696, DateTimeKind.Utc).AddTicks(4695) });

            migrationBuilder.UpdateData(
                table: "Levels",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 5, 26, 14, 5, 5, 696, DateTimeKind.Utc).AddTicks(4697), new DateTime(2025, 5, 26, 14, 5, 5, 696, DateTimeKind.Utc).AddTicks(4697) });

            migrationBuilder.UpdateData(
                table: "Levels",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 5, 26, 14, 5, 5, 696, DateTimeKind.Utc).AddTicks(4699), new DateTime(2025, 5, 26, 14, 5, 5, 696, DateTimeKind.Utc).AddTicks(4699) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 5, 26, 14, 5, 5, 696, DateTimeKind.Utc).AddTicks(4365), new DateTime(2025, 5, 26, 14, 5, 5, 696, DateTimeKind.Utc).AddTicks(4369) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 5, 26, 14, 5, 5, 696, DateTimeKind.Utc).AddTicks(4373), new DateTime(2025, 5, 26, 14, 5, 5, 696, DateTimeKind.Utc).AddTicks(4373) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 5, 26, 14, 5, 5, 696, DateTimeKind.Utc).AddTicks(4374), new DateTime(2025, 5, 26, 14, 5, 5, 696, DateTimeKind.Utc).AddTicks(4374) });

            migrationBuilder.UpdateData(
                table: "Skills",
                keyColumn: "SkillId",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 5, 26, 14, 5, 5, 696, DateTimeKind.Utc).AddTicks(4731), new DateTime(2025, 5, 26, 14, 5, 5, 696, DateTimeKind.Utc).AddTicks(4732) });

            migrationBuilder.UpdateData(
                table: "Skills",
                keyColumn: "SkillId",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 5, 26, 14, 5, 5, 696, DateTimeKind.Utc).AddTicks(4734), new DateTime(2025, 5, 26, 14, 5, 5, 696, DateTimeKind.Utc).AddTicks(4734) });

            migrationBuilder.UpdateData(
                table: "Skills",
                keyColumn: "SkillId",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 5, 26, 14, 5, 5, 696, DateTimeKind.Utc).AddTicks(4736), new DateTime(2025, 5, 26, 14, 5, 5, 696, DateTimeKind.Utc).AddTicks(4736) });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 5, 26, 14, 5, 5, 696, DateTimeKind.Utc).AddTicks(4781), new DateTime(2025, 5, 26, 14, 5, 5, 696, DateTimeKind.Utc).AddTicks(4782) });
        }
    }
}
