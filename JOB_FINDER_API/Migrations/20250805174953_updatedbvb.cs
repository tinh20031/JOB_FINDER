using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JOB_FINDER_API.Migrations
{
    /// <inheritdoc />
    public partial class updatedbvb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "JobTimeStart",
                table: "Notifications",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "CompanySubscriptionTypes",
                keyColumn: "CompanySubscriptionTypeId",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 6, 0, 49, 51, 226, DateTimeKind.Unspecified).AddTicks(7345), new DateTime(2025, 8, 6, 0, 49, 51, 226, DateTimeKind.Unspecified).AddTicks(7360) });

            migrationBuilder.UpdateData(
                table: "CompanySubscriptionTypes",
                keyColumn: "CompanySubscriptionTypeId",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 6, 0, 49, 51, 226, DateTimeKind.Unspecified).AddTicks(7365), new DateTime(2025, 8, 6, 0, 49, 51, 226, DateTimeKind.Unspecified).AddTicks(7366) });

            migrationBuilder.UpdateData(
                table: "CompanySubscriptionTypes",
                keyColumn: "CompanySubscriptionTypeId",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 6, 0, 49, 51, 226, DateTimeKind.Unspecified).AddTicks(7370), new DateTime(2025, 8, 6, 0, 49, 51, 226, DateTimeKind.Unspecified).AddTicks(7371) });

            migrationBuilder.UpdateData(
                table: "Industries",
                keyColumn: "IndustryId",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 6, 0, 49, 51, 220, DateTimeKind.Unspecified).AddTicks(596), new DateTime(2025, 8, 6, 0, 49, 51, 220, DateTimeKind.Unspecified).AddTicks(621) });

            migrationBuilder.UpdateData(
                table: "Industries",
                keyColumn: "IndustryId",
                keyValue: 4,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 6, 0, 49, 51, 220, DateTimeKind.Unspecified).AddTicks(624), new DateTime(2025, 8, 6, 0, 49, 51, 220, DateTimeKind.Unspecified).AddTicks(626) });

            migrationBuilder.UpdateData(
                table: "Industries",
                keyColumn: "IndustryId",
                keyValue: 5,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 6, 0, 49, 51, 220, DateTimeKind.Unspecified).AddTicks(628), new DateTime(2025, 8, 6, 0, 49, 51, 220, DateTimeKind.Unspecified).AddTicks(630) });

            migrationBuilder.UpdateData(
                table: "Industries",
                keyColumn: "IndustryId",
                keyValue: 6,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 6, 0, 49, 51, 220, DateTimeKind.Unspecified).AddTicks(632), new DateTime(2025, 8, 6, 0, 49, 51, 220, DateTimeKind.Unspecified).AddTicks(633) });

            migrationBuilder.UpdateData(
                table: "Industries",
                keyColumn: "IndustryId",
                keyValue: 7,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 6, 0, 49, 51, 220, DateTimeKind.Unspecified).AddTicks(636), new DateTime(2025, 8, 6, 0, 49, 51, 220, DateTimeKind.Unspecified).AddTicks(637) });

            migrationBuilder.UpdateData(
                table: "Industries",
                keyColumn: "IndustryId",
                keyValue: 8,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 6, 0, 49, 51, 220, DateTimeKind.Unspecified).AddTicks(639), new DateTime(2025, 8, 6, 0, 49, 51, 220, DateTimeKind.Unspecified).AddTicks(640) });

            migrationBuilder.UpdateData(
                table: "Industries",
                keyColumn: "IndustryId",
                keyValue: 9,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 6, 0, 49, 51, 220, DateTimeKind.Unspecified).AddTicks(643), new DateTime(2025, 8, 6, 0, 49, 51, 220, DateTimeKind.Unspecified).AddTicks(644) });

            migrationBuilder.UpdateData(
                table: "JobTypes",
                keyColumn: "JobTypeId",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 6, 0, 49, 51, 220, DateTimeKind.Unspecified).AddTicks(690), new DateTime(2025, 8, 6, 0, 49, 51, 220, DateTimeKind.Unspecified).AddTicks(692) });

            migrationBuilder.UpdateData(
                table: "JobTypes",
                keyColumn: "JobTypeId",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 6, 0, 49, 51, 220, DateTimeKind.Unspecified).AddTicks(695), new DateTime(2025, 8, 6, 0, 49, 51, 220, DateTimeKind.Unspecified).AddTicks(696) });

            migrationBuilder.UpdateData(
                table: "JobTypes",
                keyColumn: "JobTypeId",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 6, 0, 49, 51, 220, DateTimeKind.Unspecified).AddTicks(699), new DateTime(2025, 8, 6, 0, 49, 51, 220, DateTimeKind.Unspecified).AddTicks(700) });

            migrationBuilder.UpdateData(
                table: "Levels",
                keyColumn: "LevelId",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 6, 0, 49, 51, 220, DateTimeKind.Unspecified).AddTicks(735), new DateTime(2025, 8, 6, 0, 49, 51, 220, DateTimeKind.Unspecified).AddTicks(737) });

            migrationBuilder.UpdateData(
                table: "Levels",
                keyColumn: "LevelId",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 6, 0, 49, 51, 220, DateTimeKind.Unspecified).AddTicks(741), new DateTime(2025, 8, 6, 0, 49, 51, 220, DateTimeKind.Unspecified).AddTicks(742) });

            migrationBuilder.UpdateData(
                table: "Levels",
                keyColumn: "LevelId",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 6, 0, 49, 51, 220, DateTimeKind.Unspecified).AddTicks(745), new DateTime(2025, 8, 6, 0, 49, 51, 220, DateTimeKind.Unspecified).AddTicks(746) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 5, 17, 49, 51, 220, DateTimeKind.Utc).AddTicks(393), new DateTime(2025, 8, 5, 17, 49, 51, 220, DateTimeKind.Utc).AddTicks(395) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 5, 17, 49, 51, 220, DateTimeKind.Utc).AddTicks(402), new DateTime(2025, 8, 5, 17, 49, 51, 220, DateTimeKind.Utc).AddTicks(402) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 5, 17, 49, 51, 220, DateTimeKind.Utc).AddTicks(403), new DateTime(2025, 8, 5, 17, 49, 51, 220, DateTimeKind.Utc).AddTicks(404) });

            migrationBuilder.UpdateData(
                table: "SubscriptionTypes",
                keyColumn: "SubscriptionTypeId",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 5, 17, 49, 51, 226, DateTimeKind.Utc).AddTicks(7257), new DateTime(2025, 8, 5, 17, 49, 51, 226, DateTimeKind.Utc).AddTicks(7259) });

            migrationBuilder.UpdateData(
                table: "SubscriptionTypes",
                keyColumn: "SubscriptionTypeId",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 5, 17, 49, 51, 226, DateTimeKind.Utc).AddTicks(7267), new DateTime(2025, 8, 5, 17, 49, 51, 226, DateTimeKind.Utc).AddTicks(7267) });

            migrationBuilder.UpdateData(
                table: "SubscriptionTypes",
                keyColumn: "SubscriptionTypeId",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 5, 17, 49, 51, 226, DateTimeKind.Utc).AddTicks(7272), new DateTime(2025, 8, 5, 17, 49, 51, 226, DateTimeKind.Utc).AddTicks(7272) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "JobTimeStart",
                table: "Notifications");

            migrationBuilder.UpdateData(
                table: "CompanySubscriptionTypes",
                keyColumn: "CompanySubscriptionTypeId",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 5, 21, 13, 58, 708, DateTimeKind.Unspecified).AddTicks(2135), new DateTime(2025, 8, 5, 21, 13, 58, 708, DateTimeKind.Unspecified).AddTicks(2160) });

            migrationBuilder.UpdateData(
                table: "CompanySubscriptionTypes",
                keyColumn: "CompanySubscriptionTypeId",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 5, 21, 13, 58, 708, DateTimeKind.Unspecified).AddTicks(2165), new DateTime(2025, 8, 5, 21, 13, 58, 708, DateTimeKind.Unspecified).AddTicks(2166) });

            migrationBuilder.UpdateData(
                table: "CompanySubscriptionTypes",
                keyColumn: "CompanySubscriptionTypeId",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 5, 21, 13, 58, 708, DateTimeKind.Unspecified).AddTicks(2170), new DateTime(2025, 8, 5, 21, 13, 58, 708, DateTimeKind.Unspecified).AddTicks(2171) });

            migrationBuilder.UpdateData(
                table: "Industries",
                keyColumn: "IndustryId",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 5, 21, 13, 58, 701, DateTimeKind.Unspecified).AddTicks(4922), new DateTime(2025, 8, 5, 21, 13, 58, 701, DateTimeKind.Unspecified).AddTicks(4948) });

            migrationBuilder.UpdateData(
                table: "Industries",
                keyColumn: "IndustryId",
                keyValue: 4,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 5, 21, 13, 58, 701, DateTimeKind.Unspecified).AddTicks(4951), new DateTime(2025, 8, 5, 21, 13, 58, 701, DateTimeKind.Unspecified).AddTicks(4952) });

            migrationBuilder.UpdateData(
                table: "Industries",
                keyColumn: "IndustryId",
                keyValue: 5,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 5, 21, 13, 58, 701, DateTimeKind.Unspecified).AddTicks(4955), new DateTime(2025, 8, 5, 21, 13, 58, 701, DateTimeKind.Unspecified).AddTicks(4956) });

            migrationBuilder.UpdateData(
                table: "Industries",
                keyColumn: "IndustryId",
                keyValue: 6,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 5, 21, 13, 58, 701, DateTimeKind.Unspecified).AddTicks(4959), new DateTime(2025, 8, 5, 21, 13, 58, 701, DateTimeKind.Unspecified).AddTicks(4960) });

            migrationBuilder.UpdateData(
                table: "Industries",
                keyColumn: "IndustryId",
                keyValue: 7,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 5, 21, 13, 58, 701, DateTimeKind.Unspecified).AddTicks(4963), new DateTime(2025, 8, 5, 21, 13, 58, 701, DateTimeKind.Unspecified).AddTicks(4964) });

            migrationBuilder.UpdateData(
                table: "Industries",
                keyColumn: "IndustryId",
                keyValue: 8,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 5, 21, 13, 58, 701, DateTimeKind.Unspecified).AddTicks(4967), new DateTime(2025, 8, 5, 21, 13, 58, 701, DateTimeKind.Unspecified).AddTicks(4968) });

            migrationBuilder.UpdateData(
                table: "Industries",
                keyColumn: "IndustryId",
                keyValue: 9,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 5, 21, 13, 58, 701, DateTimeKind.Unspecified).AddTicks(4970), new DateTime(2025, 8, 5, 21, 13, 58, 701, DateTimeKind.Unspecified).AddTicks(4971) });

            migrationBuilder.UpdateData(
                table: "JobTypes",
                keyColumn: "JobTypeId",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 5, 21, 13, 58, 701, DateTimeKind.Unspecified).AddTicks(5019), new DateTime(2025, 8, 5, 21, 13, 58, 701, DateTimeKind.Unspecified).AddTicks(5020) });

            migrationBuilder.UpdateData(
                table: "JobTypes",
                keyColumn: "JobTypeId",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 5, 21, 13, 58, 701, DateTimeKind.Unspecified).AddTicks(5023), new DateTime(2025, 8, 5, 21, 13, 58, 701, DateTimeKind.Unspecified).AddTicks(5025) });

            migrationBuilder.UpdateData(
                table: "JobTypes",
                keyColumn: "JobTypeId",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 5, 21, 13, 58, 701, DateTimeKind.Unspecified).AddTicks(5027), new DateTime(2025, 8, 5, 21, 13, 58, 701, DateTimeKind.Unspecified).AddTicks(5029) });

            migrationBuilder.UpdateData(
                table: "Levels",
                keyColumn: "LevelId",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 5, 21, 13, 58, 701, DateTimeKind.Unspecified).AddTicks(5060), new DateTime(2025, 8, 5, 21, 13, 58, 701, DateTimeKind.Unspecified).AddTicks(5062) });

            migrationBuilder.UpdateData(
                table: "Levels",
                keyColumn: "LevelId",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 5, 21, 13, 58, 701, DateTimeKind.Unspecified).AddTicks(5068), new DateTime(2025, 8, 5, 21, 13, 58, 701, DateTimeKind.Unspecified).AddTicks(5069) });

            migrationBuilder.UpdateData(
                table: "Levels",
                keyColumn: "LevelId",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 5, 21, 13, 58, 701, DateTimeKind.Unspecified).AddTicks(5071), new DateTime(2025, 8, 5, 21, 13, 58, 701, DateTimeKind.Unspecified).AddTicks(5073) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 5, 14, 13, 58, 701, DateTimeKind.Utc).AddTicks(4703), new DateTime(2025, 8, 5, 14, 13, 58, 701, DateTimeKind.Utc).AddTicks(4706) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 5, 14, 13, 58, 701, DateTimeKind.Utc).AddTicks(4714), new DateTime(2025, 8, 5, 14, 13, 58, 701, DateTimeKind.Utc).AddTicks(4714) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 5, 14, 13, 58, 701, DateTimeKind.Utc).AddTicks(4715), new DateTime(2025, 8, 5, 14, 13, 58, 701, DateTimeKind.Utc).AddTicks(4716) });

            migrationBuilder.UpdateData(
                table: "SubscriptionTypes",
                keyColumn: "SubscriptionTypeId",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 5, 14, 13, 58, 708, DateTimeKind.Utc).AddTicks(2016), new DateTime(2025, 8, 5, 14, 13, 58, 708, DateTimeKind.Utc).AddTicks(2018) });

            migrationBuilder.UpdateData(
                table: "SubscriptionTypes",
                keyColumn: "SubscriptionTypeId",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 5, 14, 13, 58, 708, DateTimeKind.Utc).AddTicks(2030), new DateTime(2025, 8, 5, 14, 13, 58, 708, DateTimeKind.Utc).AddTicks(2030) });

            migrationBuilder.UpdateData(
                table: "SubscriptionTypes",
                keyColumn: "SubscriptionTypeId",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 5, 14, 13, 58, 708, DateTimeKind.Utc).AddTicks(2035), new DateTime(2025, 8, 5, 14, 13, 58, 708, DateTimeKind.Utc).AddTicks(2035) });
        }
    }
}
