using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JOB_FINDER_API.Migrations
{
    /// <inheritdoc />
    public partial class updateseeddata : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Industries",
                keyColumn: "IndustryId",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 19, 10, 33, 0, 839, DateTimeKind.Unspecified).AddTicks(4938), new DateTime(2025, 7, 19, 10, 33, 0, 839, DateTimeKind.Unspecified).AddTicks(4965) });

            migrationBuilder.UpdateData(
                table: "Industries",
                keyColumn: "IndustryId",
                keyValue: 4,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 19, 10, 33, 0, 839, DateTimeKind.Unspecified).AddTicks(4968), new DateTime(2025, 7, 19, 10, 33, 0, 839, DateTimeKind.Unspecified).AddTicks(4969) });

            migrationBuilder.UpdateData(
                table: "Industries",
                keyColumn: "IndustryId",
                keyValue: 5,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 19, 10, 33, 0, 839, DateTimeKind.Unspecified).AddTicks(4971), new DateTime(2025, 7, 19, 10, 33, 0, 839, DateTimeKind.Unspecified).AddTicks(4972) });

            migrationBuilder.UpdateData(
                table: "Industries",
                keyColumn: "IndustryId",
                keyValue: 6,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 19, 10, 33, 0, 839, DateTimeKind.Unspecified).AddTicks(4975), new DateTime(2025, 7, 19, 10, 33, 0, 839, DateTimeKind.Unspecified).AddTicks(4976) });

            migrationBuilder.UpdateData(
                table: "Industries",
                keyColumn: "IndustryId",
                keyValue: 7,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 19, 10, 33, 0, 839, DateTimeKind.Unspecified).AddTicks(4978), new DateTime(2025, 7, 19, 10, 33, 0, 839, DateTimeKind.Unspecified).AddTicks(4979) });

            migrationBuilder.UpdateData(
                table: "Industries",
                keyColumn: "IndustryId",
                keyValue: 8,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 19, 10, 33, 0, 839, DateTimeKind.Unspecified).AddTicks(4981), new DateTime(2025, 7, 19, 10, 33, 0, 839, DateTimeKind.Unspecified).AddTicks(4982) });

            migrationBuilder.UpdateData(
                table: "Industries",
                keyColumn: "IndustryId",
                keyValue: 9,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 19, 10, 33, 0, 839, DateTimeKind.Unspecified).AddTicks(4984), new DateTime(2025, 7, 19, 10, 33, 0, 839, DateTimeKind.Unspecified).AddTicks(4986) });

            migrationBuilder.UpdateData(
                table: "JobTypes",
                keyColumn: "JobTypeId",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 19, 10, 33, 0, 839, DateTimeKind.Unspecified).AddTicks(5032), new DateTime(2025, 7, 19, 10, 33, 0, 839, DateTimeKind.Unspecified).AddTicks(5034) });

            migrationBuilder.UpdateData(
                table: "JobTypes",
                keyColumn: "JobTypeId",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 19, 10, 33, 0, 839, DateTimeKind.Unspecified).AddTicks(5037), new DateTime(2025, 7, 19, 10, 33, 0, 839, DateTimeKind.Unspecified).AddTicks(5038) });

            migrationBuilder.UpdateData(
                table: "JobTypes",
                keyColumn: "JobTypeId",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 19, 10, 33, 0, 839, DateTimeKind.Unspecified).AddTicks(5041), new DateTime(2025, 7, 19, 10, 33, 0, 839, DateTimeKind.Unspecified).AddTicks(5042) });

            migrationBuilder.UpdateData(
                table: "Levels",
                keyColumn: "LevelId",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 19, 10, 33, 0, 839, DateTimeKind.Unspecified).AddTicks(5078), new DateTime(2025, 7, 19, 10, 33, 0, 839, DateTimeKind.Unspecified).AddTicks(5080) });

            migrationBuilder.UpdateData(
                table: "Levels",
                keyColumn: "LevelId",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 19, 10, 33, 0, 839, DateTimeKind.Unspecified).AddTicks(5082), new DateTime(2025, 7, 19, 10, 33, 0, 839, DateTimeKind.Unspecified).AddTicks(5084) });

            migrationBuilder.UpdateData(
                table: "Levels",
                keyColumn: "LevelId",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 19, 10, 33, 0, 839, DateTimeKind.Unspecified).AddTicks(5086), new DateTime(2025, 7, 19, 10, 33, 0, 839, DateTimeKind.Unspecified).AddTicks(5087) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 19, 3, 33, 0, 839, DateTimeKind.Utc).AddTicks(4725), new DateTime(2025, 7, 19, 3, 33, 0, 839, DateTimeKind.Utc).AddTicks(4727) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 19, 3, 33, 0, 839, DateTimeKind.Utc).AddTicks(4732), new DateTime(2025, 7, 19, 3, 33, 0, 839, DateTimeKind.Utc).AddTicks(4733) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 19, 3, 33, 0, 839, DateTimeKind.Utc).AddTicks(4734), new DateTime(2025, 7, 19, 3, 33, 0, 839, DateTimeKind.Utc).AddTicks(4734) });

            migrationBuilder.UpdateData(
                table: "SubscriptionPlans",
                keyColumn: "PlanId",
                keyValue: 1,
                columns: new[] { "CreatedAt", "Description", "Name", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 19, 3, 33, 0, 846, DateTimeKind.Utc).AddTicks(5457), "Free service package with basic features", "Free Package", new DateTime(2025, 7, 19, 3, 33, 0, 846, DateTimeKind.Utc).AddTicks(5460) });

            migrationBuilder.UpdateData(
                table: "SubscriptionPlans",
                keyColumn: "PlanId",
                keyValue: 2,
                columns: new[] { "CreatedAt", "Description", "Name", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 19, 3, 33, 0, 846, DateTimeKind.Utc).AddTicks(5467), "Basic service package with advanced features", "Basic Package", new DateTime(2025, 7, 19, 3, 33, 0, 846, DateTimeKind.Utc).AddTicks(5467) });

            migrationBuilder.UpdateData(
                table: "SubscriptionPlans",
                keyColumn: "PlanId",
                keyValue: 3,
                columns: new[] { "CreatedAt", "Description", "Name", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 19, 3, 33, 0, 846, DateTimeKind.Utc).AddTicks(5473), "Advanced package with full features and unlimited CV uploads", "Advanced Package", new DateTime(2025, 7, 19, 3, 33, 0, 846, DateTimeKind.Utc).AddTicks(5473) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Industries",
                keyColumn: "IndustryId",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 19, 1, 34, 29, 43, DateTimeKind.Unspecified).AddTicks(8711), new DateTime(2025, 7, 19, 1, 34, 29, 43, DateTimeKind.Unspecified).AddTicks(8738) });

            migrationBuilder.UpdateData(
                table: "Industries",
                keyColumn: "IndustryId",
                keyValue: 4,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 19, 1, 34, 29, 43, DateTimeKind.Unspecified).AddTicks(8741), new DateTime(2025, 7, 19, 1, 34, 29, 43, DateTimeKind.Unspecified).AddTicks(8743) });

            migrationBuilder.UpdateData(
                table: "Industries",
                keyColumn: "IndustryId",
                keyValue: 5,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 19, 1, 34, 29, 43, DateTimeKind.Unspecified).AddTicks(8746), new DateTime(2025, 7, 19, 1, 34, 29, 43, DateTimeKind.Unspecified).AddTicks(8747) });

            migrationBuilder.UpdateData(
                table: "Industries",
                keyColumn: "IndustryId",
                keyValue: 6,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 19, 1, 34, 29, 43, DateTimeKind.Unspecified).AddTicks(8750), new DateTime(2025, 7, 19, 1, 34, 29, 43, DateTimeKind.Unspecified).AddTicks(8751) });

            migrationBuilder.UpdateData(
                table: "Industries",
                keyColumn: "IndustryId",
                keyValue: 7,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 19, 1, 34, 29, 43, DateTimeKind.Unspecified).AddTicks(8753), new DateTime(2025, 7, 19, 1, 34, 29, 43, DateTimeKind.Unspecified).AddTicks(8754) });

            migrationBuilder.UpdateData(
                table: "Industries",
                keyColumn: "IndustryId",
                keyValue: 8,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 19, 1, 34, 29, 43, DateTimeKind.Unspecified).AddTicks(8757), new DateTime(2025, 7, 19, 1, 34, 29, 43, DateTimeKind.Unspecified).AddTicks(8758) });

            migrationBuilder.UpdateData(
                table: "Industries",
                keyColumn: "IndustryId",
                keyValue: 9,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 19, 1, 34, 29, 43, DateTimeKind.Unspecified).AddTicks(8761), new DateTime(2025, 7, 19, 1, 34, 29, 43, DateTimeKind.Unspecified).AddTicks(8762) });

            migrationBuilder.UpdateData(
                table: "JobTypes",
                keyColumn: "JobTypeId",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 19, 1, 34, 29, 43, DateTimeKind.Unspecified).AddTicks(8805), new DateTime(2025, 7, 19, 1, 34, 29, 43, DateTimeKind.Unspecified).AddTicks(8806) });

            migrationBuilder.UpdateData(
                table: "JobTypes",
                keyColumn: "JobTypeId",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 19, 1, 34, 29, 43, DateTimeKind.Unspecified).AddTicks(8810), new DateTime(2025, 7, 19, 1, 34, 29, 43, DateTimeKind.Unspecified).AddTicks(8812) });

            migrationBuilder.UpdateData(
                table: "JobTypes",
                keyColumn: "JobTypeId",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 19, 1, 34, 29, 43, DateTimeKind.Unspecified).AddTicks(8814), new DateTime(2025, 7, 19, 1, 34, 29, 43, DateTimeKind.Unspecified).AddTicks(8816) });

            migrationBuilder.UpdateData(
                table: "Levels",
                keyColumn: "LevelId",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 19, 1, 34, 29, 43, DateTimeKind.Unspecified).AddTicks(8895), new DateTime(2025, 7, 19, 1, 34, 29, 43, DateTimeKind.Unspecified).AddTicks(8897) });

            migrationBuilder.UpdateData(
                table: "Levels",
                keyColumn: "LevelId",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 19, 1, 34, 29, 43, DateTimeKind.Unspecified).AddTicks(8900), new DateTime(2025, 7, 19, 1, 34, 29, 43, DateTimeKind.Unspecified).AddTicks(8901) });

            migrationBuilder.UpdateData(
                table: "Levels",
                keyColumn: "LevelId",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 19, 1, 34, 29, 43, DateTimeKind.Unspecified).AddTicks(8904), new DateTime(2025, 7, 19, 1, 34, 29, 43, DateTimeKind.Unspecified).AddTicks(8906) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 18, 18, 34, 29, 43, DateTimeKind.Utc).AddTicks(8484), new DateTime(2025, 7, 18, 18, 34, 29, 43, DateTimeKind.Utc).AddTicks(8485) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 18, 18, 34, 29, 43, DateTimeKind.Utc).AddTicks(8490), new DateTime(2025, 7, 18, 18, 34, 29, 43, DateTimeKind.Utc).AddTicks(8491) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 18, 18, 34, 29, 43, DateTimeKind.Utc).AddTicks(8492), new DateTime(2025, 7, 18, 18, 34, 29, 43, DateTimeKind.Utc).AddTicks(8492) });

            migrationBuilder.UpdateData(
                table: "SubscriptionPlans",
                keyColumn: "PlanId",
                keyValue: 1,
                columns: new[] { "CreatedAt", "Description", "Name", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 18, 18, 34, 29, 50, DateTimeKind.Utc).AddTicks(7199), "Gói dịch vụ miễn phí với các tính năng cơ bản", "Gói Miễn Phí", new DateTime(2025, 7, 18, 18, 34, 29, 50, DateTimeKind.Utc).AddTicks(7200) });

            migrationBuilder.UpdateData(
                table: "SubscriptionPlans",
                keyColumn: "PlanId",
                keyValue: 2,
                columns: new[] { "CreatedAt", "Description", "Name", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 18, 18, 34, 29, 50, DateTimeKind.Utc).AddTicks(7206), "Gói dịch vụ cơ bản với các tính năng nâng cao", "Gói Cơ Bản", new DateTime(2025, 7, 18, 18, 34, 29, 50, DateTimeKind.Utc).AddTicks(7207) });

            migrationBuilder.UpdateData(
                table: "SubscriptionPlans",
                keyColumn: "PlanId",
                keyValue: 3,
                columns: new[] { "CreatedAt", "Description", "Name", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 18, 18, 34, 29, 50, DateTimeKind.Utc).AddTicks(7211), "Gói dịch vụ nâng cao với đầy đủ tính năng và không giới hạn tải CV", "Gói Nâng Cao", new DateTime(2025, 7, 18, 18, 34, 29, 50, DateTimeKind.Utc).AddTicks(7211) });
        }
    }
}
