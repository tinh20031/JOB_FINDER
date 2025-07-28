using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JOB_FINDER_API.Migrations
{
    /// <inheritdoc />
    public partial class updatePackage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsTrending",
                table: "Jobs",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "TrendingJobLimit",
                table: "CompanySubscriptionTypes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "RemainingTrendingJobPosts",
                table: "CompanySubscriptions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "CompanySubscriptionTypes",
                keyColumn: "CompanySubscriptionTypeId",
                keyValue: 1,
                columns: new[] { "CreatedAt", "TrendingJobLimit", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 27, 23, 28, 10, 761, DateTimeKind.Unspecified).AddTicks(2476), 0, new DateTime(2025, 7, 27, 23, 28, 10, 761, DateTimeKind.Unspecified).AddTicks(2489) });

            migrationBuilder.UpdateData(
                table: "CompanySubscriptionTypes",
                keyColumn: "CompanySubscriptionTypeId",
                keyValue: 2,
                columns: new[] { "CreatedAt", "TrendingJobLimit", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 27, 23, 28, 10, 761, DateTimeKind.Unspecified).AddTicks(2493), 5, new DateTime(2025, 7, 27, 23, 28, 10, 761, DateTimeKind.Unspecified).AddTicks(2495) });

            migrationBuilder.UpdateData(
                table: "CompanySubscriptionTypes",
                keyColumn: "CompanySubscriptionTypeId",
                keyValue: 3,
                columns: new[] { "CreatedAt", "TrendingJobLimit", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 27, 23, 28, 10, 761, DateTimeKind.Unspecified).AddTicks(2499), 10, new DateTime(2025, 7, 27, 23, 28, 10, 761, DateTimeKind.Unspecified).AddTicks(2500) });

            migrationBuilder.UpdateData(
                table: "Industries",
                keyColumn: "IndustryId",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 27, 23, 28, 10, 754, DateTimeKind.Unspecified).AddTicks(5412), new DateTime(2025, 7, 27, 23, 28, 10, 754, DateTimeKind.Unspecified).AddTicks(5445) });

            migrationBuilder.UpdateData(
                table: "Industries",
                keyColumn: "IndustryId",
                keyValue: 4,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 27, 23, 28, 10, 754, DateTimeKind.Unspecified).AddTicks(5448), new DateTime(2025, 7, 27, 23, 28, 10, 754, DateTimeKind.Unspecified).AddTicks(5449) });

            migrationBuilder.UpdateData(
                table: "Industries",
                keyColumn: "IndustryId",
                keyValue: 5,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 27, 23, 28, 10, 754, DateTimeKind.Unspecified).AddTicks(5452), new DateTime(2025, 7, 27, 23, 28, 10, 754, DateTimeKind.Unspecified).AddTicks(5454) });

            migrationBuilder.UpdateData(
                table: "Industries",
                keyColumn: "IndustryId",
                keyValue: 6,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 27, 23, 28, 10, 754, DateTimeKind.Unspecified).AddTicks(5456), new DateTime(2025, 7, 27, 23, 28, 10, 754, DateTimeKind.Unspecified).AddTicks(5457) });

            migrationBuilder.UpdateData(
                table: "Industries",
                keyColumn: "IndustryId",
                keyValue: 7,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 27, 23, 28, 10, 754, DateTimeKind.Unspecified).AddTicks(5459), new DateTime(2025, 7, 27, 23, 28, 10, 754, DateTimeKind.Unspecified).AddTicks(5460) });

            migrationBuilder.UpdateData(
                table: "Industries",
                keyColumn: "IndustryId",
                keyValue: 8,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 27, 23, 28, 10, 754, DateTimeKind.Unspecified).AddTicks(5463), new DateTime(2025, 7, 27, 23, 28, 10, 754, DateTimeKind.Unspecified).AddTicks(5464) });

            migrationBuilder.UpdateData(
                table: "Industries",
                keyColumn: "IndustryId",
                keyValue: 9,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 27, 23, 28, 10, 754, DateTimeKind.Unspecified).AddTicks(5466), new DateTime(2025, 7, 27, 23, 28, 10, 754, DateTimeKind.Unspecified).AddTicks(5467) });

            migrationBuilder.UpdateData(
                table: "JobTypes",
                keyColumn: "JobTypeId",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 27, 23, 28, 10, 754, DateTimeKind.Unspecified).AddTicks(5511), new DateTime(2025, 7, 27, 23, 28, 10, 754, DateTimeKind.Unspecified).AddTicks(5514) });

            migrationBuilder.UpdateData(
                table: "JobTypes",
                keyColumn: "JobTypeId",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 27, 23, 28, 10, 754, DateTimeKind.Unspecified).AddTicks(5517), new DateTime(2025, 7, 27, 23, 28, 10, 754, DateTimeKind.Unspecified).AddTicks(5518) });

            migrationBuilder.UpdateData(
                table: "JobTypes",
                keyColumn: "JobTypeId",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 27, 23, 28, 10, 754, DateTimeKind.Unspecified).AddTicks(5520), new DateTime(2025, 7, 27, 23, 28, 10, 754, DateTimeKind.Unspecified).AddTicks(5521) });

            migrationBuilder.UpdateData(
                table: "Levels",
                keyColumn: "LevelId",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 27, 23, 28, 10, 754, DateTimeKind.Unspecified).AddTicks(5588), new DateTime(2025, 7, 27, 23, 28, 10, 754, DateTimeKind.Unspecified).AddTicks(5589) });

            migrationBuilder.UpdateData(
                table: "Levels",
                keyColumn: "LevelId",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 27, 23, 28, 10, 754, DateTimeKind.Unspecified).AddTicks(5592), new DateTime(2025, 7, 27, 23, 28, 10, 754, DateTimeKind.Unspecified).AddTicks(5593) });

            migrationBuilder.UpdateData(
                table: "Levels",
                keyColumn: "LevelId",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 27, 23, 28, 10, 754, DateTimeKind.Unspecified).AddTicks(5596), new DateTime(2025, 7, 27, 23, 28, 10, 754, DateTimeKind.Unspecified).AddTicks(5597) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 27, 16, 28, 10, 754, DateTimeKind.Utc).AddTicks(5180), new DateTime(2025, 7, 27, 16, 28, 10, 754, DateTimeKind.Utc).AddTicks(5183) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 27, 16, 28, 10, 754, DateTimeKind.Utc).AddTicks(5192), new DateTime(2025, 7, 27, 16, 28, 10, 754, DateTimeKind.Utc).AddTicks(5192) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 27, 16, 28, 10, 754, DateTimeKind.Utc).AddTicks(5193), new DateTime(2025, 7, 27, 16, 28, 10, 754, DateTimeKind.Utc).AddTicks(5193) });

            migrationBuilder.UpdateData(
                table: "SubscriptionTypes",
                keyColumn: "SubscriptionTypeId",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 27, 16, 28, 10, 761, DateTimeKind.Utc).AddTicks(2391), new DateTime(2025, 7, 27, 16, 28, 10, 761, DateTimeKind.Utc).AddTicks(2392) });

            migrationBuilder.UpdateData(
                table: "SubscriptionTypes",
                keyColumn: "SubscriptionTypeId",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 27, 16, 28, 10, 761, DateTimeKind.Utc).AddTicks(2402), new DateTime(2025, 7, 27, 16, 28, 10, 761, DateTimeKind.Utc).AddTicks(2403) });

            migrationBuilder.UpdateData(
                table: "SubscriptionTypes",
                keyColumn: "SubscriptionTypeId",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 27, 16, 28, 10, 761, DateTimeKind.Utc).AddTicks(2407), new DateTime(2025, 7, 27, 16, 28, 10, 761, DateTimeKind.Utc).AddTicks(2407) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsTrending",
                table: "Jobs");

            migrationBuilder.DropColumn(
                name: "TrendingJobLimit",
                table: "CompanySubscriptionTypes");

            migrationBuilder.DropColumn(
                name: "RemainingTrendingJobPosts",
                table: "CompanySubscriptions");

            migrationBuilder.UpdateData(
                table: "CompanySubscriptionTypes",
                keyColumn: "CompanySubscriptionTypeId",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 25, 20, 58, 42, 418, DateTimeKind.Unspecified).AddTicks(1256), new DateTime(2025, 7, 25, 20, 58, 42, 418, DateTimeKind.Unspecified).AddTicks(1267) });

            migrationBuilder.UpdateData(
                table: "CompanySubscriptionTypes",
                keyColumn: "CompanySubscriptionTypeId",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 25, 20, 58, 42, 418, DateTimeKind.Unspecified).AddTicks(1271), new DateTime(2025, 7, 25, 20, 58, 42, 418, DateTimeKind.Unspecified).AddTicks(1272) });

            migrationBuilder.UpdateData(
                table: "CompanySubscriptionTypes",
                keyColumn: "CompanySubscriptionTypeId",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 25, 20, 58, 42, 418, DateTimeKind.Unspecified).AddTicks(1276), new DateTime(2025, 7, 25, 20, 58, 42, 418, DateTimeKind.Unspecified).AddTicks(1277) });

            migrationBuilder.UpdateData(
                table: "Industries",
                keyColumn: "IndustryId",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 25, 20, 58, 42, 412, DateTimeKind.Unspecified).AddTicks(3241), new DateTime(2025, 7, 25, 20, 58, 42, 412, DateTimeKind.Unspecified).AddTicks(3265) });

            migrationBuilder.UpdateData(
                table: "Industries",
                keyColumn: "IndustryId",
                keyValue: 4,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 25, 20, 58, 42, 412, DateTimeKind.Unspecified).AddTicks(3270), new DateTime(2025, 7, 25, 20, 58, 42, 412, DateTimeKind.Unspecified).AddTicks(3271) });

            migrationBuilder.UpdateData(
                table: "Industries",
                keyColumn: "IndustryId",
                keyValue: 5,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 25, 20, 58, 42, 412, DateTimeKind.Unspecified).AddTicks(3274), new DateTime(2025, 7, 25, 20, 58, 42, 412, DateTimeKind.Unspecified).AddTicks(3275) });

            migrationBuilder.UpdateData(
                table: "Industries",
                keyColumn: "IndustryId",
                keyValue: 6,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 25, 20, 58, 42, 412, DateTimeKind.Unspecified).AddTicks(3277), new DateTime(2025, 7, 25, 20, 58, 42, 412, DateTimeKind.Unspecified).AddTicks(3279) });

            migrationBuilder.UpdateData(
                table: "Industries",
                keyColumn: "IndustryId",
                keyValue: 7,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 25, 20, 58, 42, 412, DateTimeKind.Unspecified).AddTicks(3281), new DateTime(2025, 7, 25, 20, 58, 42, 412, DateTimeKind.Unspecified).AddTicks(3282) });

            migrationBuilder.UpdateData(
                table: "Industries",
                keyColumn: "IndustryId",
                keyValue: 8,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 25, 20, 58, 42, 412, DateTimeKind.Unspecified).AddTicks(3284), new DateTime(2025, 7, 25, 20, 58, 42, 412, DateTimeKind.Unspecified).AddTicks(3286) });

            migrationBuilder.UpdateData(
                table: "Industries",
                keyColumn: "IndustryId",
                keyValue: 9,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 25, 20, 58, 42, 412, DateTimeKind.Unspecified).AddTicks(3288), new DateTime(2025, 7, 25, 20, 58, 42, 412, DateTimeKind.Unspecified).AddTicks(3289) });

            migrationBuilder.UpdateData(
                table: "JobTypes",
                keyColumn: "JobTypeId",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 25, 20, 58, 42, 412, DateTimeKind.Unspecified).AddTicks(3329), new DateTime(2025, 7, 25, 20, 58, 42, 412, DateTimeKind.Unspecified).AddTicks(3331) });

            migrationBuilder.UpdateData(
                table: "JobTypes",
                keyColumn: "JobTypeId",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 25, 20, 58, 42, 412, DateTimeKind.Unspecified).AddTicks(3334), new DateTime(2025, 7, 25, 20, 58, 42, 412, DateTimeKind.Unspecified).AddTicks(3335) });

            migrationBuilder.UpdateData(
                table: "JobTypes",
                keyColumn: "JobTypeId",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 25, 20, 58, 42, 412, DateTimeKind.Unspecified).AddTicks(3338), new DateTime(2025, 7, 25, 20, 58, 42, 412, DateTimeKind.Unspecified).AddTicks(3339) });

            migrationBuilder.UpdateData(
                table: "Levels",
                keyColumn: "LevelId",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 25, 20, 58, 42, 412, DateTimeKind.Unspecified).AddTicks(3371), new DateTime(2025, 7, 25, 20, 58, 42, 412, DateTimeKind.Unspecified).AddTicks(3372) });

            migrationBuilder.UpdateData(
                table: "Levels",
                keyColumn: "LevelId",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 25, 20, 58, 42, 412, DateTimeKind.Unspecified).AddTicks(3376), new DateTime(2025, 7, 25, 20, 58, 42, 412, DateTimeKind.Unspecified).AddTicks(3377) });

            migrationBuilder.UpdateData(
                table: "Levels",
                keyColumn: "LevelId",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 25, 20, 58, 42, 412, DateTimeKind.Unspecified).AddTicks(3380), new DateTime(2025, 7, 25, 20, 58, 42, 412, DateTimeKind.Unspecified).AddTicks(3381) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 25, 13, 58, 42, 412, DateTimeKind.Utc).AddTicks(3024), new DateTime(2025, 7, 25, 13, 58, 42, 412, DateTimeKind.Utc).AddTicks(3026) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 25, 13, 58, 42, 412, DateTimeKind.Utc).AddTicks(3035), new DateTime(2025, 7, 25, 13, 58, 42, 412, DateTimeKind.Utc).AddTicks(3036) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 25, 13, 58, 42, 412, DateTimeKind.Utc).AddTicks(3037), new DateTime(2025, 7, 25, 13, 58, 42, 412, DateTimeKind.Utc).AddTicks(3037) });

            migrationBuilder.UpdateData(
                table: "SubscriptionTypes",
                keyColumn: "SubscriptionTypeId",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 25, 13, 58, 42, 418, DateTimeKind.Utc).AddTicks(1178), new DateTime(2025, 7, 25, 13, 58, 42, 418, DateTimeKind.Utc).AddTicks(1179) });

            migrationBuilder.UpdateData(
                table: "SubscriptionTypes",
                keyColumn: "SubscriptionTypeId",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 25, 13, 58, 42, 418, DateTimeKind.Utc).AddTicks(1184), new DateTime(2025, 7, 25, 13, 58, 42, 418, DateTimeKind.Utc).AddTicks(1184) });

            migrationBuilder.UpdateData(
                table: "SubscriptionTypes",
                keyColumn: "SubscriptionTypeId",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 25, 13, 58, 42, 418, DateTimeKind.Utc).AddTicks(1188), new DateTime(2025, 7, 25, 13, 58, 42, 418, DateTimeKind.Utc).AddTicks(1189) });
        }
    }
}
