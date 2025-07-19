using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JOB_FINDER_API.Migrations
{
    /// <inheritdoc />
    public partial class updaetDBNew : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CVFeedbackLimit",
                table: "SubscriptionPlans");

            migrationBuilder.RenameColumn(
                name: "MatchingLimit",
                table: "SubscriptionPlans",
                newName: "TryMatchingLimit");

            migrationBuilder.UpdateData(
                table: "Industries",
                keyColumn: "IndustryId",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 19, 1, 29, 54, 224, DateTimeKind.Unspecified).AddTicks(4792), new DateTime(2025, 7, 19, 1, 29, 54, 224, DateTimeKind.Unspecified).AddTicks(4830) });

            migrationBuilder.UpdateData(
                table: "Industries",
                keyColumn: "IndustryId",
                keyValue: 4,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 19, 1, 29, 54, 224, DateTimeKind.Unspecified).AddTicks(4835), new DateTime(2025, 7, 19, 1, 29, 54, 224, DateTimeKind.Unspecified).AddTicks(4837) });

            migrationBuilder.UpdateData(
                table: "Industries",
                keyColumn: "IndustryId",
                keyValue: 5,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 19, 1, 29, 54, 224, DateTimeKind.Unspecified).AddTicks(4841), new DateTime(2025, 7, 19, 1, 29, 54, 224, DateTimeKind.Unspecified).AddTicks(4843) });

            migrationBuilder.UpdateData(
                table: "Industries",
                keyColumn: "IndustryId",
                keyValue: 6,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 19, 1, 29, 54, 224, DateTimeKind.Unspecified).AddTicks(4958), new DateTime(2025, 7, 19, 1, 29, 54, 224, DateTimeKind.Unspecified).AddTicks(4961) });

            migrationBuilder.UpdateData(
                table: "Industries",
                keyColumn: "IndustryId",
                keyValue: 7,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 19, 1, 29, 54, 224, DateTimeKind.Unspecified).AddTicks(4964), new DateTime(2025, 7, 19, 1, 29, 54, 224, DateTimeKind.Unspecified).AddTicks(4966) });

            migrationBuilder.UpdateData(
                table: "Industries",
                keyColumn: "IndustryId",
                keyValue: 8,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 19, 1, 29, 54, 224, DateTimeKind.Unspecified).AddTicks(4969), new DateTime(2025, 7, 19, 1, 29, 54, 224, DateTimeKind.Unspecified).AddTicks(4971) });

            migrationBuilder.UpdateData(
                table: "Industries",
                keyColumn: "IndustryId",
                keyValue: 9,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 19, 1, 29, 54, 224, DateTimeKind.Unspecified).AddTicks(4974), new DateTime(2025, 7, 19, 1, 29, 54, 224, DateTimeKind.Unspecified).AddTicks(4976) });

            migrationBuilder.UpdateData(
                table: "JobTypes",
                keyColumn: "JobTypeId",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 19, 1, 29, 54, 224, DateTimeKind.Unspecified).AddTicks(5052), new DateTime(2025, 7, 19, 1, 29, 54, 224, DateTimeKind.Unspecified).AddTicks(5054) });

            migrationBuilder.UpdateData(
                table: "JobTypes",
                keyColumn: "JobTypeId",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 19, 1, 29, 54, 224, DateTimeKind.Unspecified).AddTicks(5059), new DateTime(2025, 7, 19, 1, 29, 54, 224, DateTimeKind.Unspecified).AddTicks(5061) });

            migrationBuilder.UpdateData(
                table: "JobTypes",
                keyColumn: "JobTypeId",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 19, 1, 29, 54, 224, DateTimeKind.Unspecified).AddTicks(5065), new DateTime(2025, 7, 19, 1, 29, 54, 224, DateTimeKind.Unspecified).AddTicks(5066) });

            migrationBuilder.UpdateData(
                table: "Levels",
                keyColumn: "LevelId",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 19, 1, 29, 54, 224, DateTimeKind.Unspecified).AddTicks(5119), new DateTime(2025, 7, 19, 1, 29, 54, 224, DateTimeKind.Unspecified).AddTicks(5121) });

            migrationBuilder.UpdateData(
                table: "Levels",
                keyColumn: "LevelId",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 19, 1, 29, 54, 224, DateTimeKind.Unspecified).AddTicks(5125), new DateTime(2025, 7, 19, 1, 29, 54, 224, DateTimeKind.Unspecified).AddTicks(5127) });

            migrationBuilder.UpdateData(
                table: "Levels",
                keyColumn: "LevelId",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 19, 1, 29, 54, 224, DateTimeKind.Unspecified).AddTicks(5131), new DateTime(2025, 7, 19, 1, 29, 54, 224, DateTimeKind.Unspecified).AddTicks(5132) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 18, 18, 29, 54, 224, DateTimeKind.Utc).AddTicks(4484), new DateTime(2025, 7, 18, 18, 29, 54, 224, DateTimeKind.Utc).AddTicks(4488) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 18, 18, 29, 54, 224, DateTimeKind.Utc).AddTicks(4494), new DateTime(2025, 7, 18, 18, 29, 54, 224, DateTimeKind.Utc).AddTicks(4495) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 18, 18, 29, 54, 224, DateTimeKind.Utc).AddTicks(4496), new DateTime(2025, 7, 18, 18, 29, 54, 224, DateTimeKind.Utc).AddTicks(4496) });

            migrationBuilder.UpdateData(
                table: "SubscriptionPlans",
                keyColumn: "PlanId",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 18, 18, 29, 54, 232, DateTimeKind.Utc).AddTicks(3676), new DateTime(2025, 7, 18, 18, 29, 54, 232, DateTimeKind.Utc).AddTicks(3680) });

            migrationBuilder.UpdateData(
                table: "SubscriptionPlans",
                keyColumn: "PlanId",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 18, 18, 29, 54, 232, DateTimeKind.Utc).AddTicks(3686), new DateTime(2025, 7, 18, 18, 29, 54, 232, DateTimeKind.Utc).AddTicks(3686) });

            migrationBuilder.UpdateData(
                table: "SubscriptionPlans",
                keyColumn: "PlanId",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 18, 18, 29, 54, 232, DateTimeKind.Utc).AddTicks(3691), new DateTime(2025, 7, 18, 18, 29, 54, 232, DateTimeKind.Utc).AddTicks(3692) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TryMatchingLimit",
                table: "SubscriptionPlans",
                newName: "MatchingLimit");

            migrationBuilder.AddColumn<int>(
                name: "CVFeedbackLimit",
                table: "SubscriptionPlans",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "Industries",
                keyColumn: "IndustryId",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 18, 23, 15, 4, 603, DateTimeKind.Unspecified).AddTicks(1309), new DateTime(2025, 7, 18, 23, 15, 4, 603, DateTimeKind.Unspecified).AddTicks(1334) });

            migrationBuilder.UpdateData(
                table: "Industries",
                keyColumn: "IndustryId",
                keyValue: 4,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 18, 23, 15, 4, 603, DateTimeKind.Unspecified).AddTicks(1338), new DateTime(2025, 7, 18, 23, 15, 4, 603, DateTimeKind.Unspecified).AddTicks(1339) });

            migrationBuilder.UpdateData(
                table: "Industries",
                keyColumn: "IndustryId",
                keyValue: 5,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 18, 23, 15, 4, 603, DateTimeKind.Unspecified).AddTicks(1342), new DateTime(2025, 7, 18, 23, 15, 4, 603, DateTimeKind.Unspecified).AddTicks(1343) });

            migrationBuilder.UpdateData(
                table: "Industries",
                keyColumn: "IndustryId",
                keyValue: 6,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 18, 23, 15, 4, 603, DateTimeKind.Unspecified).AddTicks(1345), new DateTime(2025, 7, 18, 23, 15, 4, 603, DateTimeKind.Unspecified).AddTicks(1346) });

            migrationBuilder.UpdateData(
                table: "Industries",
                keyColumn: "IndustryId",
                keyValue: 7,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 18, 23, 15, 4, 603, DateTimeKind.Unspecified).AddTicks(1348), new DateTime(2025, 7, 18, 23, 15, 4, 603, DateTimeKind.Unspecified).AddTicks(1349) });

            migrationBuilder.UpdateData(
                table: "Industries",
                keyColumn: "IndustryId",
                keyValue: 8,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 18, 23, 15, 4, 603, DateTimeKind.Unspecified).AddTicks(1351), new DateTime(2025, 7, 18, 23, 15, 4, 603, DateTimeKind.Unspecified).AddTicks(1353) });

            migrationBuilder.UpdateData(
                table: "Industries",
                keyColumn: "IndustryId",
                keyValue: 9,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 18, 23, 15, 4, 603, DateTimeKind.Unspecified).AddTicks(1355), new DateTime(2025, 7, 18, 23, 15, 4, 603, DateTimeKind.Unspecified).AddTicks(1356) });

            migrationBuilder.UpdateData(
                table: "JobTypes",
                keyColumn: "JobTypeId",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 18, 23, 15, 4, 603, DateTimeKind.Unspecified).AddTicks(1402), new DateTime(2025, 7, 18, 23, 15, 4, 603, DateTimeKind.Unspecified).AddTicks(1404) });

            migrationBuilder.UpdateData(
                table: "JobTypes",
                keyColumn: "JobTypeId",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 18, 23, 15, 4, 603, DateTimeKind.Unspecified).AddTicks(1407), new DateTime(2025, 7, 18, 23, 15, 4, 603, DateTimeKind.Unspecified).AddTicks(1408) });

            migrationBuilder.UpdateData(
                table: "JobTypes",
                keyColumn: "JobTypeId",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 18, 23, 15, 4, 603, DateTimeKind.Unspecified).AddTicks(1411), new DateTime(2025, 7, 18, 23, 15, 4, 603, DateTimeKind.Unspecified).AddTicks(1412) });

            migrationBuilder.UpdateData(
                table: "Levels",
                keyColumn: "LevelId",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 18, 23, 15, 4, 603, DateTimeKind.Unspecified).AddTicks(1456), new DateTime(2025, 7, 18, 23, 15, 4, 603, DateTimeKind.Unspecified).AddTicks(1457) });

            migrationBuilder.UpdateData(
                table: "Levels",
                keyColumn: "LevelId",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 18, 23, 15, 4, 603, DateTimeKind.Unspecified).AddTicks(1460), new DateTime(2025, 7, 18, 23, 15, 4, 603, DateTimeKind.Unspecified).AddTicks(1462) });

            migrationBuilder.UpdateData(
                table: "Levels",
                keyColumn: "LevelId",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 18, 23, 15, 4, 603, DateTimeKind.Unspecified).AddTicks(1464), new DateTime(2025, 7, 18, 23, 15, 4, 603, DateTimeKind.Unspecified).AddTicks(1466) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 18, 16, 15, 4, 603, DateTimeKind.Utc).AddTicks(1088), new DateTime(2025, 7, 18, 16, 15, 4, 603, DateTimeKind.Utc).AddTicks(1091) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 18, 16, 15, 4, 603, DateTimeKind.Utc).AddTicks(1099), new DateTime(2025, 7, 18, 16, 15, 4, 603, DateTimeKind.Utc).AddTicks(1099) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 18, 16, 15, 4, 603, DateTimeKind.Utc).AddTicks(1100), new DateTime(2025, 7, 18, 16, 15, 4, 603, DateTimeKind.Utc).AddTicks(1101) });

            migrationBuilder.UpdateData(
                table: "SubscriptionPlans",
                keyColumn: "PlanId",
                keyValue: 1,
                columns: new[] { "CVFeedbackLimit", "CreatedAt", "UpdatedAt" },
                values: new object[] { 1, new DateTime(2025, 7, 18, 16, 15, 4, 610, DateTimeKind.Utc).AddTicks(2244), new DateTime(2025, 7, 18, 16, 15, 4, 610, DateTimeKind.Utc).AddTicks(2246) });

            migrationBuilder.UpdateData(
                table: "SubscriptionPlans",
                keyColumn: "PlanId",
                keyValue: 2,
                columns: new[] { "CVFeedbackLimit", "CreatedAt", "UpdatedAt" },
                values: new object[] { 3, new DateTime(2025, 7, 18, 16, 15, 4, 610, DateTimeKind.Utc).AddTicks(2255), new DateTime(2025, 7, 18, 16, 15, 4, 610, DateTimeKind.Utc).AddTicks(2255) });

            migrationBuilder.UpdateData(
                table: "SubscriptionPlans",
                keyColumn: "PlanId",
                keyValue: 3,
                columns: new[] { "CVFeedbackLimit", "CreatedAt", "UpdatedAt" },
                values: new object[] { 7, new DateTime(2025, 7, 18, 16, 15, 4, 610, DateTimeKind.Utc).AddTicks(2260), new DateTime(2025, 7, 18, 16, 15, 4, 610, DateTimeKind.Utc).AddTicks(2260) });
        }
    }
}
