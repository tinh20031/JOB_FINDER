using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JOB_FINDER_API.Migrations
{
    /// <inheritdoc />
    public partial class updateCandidateplan : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CVFeedbackUsed",
                table: "CandidateSubscriptions");

            migrationBuilder.RenameColumn(
                name: "MatchingUsed",
                table: "CandidateSubscriptions",
                newName: "TryMatchingUsed");

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
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 18, 18, 34, 29, 50, DateTimeKind.Utc).AddTicks(7199), new DateTime(2025, 7, 18, 18, 34, 29, 50, DateTimeKind.Utc).AddTicks(7200) });

            migrationBuilder.UpdateData(
                table: "SubscriptionPlans",
                keyColumn: "PlanId",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 18, 18, 34, 29, 50, DateTimeKind.Utc).AddTicks(7206), new DateTime(2025, 7, 18, 18, 34, 29, 50, DateTimeKind.Utc).AddTicks(7207) });

            migrationBuilder.UpdateData(
                table: "SubscriptionPlans",
                keyColumn: "PlanId",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 18, 18, 34, 29, 50, DateTimeKind.Utc).AddTicks(7211), new DateTime(2025, 7, 18, 18, 34, 29, 50, DateTimeKind.Utc).AddTicks(7211) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TryMatchingUsed",
                table: "CandidateSubscriptions",
                newName: "MatchingUsed");

            migrationBuilder.AddColumn<int>(
                name: "CVFeedbackUsed",
                table: "CandidateSubscriptions",
                type: "int",
                nullable: false,
                defaultValue: 0);

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
    }
}
