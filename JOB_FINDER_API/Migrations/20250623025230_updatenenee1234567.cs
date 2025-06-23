using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace JOB_FINDER_API.Migrations
{
    /// <inheritdoc />
    public partial class updatenenee1234567 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AboutMes_CandidateProfiles_CandidateProfileId",
                table: "AboutMes");

            migrationBuilder.DropForeignKey(
                name: "FK_Awards_CandidateProfiles_CandidateProfileId",
                table: "Awards");

            migrationBuilder.DropForeignKey(
                name: "FK_Certificates_CandidateProfiles_CandidateProfileId",
                table: "Certificates");

            migrationBuilder.DropForeignKey(
                name: "FK_Educations_CandidateProfiles_CandidateProfileId",
                table: "Educations");

            migrationBuilder.DropForeignKey(
                name: "FK_ForeignLanguages_CandidateProfiles_CandidateProfileId",
                table: "ForeignLanguages");

            migrationBuilder.DropForeignKey(
                name: "FK_HighlightProjects_CandidateProfiles_CandidateProfileId",
                table: "HighlightProjects");

            migrationBuilder.DropForeignKey(
                name: "FK_Messages_Jobs_RelatedJobId",
                table: "Messages");

            migrationBuilder.DropForeignKey(
                name: "FK_Skills_CandidateProfiles_CandidateProfileId",
                table: "Skills");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkExperiences_CandidateProfiles_CandidateProfileId",
                table: "WorkExperiences");

            migrationBuilder.DropIndex(
                name: "IX_Messages_RelatedJobId",
                table: "Messages");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CandidateProfiles",
                table: "CandidateProfiles");

            migrationBuilder.DropIndex(
                name: "IX_CandidateProfiles_UserId",
                table: "CandidateProfiles");

            migrationBuilder.DropColumn(
                name: "RelatedJobId",
                table: "Messages");

            migrationBuilder.AddColumn<float>(
                name: "DescriptionWeight",
                table: "Jobs",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "EducationWeight",
                table: "Jobs",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "ExperienceWeight",
                table: "Jobs",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "SkillsWeight",
                table: "Jobs",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AlterColumn<int>(
                name: "CandidateProfileId",
                table: "CandidateProfiles",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .OldAnnotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<float>(
                name: "SimilarityScore",
                table: "Applications",
                type: "real",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_CandidateProfiles",
                table: "CandidateProfiles",
                column: "UserId");

            migrationBuilder.CreateTable(
                name: "Embeddings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Text = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Model = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Vector = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Embeddings", x => x.Id);
                });

            migrationBuilder.UpdateData(
                table: "Industries",
                keyColumn: "IndustryId",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 23, 2, 52, 28, 992, DateTimeKind.Utc).AddTicks(9263), new DateTime(2025, 6, 23, 2, 52, 28, 992, DateTimeKind.Utc).AddTicks(9264) });

            migrationBuilder.UpdateData(
                table: "Industries",
                keyColumn: "IndustryId",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 23, 2, 52, 28, 992, DateTimeKind.Utc).AddTicks(9266), new DateTime(2025, 6, 23, 2, 52, 28, 992, DateTimeKind.Utc).AddTicks(9266) });

            migrationBuilder.UpdateData(
                table: "JobTypes",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 23, 2, 52, 28, 992, DateTimeKind.Utc).AddTicks(9291), new DateTime(2025, 6, 23, 2, 52, 28, 992, DateTimeKind.Utc).AddTicks(9292) });

            migrationBuilder.UpdateData(
                table: "JobTypes",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 23, 2, 52, 28, 992, DateTimeKind.Utc).AddTicks(9294), new DateTime(2025, 6, 23, 2, 52, 28, 992, DateTimeKind.Utc).AddTicks(9294) });

            migrationBuilder.UpdateData(
                table: "JobTypes",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 23, 2, 52, 28, 992, DateTimeKind.Utc).AddTicks(9296), new DateTime(2025, 6, 23, 2, 52, 28, 992, DateTimeKind.Utc).AddTicks(9296) });

            migrationBuilder.UpdateData(
                table: "Levels",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 23, 2, 52, 28, 992, DateTimeKind.Utc).AddTicks(9321), new DateTime(2025, 6, 23, 2, 52, 28, 992, DateTimeKind.Utc).AddTicks(9321) });

            migrationBuilder.UpdateData(
                table: "Levels",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 23, 2, 52, 28, 992, DateTimeKind.Utc).AddTicks(9323), new DateTime(2025, 6, 23, 2, 52, 28, 992, DateTimeKind.Utc).AddTicks(9323) });

            migrationBuilder.UpdateData(
                table: "Levels",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 23, 2, 52, 28, 992, DateTimeKind.Utc).AddTicks(9325), new DateTime(2025, 6, 23, 2, 52, 28, 992, DateTimeKind.Utc).AddTicks(9325) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 23, 2, 52, 28, 992, DateTimeKind.Utc).AddTicks(9078), new DateTime(2025, 6, 23, 2, 52, 28, 992, DateTimeKind.Utc).AddTicks(9081) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 23, 2, 52, 28, 992, DateTimeKind.Utc).AddTicks(9090), new DateTime(2025, 6, 23, 2, 52, 28, 992, DateTimeKind.Utc).AddTicks(9091) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 23, 2, 52, 28, 992, DateTimeKind.Utc).AddTicks(9092), new DateTime(2025, 6, 23, 2, 52, 28, 992, DateTimeKind.Utc).AddTicks(9092) });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "CreatedAt", "Email", "FullName", "Image", "IsActive", "Password", "Phone", "RoleId", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 6, 23, 2, 52, 28, 992, DateTimeKind.Utc).AddTicks(9466), "tinhadmin@gmail.com", "tinh", null, true, "123", "0123456789", 1, new DateTime(2025, 6, 23, 2, 52, 28, 992, DateTimeKind.Utc).AddTicks(9467) },
                    { 2, new DateTime(2025, 6, 23, 2, 52, 28, 992, DateTimeKind.Utc).AddTicks(9471), "contact@techcorp.com", "Tech Corp", null, true, "123", "0987654321", 2, new DateTime(2025, 6, 23, 2, 52, 28, 992, DateTimeKind.Utc).AddTicks(9471) },
                    { 3, new DateTime(2025, 6, 23, 2, 52, 28, 992, DateTimeKind.Utc).AddTicks(9474), "admin@jobfinder.com", "Admin User", null, true, "123", "0912345678", 3, new DateTime(2025, 6, 23, 2, 52, 28, 992, DateTimeKind.Utc).AddTicks(9474) }
                });

            migrationBuilder.InsertData(
                table: "CandidateProfiles",
                columns: new[] { "UserId", "Address", "CandidateProfileId", "City", "Dob", "Gender", "JobTitle", "PersonalLink", "Province" },
                values: new object[] { 1, "123 Main Street", 0, "Ho Chi Minh City", new DateTime(1995, 5, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "Male", "Software Developer", null, "Ho Chi Minh" });

            migrationBuilder.InsertData(
                table: "CompanyProfile",
                columns: new[] { "UserId", "CompanyName", "CompanyProfileDescription", "Contact", "ImageLogoLgr", "IndustryId", "IsActive", "IsVerified", "Location", "TeamSize", "UrlCompanyLogo", "Website" },
                values: new object[] { 2, "Tech Corp", "A leading tech company specializing in software solutions.", "contact@techcorp.com", "https://res.cloudinary.com/dzf0ccons/image/upload/v1748267504/image_user/bfltymsad63wfkyp3bvl.jpg", 1, true, true, "123 Tech Street, District 1, Ho Chi Minh City", "50-100 employees", "https://res.cloudinary.com/dzf0ccons/image/upload/v1748267504/image_user/bfltymsad63wfkyp3bvl.jpg", "https://techcorp.com" });

            migrationBuilder.InsertData(
                table: "Experiences",
                columns: new[] { "Id", "CreatedAt", "ExperienceName", "UpdatedAt", "UserId" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 6, 23, 2, 52, 28, 992, DateTimeKind.Utc).AddTicks(9228), "Less than 1 year", new DateTime(2025, 6, 23, 2, 52, 28, 992, DateTimeKind.Utc).AddTicks(9229), 1 },
                    { 2, new DateTime(2025, 6, 23, 2, 52, 28, 992, DateTimeKind.Utc).AddTicks(9231), "1-3 years", new DateTime(2025, 6, 23, 2, 52, 28, 992, DateTimeKind.Utc).AddTicks(9231), 1 },
                    { 3, new DateTime(2025, 6, 23, 2, 52, 28, 992, DateTimeKind.Utc).AddTicks(9233), "More than 3 years", new DateTime(2025, 6, 23, 2, 52, 28, 992, DateTimeKind.Utc).AddTicks(9233), 1 }
                });

            migrationBuilder.InsertData(
                table: "Jobs",
                columns: new[] { "JobId", "AddressDetail", "CompanyId", "CreatedAt", "DeactivatedByAdmin", "Description", "DescriptionWeight", "Education", "EducationWeight", "ExperienceLevelId", "ExperienceWeight", "ExpiryDate", "IndustryId", "IsSalaryNegotiable", "JobTypeId", "LevelId", "MaxSalary", "MinSalary", "ProvinceName", "SkillsWeight", "Status", "TimeEnd", "TimeStart", "Title", "UpdatedAt", "YourExperience", "YourSkill" },
                values: new object[,]
                {
                    { 1, "123 Tech Street, District 1", 2, new DateTime(2025, 6, 23, 2, 52, 28, 992, DateTimeKind.Utc).AddTicks(9428), false, "Develop and maintain web applications using C# and JavaScript.", 0f, "Bachelor's Degree in Computer Science", 0f, 2, 0f, new DateTime(2025, 7, 23, 2, 52, 28, 992, DateTimeKind.Utc).AddTicks(9419), 1, true, 1, 2, 55000, 45000, "Ho Chi Minh City", 0f, 1, new DateTime(2025, 7, 23, 2, 52, 28, 992, DateTimeKind.Utc).AddTicks(9426), new DateTime(2025, 6, 23, 2, 52, 28, 992, DateTimeKind.Utc).AddTicks(9426), "Junior Software Engineer", new DateTime(2025, 6, 23, 2, 52, 28, 992, DateTimeKind.Utc).AddTicks(9428), "1+ years working with web development", "C#, .NET, JavaScript, SQL" },
                    { 2, "456 Finance Avenue, Ba Dinh", 2, new DateTime(2025, 6, 23, 2, 52, 28, 992, DateTimeKind.Utc).AddTicks(9435), false, "Analyze financial data and generate reports.", 0f, "Master's Degree in Finance or Data Science", 0f, 4, 0f, new DateTime(2025, 8, 7, 2, 52, 28, 992, DateTimeKind.Utc).AddTicks(9432), 2, false, 3, 3, 85000, 75000, "Hanoi", 0f, 0, new DateTime(2025, 8, 7, 2, 52, 28, 992, DateTimeKind.Utc).AddTicks(9433), new DateTime(2025, 6, 23, 2, 52, 28, 992, DateTimeKind.Utc).AddTicks(9433), "Senior Data Analyst", new DateTime(2025, 6, 23, 2, 52, 28, 992, DateTimeKind.Utc).AddTicks(9435), "5+ years of data analysis experience", "SQL, Python, Excel, Power BI" }
                });

            migrationBuilder.AddForeignKey(
                name: "FK_AboutMes_CandidateProfiles_CandidateProfileId",
                table: "AboutMes",
                column: "CandidateProfileId",
                principalTable: "CandidateProfiles",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Awards_CandidateProfiles_CandidateProfileId",
                table: "Awards",
                column: "CandidateProfileId",
                principalTable: "CandidateProfiles",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Certificates_CandidateProfiles_CandidateProfileId",
                table: "Certificates",
                column: "CandidateProfileId",
                principalTable: "CandidateProfiles",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Educations_CandidateProfiles_CandidateProfileId",
                table: "Educations",
                column: "CandidateProfileId",
                principalTable: "CandidateProfiles",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ForeignLanguages_CandidateProfiles_CandidateProfileId",
                table: "ForeignLanguages",
                column: "CandidateProfileId",
                principalTable: "CandidateProfiles",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_HighlightProjects_CandidateProfiles_CandidateProfileId",
                table: "HighlightProjects",
                column: "CandidateProfileId",
                principalTable: "CandidateProfiles",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Skills_CandidateProfiles_CandidateProfileId",
                table: "Skills",
                column: "CandidateProfileId",
                principalTable: "CandidateProfiles",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_WorkExperiences_CandidateProfiles_CandidateProfileId",
                table: "WorkExperiences",
                column: "CandidateProfileId",
                principalTable: "CandidateProfiles",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AboutMes_CandidateProfiles_CandidateProfileId",
                table: "AboutMes");

            migrationBuilder.DropForeignKey(
                name: "FK_Awards_CandidateProfiles_CandidateProfileId",
                table: "Awards");

            migrationBuilder.DropForeignKey(
                name: "FK_Certificates_CandidateProfiles_CandidateProfileId",
                table: "Certificates");

            migrationBuilder.DropForeignKey(
                name: "FK_Educations_CandidateProfiles_CandidateProfileId",
                table: "Educations");

            migrationBuilder.DropForeignKey(
                name: "FK_ForeignLanguages_CandidateProfiles_CandidateProfileId",
                table: "ForeignLanguages");

            migrationBuilder.DropForeignKey(
                name: "FK_HighlightProjects_CandidateProfiles_CandidateProfileId",
                table: "HighlightProjects");

            migrationBuilder.DropForeignKey(
                name: "FK_Skills_CandidateProfiles_CandidateProfileId",
                table: "Skills");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkExperiences_CandidateProfiles_CandidateProfileId",
                table: "WorkExperiences");

            migrationBuilder.DropTable(
                name: "Embeddings");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CandidateProfiles",
                table: "CandidateProfiles");

            migrationBuilder.DeleteData(
                table: "CandidateProfiles",
                keyColumn: "UserId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "CompanyProfile",
                keyColumn: "UserId",
                keyValue: 2);

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
                table: "Users",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DropColumn(
                name: "DescriptionWeight",
                table: "Jobs");

            migrationBuilder.DropColumn(
                name: "EducationWeight",
                table: "Jobs");

            migrationBuilder.DropColumn(
                name: "ExperienceWeight",
                table: "Jobs");

            migrationBuilder.DropColumn(
                name: "SkillsWeight",
                table: "Jobs");

            migrationBuilder.DropColumn(
                name: "SimilarityScore",
                table: "Applications");

            migrationBuilder.AddColumn<int>(
                name: "RelatedJobId",
                table: "Messages",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "CandidateProfileId",
                table: "CandidateProfiles",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CandidateProfiles",
                table: "CandidateProfiles",
                column: "CandidateProfileId");

            migrationBuilder.UpdateData(
                table: "Industries",
                keyColumn: "IndustryId",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 18, 7, 44, 12, 397, DateTimeKind.Utc).AddTicks(7801), new DateTime(2025, 6, 18, 7, 44, 12, 397, DateTimeKind.Utc).AddTicks(7802) });

            migrationBuilder.UpdateData(
                table: "Industries",
                keyColumn: "IndustryId",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 18, 7, 44, 12, 397, DateTimeKind.Utc).AddTicks(7804), new DateTime(2025, 6, 18, 7, 44, 12, 397, DateTimeKind.Utc).AddTicks(7804) });

            migrationBuilder.UpdateData(
                table: "JobTypes",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 18, 7, 44, 12, 397, DateTimeKind.Utc).AddTicks(7838), new DateTime(2025, 6, 18, 7, 44, 12, 397, DateTimeKind.Utc).AddTicks(7838) });

            migrationBuilder.UpdateData(
                table: "JobTypes",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 18, 7, 44, 12, 397, DateTimeKind.Utc).AddTicks(7840), new DateTime(2025, 6, 18, 7, 44, 12, 397, DateTimeKind.Utc).AddTicks(7841) });

            migrationBuilder.UpdateData(
                table: "JobTypes",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 18, 7, 44, 12, 397, DateTimeKind.Utc).AddTicks(7842), new DateTime(2025, 6, 18, 7, 44, 12, 397, DateTimeKind.Utc).AddTicks(7843) });

            migrationBuilder.UpdateData(
                table: "Levels",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 18, 7, 44, 12, 397, DateTimeKind.Utc).AddTicks(7871), new DateTime(2025, 6, 18, 7, 44, 12, 397, DateTimeKind.Utc).AddTicks(7872) });

            migrationBuilder.UpdateData(
                table: "Levels",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 18, 7, 44, 12, 397, DateTimeKind.Utc).AddTicks(7874), new DateTime(2025, 6, 18, 7, 44, 12, 397, DateTimeKind.Utc).AddTicks(7874) });

            migrationBuilder.UpdateData(
                table: "Levels",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 18, 7, 44, 12, 397, DateTimeKind.Utc).AddTicks(7876), new DateTime(2025, 6, 18, 7, 44, 12, 397, DateTimeKind.Utc).AddTicks(7876) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 18, 7, 44, 12, 397, DateTimeKind.Utc).AddTicks(7624), new DateTime(2025, 6, 18, 7, 44, 12, 397, DateTimeKind.Utc).AddTicks(7628) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 18, 7, 44, 12, 397, DateTimeKind.Utc).AddTicks(7633), new DateTime(2025, 6, 18, 7, 44, 12, 397, DateTimeKind.Utc).AddTicks(7633) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 18, 7, 44, 12, 397, DateTimeKind.Utc).AddTicks(7634), new DateTime(2025, 6, 18, 7, 44, 12, 397, DateTimeKind.Utc).AddTicks(7635) });

            migrationBuilder.CreateIndex(
                name: "IX_Messages_RelatedJobId",
                table: "Messages",
                column: "RelatedJobId");

            migrationBuilder.CreateIndex(
                name: "IX_CandidateProfiles_UserId",
                table: "CandidateProfiles",
                column: "UserId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_AboutMes_CandidateProfiles_CandidateProfileId",
                table: "AboutMes",
                column: "CandidateProfileId",
                principalTable: "CandidateProfiles",
                principalColumn: "CandidateProfileId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Awards_CandidateProfiles_CandidateProfileId",
                table: "Awards",
                column: "CandidateProfileId",
                principalTable: "CandidateProfiles",
                principalColumn: "CandidateProfileId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Certificates_CandidateProfiles_CandidateProfileId",
                table: "Certificates",
                column: "CandidateProfileId",
                principalTable: "CandidateProfiles",
                principalColumn: "CandidateProfileId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Educations_CandidateProfiles_CandidateProfileId",
                table: "Educations",
                column: "CandidateProfileId",
                principalTable: "CandidateProfiles",
                principalColumn: "CandidateProfileId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ForeignLanguages_CandidateProfiles_CandidateProfileId",
                table: "ForeignLanguages",
                column: "CandidateProfileId",
                principalTable: "CandidateProfiles",
                principalColumn: "CandidateProfileId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_HighlightProjects_CandidateProfiles_CandidateProfileId",
                table: "HighlightProjects",
                column: "CandidateProfileId",
                principalTable: "CandidateProfiles",
                principalColumn: "CandidateProfileId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_Jobs_RelatedJobId",
                table: "Messages",
                column: "RelatedJobId",
                principalTable: "Jobs",
                principalColumn: "JobId",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Skills_CandidateProfiles_CandidateProfileId",
                table: "Skills",
                column: "CandidateProfileId",
                principalTable: "CandidateProfiles",
                principalColumn: "CandidateProfileId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_WorkExperiences_CandidateProfiles_CandidateProfileId",
                table: "WorkExperiences",
                column: "CandidateProfileId",
                principalTable: "CandidateProfiles",
                principalColumn: "CandidateProfileId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
