using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace JOB_FINDER_API.Migrations
{
    /// <inheritdoc />
    public partial class init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "DeactivatedByAdmin",
                table: "Jobs",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.InsertData(
                table: "CandidateProfiles",
                columns: new[] { "UserId", "Address", "City", "Description", "Dob", "Gender", "JobTitle", "Language", "Province" },
                values: new object[] { 1, "123 Main Street", "Ho Chi Minh City", "A passionate developer with expertise in C# and JavaScript.", new DateTime(1995, 5, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "Male", "Software Developer", "English, Vietnamese", "Ho Chi Minh" });

            migrationBuilder.InsertData(
                table: "ExperienceLevel",
                columns: new[] { "id", "name" },
                values: new object[,]
                {
                    { 1, "Fresher" },
                    { 2, "Junior" },
                    { 3, "Middle" },
                    { 4, "Senior" }
                });

            migrationBuilder.UpdateData(
                table: "Experiences",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 9, 3, 20, 27, 0, DateTimeKind.Utc).AddTicks(5232), new DateTime(2025, 6, 9, 3, 20, 27, 0, DateTimeKind.Utc).AddTicks(5233) });

            migrationBuilder.UpdateData(
                table: "Experiences",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 9, 3, 20, 27, 0, DateTimeKind.Utc).AddTicks(5234), new DateTime(2025, 6, 9, 3, 20, 27, 0, DateTimeKind.Utc).AddTicks(5235) });

            migrationBuilder.UpdateData(
                table: "Experiences",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 9, 3, 20, 27, 0, DateTimeKind.Utc).AddTicks(5236), new DateTime(2025, 6, 9, 3, 20, 27, 0, DateTimeKind.Utc).AddTicks(5237) });

            migrationBuilder.UpdateData(
                table: "Industries",
                keyColumn: "IndustryId",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 9, 3, 20, 27, 0, DateTimeKind.Utc).AddTicks(5266), new DateTime(2025, 6, 9, 3, 20, 27, 0, DateTimeKind.Utc).AddTicks(5267) });

            migrationBuilder.UpdateData(
                table: "Industries",
                keyColumn: "IndustryId",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 9, 3, 20, 27, 0, DateTimeKind.Utc).AddTicks(5269), new DateTime(2025, 6, 9, 3, 20, 27, 0, DateTimeKind.Utc).AddTicks(5270) });

            migrationBuilder.UpdateData(
                table: "JobTypes",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 9, 3, 20, 27, 0, DateTimeKind.Utc).AddTicks(5301), new DateTime(2025, 6, 9, 3, 20, 27, 0, DateTimeKind.Utc).AddTicks(5302) });

            migrationBuilder.UpdateData(
                table: "JobTypes",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 9, 3, 20, 27, 0, DateTimeKind.Utc).AddTicks(5304), new DateTime(2025, 6, 9, 3, 20, 27, 0, DateTimeKind.Utc).AddTicks(5305) });

            migrationBuilder.UpdateData(
                table: "JobTypes",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 9, 3, 20, 27, 0, DateTimeKind.Utc).AddTicks(5307), new DateTime(2025, 6, 9, 3, 20, 27, 0, DateTimeKind.Utc).AddTicks(5307) });

            migrationBuilder.UpdateData(
                table: "Levels",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 9, 3, 20, 27, 0, DateTimeKind.Utc).AddTicks(5335), new DateTime(2025, 6, 9, 3, 20, 27, 0, DateTimeKind.Utc).AddTicks(5335) });

            migrationBuilder.UpdateData(
                table: "Levels",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 9, 3, 20, 27, 0, DateTimeKind.Utc).AddTicks(5338), new DateTime(2025, 6, 9, 3, 20, 27, 0, DateTimeKind.Utc).AddTicks(5338) });

            migrationBuilder.UpdateData(
                table: "Levels",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 9, 3, 20, 27, 0, DateTimeKind.Utc).AddTicks(5340), new DateTime(2025, 6, 9, 3, 20, 27, 0, DateTimeKind.Utc).AddTicks(5340) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 9, 3, 20, 27, 0, DateTimeKind.Utc).AddTicks(5082), new DateTime(2025, 6, 9, 3, 20, 27, 0, DateTimeKind.Utc).AddTicks(5085) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 9, 3, 20, 27, 0, DateTimeKind.Utc).AddTicks(5090), new DateTime(2025, 6, 9, 3, 20, 27, 0, DateTimeKind.Utc).AddTicks(5090) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 9, 3, 20, 27, 0, DateTimeKind.Utc).AddTicks(5092), new DateTime(2025, 6, 9, 3, 20, 27, 0, DateTimeKind.Utc).AddTicks(5092) });

            migrationBuilder.UpdateData(
                table: "Skills",
                keyColumn: "SkillId",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 9, 3, 20, 27, 0, DateTimeKind.Utc).AddTicks(5371), new DateTime(2025, 6, 9, 3, 20, 27, 0, DateTimeKind.Utc).AddTicks(5371) });

            migrationBuilder.UpdateData(
                table: "Skills",
                keyColumn: "SkillId",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 9, 3, 20, 27, 0, DateTimeKind.Utc).AddTicks(5374), new DateTime(2025, 6, 9, 3, 20, 27, 0, DateTimeKind.Utc).AddTicks(5374) });

            migrationBuilder.UpdateData(
                table: "Skills",
                keyColumn: "SkillId",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 9, 3, 20, 27, 0, DateTimeKind.Utc).AddTicks(5376), new DateTime(2025, 6, 9, 3, 20, 27, 0, DateTimeKind.Utc).AddTicks(5376) });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 9, 3, 20, 27, 0, DateTimeKind.Utc).AddTicks(5489), new DateTime(2025, 6, 9, 3, 20, 27, 0, DateTimeKind.Utc).AddTicks(5489) });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "CreatedAt", "Email", "FullName", "Image", "IsActive", "Password", "Phone", "RoleId", "UpdatedAt" },
                values: new object[,]
                {
                    { 2, new DateTime(2025, 6, 9, 3, 20, 27, 0, DateTimeKind.Utc).AddTicks(5493), "contact@techcorp.com", "Tech Corp", null, true, "123", "0987654321", 2, new DateTime(2025, 6, 9, 3, 20, 27, 0, DateTimeKind.Utc).AddTicks(5494) },
                    { 3, new DateTime(2025, 6, 9, 3, 20, 27, 0, DateTimeKind.Utc).AddTicks(5516), "admin@jobfinder.com", "Admin User", null, true, "123", "0912345678", 3, new DateTime(2025, 6, 9, 3, 20, 27, 0, DateTimeKind.Utc).AddTicks(5517) }
                });

            migrationBuilder.InsertData(
                table: "CompanyProfile",
                columns: new[] { "UserId", "CompanyName", "CompanyProfileDescription", "Contact", "ImageLogoLgr", "IndustryId", "IsActive", "IsVerified", "Location", "TeamSize", "UrlCompanyLogo", "Website" },
                values: new object[] { 2, "Tech Corp", "A leading tech company specializing in software solutions.", "contact@techcorp.com", "https://res.cloudinary.com/dzf0ccons/image/upload/v1748267504/image_user/bfltymsad63wfkyp3bvl.jpg", 1, true, true, "123 Tech Street, District 1, Ho Chi Minh City", "50-100 employees", "https://res.cloudinary.com/dzf0ccons/image/upload/v1748267504/image_user/bfltymsad63wfkyp3bvl.jpg", "https://techcorp.com" });

            migrationBuilder.InsertData(
                table: "Jobs",
                columns: new[] { "JobId", "AddressDetail", "CompanyId", "CreatedAt", "DeactivatedByAdmin", "Description", "ExperienceLevelId", "ExpiryDate", "IndustryId", "JobTypeId", "LevelId", "ProvinceName", "Salary", "Status", "TimeEnd", "TimeStart", "Title", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, "123 Tech Street, District 1", 2, new DateTime(2025, 6, 9, 3, 20, 27, 0, DateTimeKind.Utc).AddTicks(5444), false, "Develop and maintain web applications using C# and JavaScript.", 2, new DateTime(2025, 7, 9, 3, 20, 27, 0, DateTimeKind.Utc).AddTicks(5434), 1, 1, 2, "Ho Chi Minh City", 50000, 1, new DateTime(2025, 7, 9, 3, 20, 27, 0, DateTimeKind.Utc).AddTicks(5442), new DateTime(2025, 6, 9, 3, 20, 27, 0, DateTimeKind.Utc).AddTicks(5441), "Junior Software Engineer", new DateTime(2025, 6, 9, 3, 20, 27, 0, DateTimeKind.Utc).AddTicks(5445) },
                    { 2, "456 Finance Avenue, Ba Dinh", 2, new DateTime(2025, 6, 9, 3, 20, 27, 0, DateTimeKind.Utc).AddTicks(5449), false, "Analyze financial data and generate reports.", 4, new DateTime(2025, 7, 24, 3, 20, 27, 0, DateTimeKind.Utc).AddTicks(5447), 2, 3, 3, "Hanoi", 80000, 0, new DateTime(2025, 7, 24, 3, 20, 27, 0, DateTimeKind.Utc).AddTicks(5448), new DateTime(2025, 6, 9, 3, 20, 27, 0, DateTimeKind.Utc).AddTicks(5448), "Senior Data Analyst", new DateTime(2025, 6, 9, 3, 20, 27, 0, DateTimeKind.Utc).AddTicks(5450) }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "CandidateProfiles",
                keyColumn: "UserId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "CompanyProfile",
                keyColumn: "UserId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "ExperienceLevel",
                keyColumn: "id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "ExperienceLevel",
                keyColumn: "id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Jobs",
                keyColumn: "JobId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Jobs",
                keyColumn: "JobId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "ExperienceLevel",
                keyColumn: "id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "ExperienceLevel",
                keyColumn: "id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DropColumn(
                name: "DeactivatedByAdmin",
                table: "Jobs");

            migrationBuilder.UpdateData(
                table: "Experiences",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 2, 4, 33, 52, 493, DateTimeKind.Utc).AddTicks(2835), new DateTime(2025, 6, 2, 4, 33, 52, 493, DateTimeKind.Utc).AddTicks(2836) });

            migrationBuilder.UpdateData(
                table: "Experiences",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 2, 4, 33, 52, 493, DateTimeKind.Utc).AddTicks(2837), new DateTime(2025, 6, 2, 4, 33, 52, 493, DateTimeKind.Utc).AddTicks(2838) });

            migrationBuilder.UpdateData(
                table: "Experiences",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 2, 4, 33, 52, 493, DateTimeKind.Utc).AddTicks(2839), new DateTime(2025, 6, 2, 4, 33, 52, 493, DateTimeKind.Utc).AddTicks(2840) });

            migrationBuilder.UpdateData(
                table: "Industries",
                keyColumn: "IndustryId",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 2, 4, 33, 52, 493, DateTimeKind.Utc).AddTicks(2875), new DateTime(2025, 6, 2, 4, 33, 52, 493, DateTimeKind.Utc).AddTicks(2877) });

            migrationBuilder.UpdateData(
                table: "Industries",
                keyColumn: "IndustryId",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 2, 4, 33, 52, 493, DateTimeKind.Utc).AddTicks(2880), new DateTime(2025, 6, 2, 4, 33, 52, 493, DateTimeKind.Utc).AddTicks(2880) });

            migrationBuilder.UpdateData(
                table: "JobTypes",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 2, 4, 33, 52, 493, DateTimeKind.Utc).AddTicks(2915), new DateTime(2025, 6, 2, 4, 33, 52, 493, DateTimeKind.Utc).AddTicks(2916) });

            migrationBuilder.UpdateData(
                table: "JobTypes",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 2, 4, 33, 52, 493, DateTimeKind.Utc).AddTicks(2918), new DateTime(2025, 6, 2, 4, 33, 52, 493, DateTimeKind.Utc).AddTicks(2918) });

            migrationBuilder.UpdateData(
                table: "JobTypes",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 2, 4, 33, 52, 493, DateTimeKind.Utc).AddTicks(2919), new DateTime(2025, 6, 2, 4, 33, 52, 493, DateTimeKind.Utc).AddTicks(2920) });

            migrationBuilder.UpdateData(
                table: "Levels",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 2, 4, 33, 52, 493, DateTimeKind.Utc).AddTicks(2954), new DateTime(2025, 6, 2, 4, 33, 52, 493, DateTimeKind.Utc).AddTicks(2954) });

            migrationBuilder.UpdateData(
                table: "Levels",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 2, 4, 33, 52, 493, DateTimeKind.Utc).AddTicks(2956), new DateTime(2025, 6, 2, 4, 33, 52, 493, DateTimeKind.Utc).AddTicks(2957) });

            migrationBuilder.UpdateData(
                table: "Levels",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 2, 4, 33, 52, 493, DateTimeKind.Utc).AddTicks(2958), new DateTime(2025, 6, 2, 4, 33, 52, 493, DateTimeKind.Utc).AddTicks(2959) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 2, 4, 33, 52, 493, DateTimeKind.Utc).AddTicks(2672), new DateTime(2025, 6, 2, 4, 33, 52, 493, DateTimeKind.Utc).AddTicks(2674) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 2, 4, 33, 52, 493, DateTimeKind.Utc).AddTicks(2679), new DateTime(2025, 6, 2, 4, 33, 52, 493, DateTimeKind.Utc).AddTicks(2679) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 2, 4, 33, 52, 493, DateTimeKind.Utc).AddTicks(2680), new DateTime(2025, 6, 2, 4, 33, 52, 493, DateTimeKind.Utc).AddTicks(2681) });

            migrationBuilder.UpdateData(
                table: "Skills",
                keyColumn: "SkillId",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 2, 4, 33, 52, 493, DateTimeKind.Utc).AddTicks(3019), new DateTime(2025, 6, 2, 4, 33, 52, 493, DateTimeKind.Utc).AddTicks(3020) });

            migrationBuilder.UpdateData(
                table: "Skills",
                keyColumn: "SkillId",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 2, 4, 33, 52, 493, DateTimeKind.Utc).AddTicks(3022), new DateTime(2025, 6, 2, 4, 33, 52, 493, DateTimeKind.Utc).AddTicks(3022) });

            migrationBuilder.UpdateData(
                table: "Skills",
                keyColumn: "SkillId",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 2, 4, 33, 52, 493, DateTimeKind.Utc).AddTicks(3024), new DateTime(2025, 6, 2, 4, 33, 52, 493, DateTimeKind.Utc).AddTicks(3024) });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 2, 4, 33, 52, 493, DateTimeKind.Utc).AddTicks(3069), new DateTime(2025, 6, 2, 4, 33, 52, 493, DateTimeKind.Utc).AddTicks(3069) });
        }
    }
}
