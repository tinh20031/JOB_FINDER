using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace JOB_FINDER_API.Migrations
{
    /// <inheritdoc />
    public partial class init2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Industries",
                columns: new[] { "IndustryId", "CreatedAt", "IndustryName", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 5, 26, 14, 5, 5, 696, DateTimeKind.Utc).AddTicks(4621), "Information Technology", new DateTime(2025, 5, 26, 14, 5, 5, 696, DateTimeKind.Utc).AddTicks(4622) },
                    { 2, new DateTime(2025, 5, 26, 14, 5, 5, 696, DateTimeKind.Utc).AddTicks(4624), "Finance", new DateTime(2025, 5, 26, 14, 5, 5, 696, DateTimeKind.Utc).AddTicks(4624) }
                });

            migrationBuilder.InsertData(
                table: "JobTypes",
                columns: new[] { "Id", "CreatedAt", "JobTypeName", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 5, 26, 14, 5, 5, 696, DateTimeKind.Utc).AddTicks(4658), "Full-time", new DateTime(2025, 5, 26, 14, 5, 5, 696, DateTimeKind.Utc).AddTicks(4659) },
                    { 2, new DateTime(2025, 5, 26, 14, 5, 5, 696, DateTimeKind.Utc).AddTicks(4661), "Part-time", new DateTime(2025, 5, 26, 14, 5, 5, 696, DateTimeKind.Utc).AddTicks(4661) },
                    { 3, new DateTime(2025, 5, 26, 14, 5, 5, 696, DateTimeKind.Utc).AddTicks(4662), "Remote", new DateTime(2025, 5, 26, 14, 5, 5, 696, DateTimeKind.Utc).AddTicks(4663) }
                });

            migrationBuilder.InsertData(
                table: "Levels",
                columns: new[] { "Id", "CreatedAt", "LevelName", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 5, 26, 14, 5, 5, 696, DateTimeKind.Utc).AddTicks(4694), "Intern", new DateTime(2025, 5, 26, 14, 5, 5, 696, DateTimeKind.Utc).AddTicks(4695) },
                    { 2, new DateTime(2025, 5, 26, 14, 5, 5, 696, DateTimeKind.Utc).AddTicks(4697), "Junior", new DateTime(2025, 5, 26, 14, 5, 5, 696, DateTimeKind.Utc).AddTicks(4697) },
                    { 3, new DateTime(2025, 5, 26, 14, 5, 5, 696, DateTimeKind.Utc).AddTicks(4699), "Senior", new DateTime(2025, 5, 26, 14, 5, 5, 696, DateTimeKind.Utc).AddTicks(4699) }
                });

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

            migrationBuilder.InsertData(
                table: "Skills",
                columns: new[] { "SkillId", "CreatedAt", "SkillName", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 5, 26, 14, 5, 5, 696, DateTimeKind.Utc).AddTicks(4731), "C#", new DateTime(2025, 5, 26, 14, 5, 5, 696, DateTimeKind.Utc).AddTicks(4732) },
                    { 2, new DateTime(2025, 5, 26, 14, 5, 5, 696, DateTimeKind.Utc).AddTicks(4734), "JavaScript", new DateTime(2025, 5, 26, 14, 5, 5, 696, DateTimeKind.Utc).AddTicks(4734) },
                    { 3, new DateTime(2025, 5, 26, 14, 5, 5, 696, DateTimeKind.Utc).AddTicks(4736), "SQL", new DateTime(2025, 5, 26, 14, 5, 5, 696, DateTimeKind.Utc).AddTicks(4736) }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "CreatedAt", "Email", "FullName", "Image", "IsActive", "Password", "Phone", "RoleId", "UpdatedAt" },
                values: new object[] { 1, new DateTime(2025, 5, 26, 14, 5, 5, 696, DateTimeKind.Utc).AddTicks(4781), "tinhadmin@gmail.com", "tinh", null, true, "123", "0123456789", 1, new DateTime(2025, 5, 26, 14, 5, 5, 696, DateTimeKind.Utc).AddTicks(4782) });

            migrationBuilder.InsertData(
                table: "Experiences",
                columns: new[] { "Id", "CreatedAt", "ExperienceName", "UpdatedAt", "UserId" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 5, 26, 14, 5, 5, 696, DateTimeKind.Utc).AddTicks(4545), "Less than 1 year", new DateTime(2025, 5, 26, 14, 5, 5, 696, DateTimeKind.Utc).AddTicks(4546), 1 },
                    { 2, new DateTime(2025, 5, 26, 14, 5, 5, 696, DateTimeKind.Utc).AddTicks(4547), "1-3 years", new DateTime(2025, 5, 26, 14, 5, 5, 696, DateTimeKind.Utc).AddTicks(4548), 1 },
                    { 3, new DateTime(2025, 5, 26, 14, 5, 5, 696, DateTimeKind.Utc).AddTicks(4549), "More than 3 years", new DateTime(2025, 5, 26, 14, 5, 5, 696, DateTimeKind.Utc).AddTicks(4549), 1 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Experiences",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Experiences",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Experiences",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Industries",
                keyColumn: "IndustryId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Industries",
                keyColumn: "IndustryId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "JobTypes",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "JobTypes",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "JobTypes",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Levels",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Levels",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Levels",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Skills",
                keyColumn: "SkillId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Skills",
                keyColumn: "SkillId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Skills",
                keyColumn: "SkillId",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 5, 25, 8, 1, 52, 218, DateTimeKind.Utc).AddTicks(1275), new DateTime(2025, 5, 25, 8, 1, 52, 218, DateTimeKind.Utc).AddTicks(1278) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 5, 25, 8, 1, 52, 218, DateTimeKind.Utc).AddTicks(1281), new DateTime(2025, 5, 25, 8, 1, 52, 218, DateTimeKind.Utc).AddTicks(1282) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 5, 25, 8, 1, 52, 218, DateTimeKind.Utc).AddTicks(1283), new DateTime(2025, 5, 25, 8, 1, 52, 218, DateTimeKind.Utc).AddTicks(1283) });
        }
    }
}
