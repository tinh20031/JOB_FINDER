using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JOB_FINDER_API.Migrations
{
    /// <inheritdoc />
    public partial class UpdateDbUserFavoriteCompany : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
    name: "UserFavoriteCompanies",
    columns: table => new
    {
        Id = table.Column<int>(nullable: false)
            .Annotation("SqlServer:Identity", "1, 1"),
        UserId = table.Column<int>(nullable: false),
        CompanyId = table.Column<int>(nullable: false),
        CreatedAt = table.Column<DateTime>(nullable: false)
    },
    constraints: table =>
    {
        table.PrimaryKey("PK_UserFavoriteCompanies", x => x.Id);
        table.ForeignKey(
            name: "FK_UserFavoriteCompanies_Users_UserId",
            column: x => x.UserId,
            principalTable: "Users",
            principalColumn: "Id",
            onDelete: ReferentialAction.Cascade);
        table.ForeignKey(
            name: "FK_UserFavoriteCompanies_Users_CompanyId",
            column: x => x.CompanyId,
            principalTable: "Users",
            principalColumn: "Id",
            onDelete: ReferentialAction.NoAction); // <--- Sửa ở đây
    });

            migrationBuilder.UpdateData(
                table: "Experiences",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 10, 11, 58, 54, 677, DateTimeKind.Utc).AddTicks(6952), new DateTime(2025, 6, 10, 11, 58, 54, 677, DateTimeKind.Utc).AddTicks(6953) });

            migrationBuilder.UpdateData(
                table: "Experiences",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 10, 11, 58, 54, 677, DateTimeKind.Utc).AddTicks(6955), new DateTime(2025, 6, 10, 11, 58, 54, 677, DateTimeKind.Utc).AddTicks(6955) });

            migrationBuilder.UpdateData(
                table: "Experiences",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 10, 11, 58, 54, 677, DateTimeKind.Utc).AddTicks(6956), new DateTime(2025, 6, 10, 11, 58, 54, 677, DateTimeKind.Utc).AddTicks(6957) });

            migrationBuilder.UpdateData(
                table: "Industries",
                keyColumn: "IndustryId",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 10, 11, 58, 54, 677, DateTimeKind.Utc).AddTicks(6990), new DateTime(2025, 6, 10, 11, 58, 54, 677, DateTimeKind.Utc).AddTicks(6992) });

            migrationBuilder.UpdateData(
                table: "Industries",
                keyColumn: "IndustryId",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 10, 11, 58, 54, 677, DateTimeKind.Utc).AddTicks(6994), new DateTime(2025, 6, 10, 11, 58, 54, 677, DateTimeKind.Utc).AddTicks(6994) });

            migrationBuilder.UpdateData(
                table: "JobTypes",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 10, 11, 58, 54, 677, DateTimeKind.Utc).AddTicks(7029), new DateTime(2025, 6, 10, 11, 58, 54, 677, DateTimeKind.Utc).AddTicks(7031) });

            migrationBuilder.UpdateData(
                table: "JobTypes",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 10, 11, 58, 54, 677, DateTimeKind.Utc).AddTicks(7033), new DateTime(2025, 6, 10, 11, 58, 54, 677, DateTimeKind.Utc).AddTicks(7034) });

            migrationBuilder.UpdateData(
                table: "JobTypes",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 10, 11, 58, 54, 677, DateTimeKind.Utc).AddTicks(7035), new DateTime(2025, 6, 10, 11, 58, 54, 677, DateTimeKind.Utc).AddTicks(7036) });

            migrationBuilder.UpdateData(
                table: "Jobs",
                keyColumn: "JobId",
                keyValue: 1,
                columns: new[] { "CreatedAt", "ExpiryDate", "TimeEnd", "TimeStart", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 10, 11, 58, 54, 677, DateTimeKind.Utc).AddTicks(7207), new DateTime(2025, 7, 10, 11, 58, 54, 677, DateTimeKind.Utc).AddTicks(7199), new DateTime(2025, 7, 10, 11, 58, 54, 677, DateTimeKind.Utc).AddTicks(7206), new DateTime(2025, 6, 10, 11, 58, 54, 677, DateTimeKind.Utc).AddTicks(7205), new DateTime(2025, 6, 10, 11, 58, 54, 677, DateTimeKind.Utc).AddTicks(7207) });

            migrationBuilder.UpdateData(
                table: "Jobs",
                keyColumn: "JobId",
                keyValue: 2,
                columns: new[] { "CreatedAt", "ExpiryDate", "TimeEnd", "TimeStart", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 10, 11, 58, 54, 677, DateTimeKind.Utc).AddTicks(7212), new DateTime(2025, 7, 25, 11, 58, 54, 677, DateTimeKind.Utc).AddTicks(7210), new DateTime(2025, 7, 25, 11, 58, 54, 677, DateTimeKind.Utc).AddTicks(7211), new DateTime(2025, 6, 10, 11, 58, 54, 677, DateTimeKind.Utc).AddTicks(7211), new DateTime(2025, 6, 10, 11, 58, 54, 677, DateTimeKind.Utc).AddTicks(7212) });

            migrationBuilder.UpdateData(
                table: "Levels",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 10, 11, 58, 54, 677, DateTimeKind.Utc).AddTicks(7064), new DateTime(2025, 6, 10, 11, 58, 54, 677, DateTimeKind.Utc).AddTicks(7065) });

            migrationBuilder.UpdateData(
                table: "Levels",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 10, 11, 58, 54, 677, DateTimeKind.Utc).AddTicks(7068), new DateTime(2025, 6, 10, 11, 58, 54, 677, DateTimeKind.Utc).AddTicks(7068) });

            migrationBuilder.UpdateData(
                table: "Levels",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 10, 11, 58, 54, 677, DateTimeKind.Utc).AddTicks(7104), new DateTime(2025, 6, 10, 11, 58, 54, 677, DateTimeKind.Utc).AddTicks(7104) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 10, 11, 58, 54, 677, DateTimeKind.Utc).AddTicks(6784), new DateTime(2025, 6, 10, 11, 58, 54, 677, DateTimeKind.Utc).AddTicks(6787) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 10, 11, 58, 54, 677, DateTimeKind.Utc).AddTicks(6793), new DateTime(2025, 6, 10, 11, 58, 54, 677, DateTimeKind.Utc).AddTicks(6793) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 10, 11, 58, 54, 677, DateTimeKind.Utc).AddTicks(6794), new DateTime(2025, 6, 10, 11, 58, 54, 677, DateTimeKind.Utc).AddTicks(6795) });

            migrationBuilder.UpdateData(
                table: "Skills",
                keyColumn: "SkillId",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 10, 11, 58, 54, 677, DateTimeKind.Utc).AddTicks(7129), new DateTime(2025, 6, 10, 11, 58, 54, 677, DateTimeKind.Utc).AddTicks(7130) });

            migrationBuilder.UpdateData(
                table: "Skills",
                keyColumn: "SkillId",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 10, 11, 58, 54, 677, DateTimeKind.Utc).AddTicks(7133), new DateTime(2025, 6, 10, 11, 58, 54, 677, DateTimeKind.Utc).AddTicks(7133) });

            migrationBuilder.UpdateData(
                table: "Skills",
                keyColumn: "SkillId",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 10, 11, 58, 54, 677, DateTimeKind.Utc).AddTicks(7135), new DateTime(2025, 6, 10, 11, 58, 54, 677, DateTimeKind.Utc).AddTicks(7135) });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 10, 11, 58, 54, 677, DateTimeKind.Utc).AddTicks(7258), new DateTime(2025, 6, 10, 11, 58, 54, 677, DateTimeKind.Utc).AddTicks(7258) });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 10, 11, 58, 54, 677, DateTimeKind.Utc).AddTicks(7262), new DateTime(2025, 6, 10, 11, 58, 54, 677, DateTimeKind.Utc).AddTicks(7262) });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 10, 11, 58, 54, 677, DateTimeKind.Utc).AddTicks(7265), new DateTime(2025, 6, 10, 11, 58, 54, 677, DateTimeKind.Utc).AddTicks(7266) });

            migrationBuilder.CreateIndex(
                name: "IX_UserFavoriteCompanies_CompanyId",
                table: "UserFavoriteCompanies",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_UserFavoriteCompanies_UserId",
                table: "UserFavoriteCompanies",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserFavoriteCompanies");

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
                table: "Jobs",
                keyColumn: "JobId",
                keyValue: 1,
                columns: new[] { "CreatedAt", "ExpiryDate", "TimeEnd", "TimeStart", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 9, 3, 20, 27, 0, DateTimeKind.Utc).AddTicks(5444), new DateTime(2025, 7, 9, 3, 20, 27, 0, DateTimeKind.Utc).AddTicks(5434), new DateTime(2025, 7, 9, 3, 20, 27, 0, DateTimeKind.Utc).AddTicks(5442), new DateTime(2025, 6, 9, 3, 20, 27, 0, DateTimeKind.Utc).AddTicks(5441), new DateTime(2025, 6, 9, 3, 20, 27, 0, DateTimeKind.Utc).AddTicks(5445) });

            migrationBuilder.UpdateData(
                table: "Jobs",
                keyColumn: "JobId",
                keyValue: 2,
                columns: new[] { "CreatedAt", "ExpiryDate", "TimeEnd", "TimeStart", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 9, 3, 20, 27, 0, DateTimeKind.Utc).AddTicks(5449), new DateTime(2025, 7, 24, 3, 20, 27, 0, DateTimeKind.Utc).AddTicks(5447), new DateTime(2025, 7, 24, 3, 20, 27, 0, DateTimeKind.Utc).AddTicks(5448), new DateTime(2025, 6, 9, 3, 20, 27, 0, DateTimeKind.Utc).AddTicks(5448), new DateTime(2025, 6, 9, 3, 20, 27, 0, DateTimeKind.Utc).AddTicks(5450) });

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

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 9, 3, 20, 27, 0, DateTimeKind.Utc).AddTicks(5493), new DateTime(2025, 6, 9, 3, 20, 27, 0, DateTimeKind.Utc).AddTicks(5494) });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 9, 3, 20, 27, 0, DateTimeKind.Utc).AddTicks(5516), new DateTime(2025, 6, 9, 3, 20, 27, 0, DateTimeKind.Utc).AddTicks(5517) });
        }
    }
}
