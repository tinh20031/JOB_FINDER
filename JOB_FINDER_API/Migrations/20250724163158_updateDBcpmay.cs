using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace JOB_FINDER_API.Migrations
{
    /// <inheritdoc />
    public partial class updateDBcpmay : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "CompanySubscriptionTypes",
                columns: new[] { "CompanySubscriptionTypeId", "CreatedAt", "CvMatchLimit", "Description", "DurationInDays", "IsActive", "JobPostLimit", "Name", "PackageType", "Price", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 7, 24, 23, 31, 56, 48, DateTimeKind.Unspecified).AddTicks(8309), 5, "Free tier with basic features", 30, true, 2, "Free", 0, 0m, new DateTime(2025, 7, 24, 23, 31, 56, 48, DateTimeKind.Unspecified).AddTicks(8321) },
                    { 2, new DateTime(2025, 7, 24, 23, 31, 56, 48, DateTimeKind.Unspecified).AddTicks(8325), 10, "Basic tier with extended features", 30, true, 10, "Basic", 1, 2000m, new DateTime(2025, 7, 24, 23, 31, 56, 48, DateTimeKind.Unspecified).AddTicks(8326) },
                    { 3, new DateTime(2025, 7, 24, 23, 31, 56, 48, DateTimeKind.Unspecified).AddTicks(8330), 2147483647, "Premium tier with unlimited features", 30, true, 2147483647, "Premium", 2, 3000m, new DateTime(2025, 7, 24, 23, 31, 56, 48, DateTimeKind.Unspecified).AddTicks(8332) }
                });

            migrationBuilder.UpdateData(
                table: "Industries",
                keyColumn: "IndustryId",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 24, 23, 31, 56, 42, DateTimeKind.Unspecified).AddTicks(966), new DateTime(2025, 7, 24, 23, 31, 56, 42, DateTimeKind.Unspecified).AddTicks(991) });

            migrationBuilder.UpdateData(
                table: "Industries",
                keyColumn: "IndustryId",
                keyValue: 4,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 24, 23, 31, 56, 42, DateTimeKind.Unspecified).AddTicks(994), new DateTime(2025, 7, 24, 23, 31, 56, 42, DateTimeKind.Unspecified).AddTicks(996) });

            migrationBuilder.UpdateData(
                table: "Industries",
                keyColumn: "IndustryId",
                keyValue: 5,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 24, 23, 31, 56, 42, DateTimeKind.Unspecified).AddTicks(999), new DateTime(2025, 7, 24, 23, 31, 56, 42, DateTimeKind.Unspecified).AddTicks(1000) });

            migrationBuilder.UpdateData(
                table: "Industries",
                keyColumn: "IndustryId",
                keyValue: 6,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 24, 23, 31, 56, 42, DateTimeKind.Unspecified).AddTicks(1002), new DateTime(2025, 7, 24, 23, 31, 56, 42, DateTimeKind.Unspecified).AddTicks(1003) });

            migrationBuilder.UpdateData(
                table: "Industries",
                keyColumn: "IndustryId",
                keyValue: 7,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 24, 23, 31, 56, 42, DateTimeKind.Unspecified).AddTicks(1006), new DateTime(2025, 7, 24, 23, 31, 56, 42, DateTimeKind.Unspecified).AddTicks(1007) });

            migrationBuilder.UpdateData(
                table: "Industries",
                keyColumn: "IndustryId",
                keyValue: 8,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 24, 23, 31, 56, 42, DateTimeKind.Unspecified).AddTicks(1009), new DateTime(2025, 7, 24, 23, 31, 56, 42, DateTimeKind.Unspecified).AddTicks(1010) });

            migrationBuilder.UpdateData(
                table: "Industries",
                keyColumn: "IndustryId",
                keyValue: 9,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 24, 23, 31, 56, 42, DateTimeKind.Unspecified).AddTicks(1012), new DateTime(2025, 7, 24, 23, 31, 56, 42, DateTimeKind.Unspecified).AddTicks(1013) });

            migrationBuilder.UpdateData(
                table: "JobTypes",
                keyColumn: "JobTypeId",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 24, 23, 31, 56, 42, DateTimeKind.Unspecified).AddTicks(1105), new DateTime(2025, 7, 24, 23, 31, 56, 42, DateTimeKind.Unspecified).AddTicks(1106) });

            migrationBuilder.UpdateData(
                table: "JobTypes",
                keyColumn: "JobTypeId",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 24, 23, 31, 56, 42, DateTimeKind.Unspecified).AddTicks(1110), new DateTime(2025, 7, 24, 23, 31, 56, 42, DateTimeKind.Unspecified).AddTicks(1112) });

            migrationBuilder.UpdateData(
                table: "JobTypes",
                keyColumn: "JobTypeId",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 24, 23, 31, 56, 42, DateTimeKind.Unspecified).AddTicks(1114), new DateTime(2025, 7, 24, 23, 31, 56, 42, DateTimeKind.Unspecified).AddTicks(1116) });

            migrationBuilder.UpdateData(
                table: "Levels",
                keyColumn: "LevelId",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 24, 23, 31, 56, 42, DateTimeKind.Unspecified).AddTicks(1146), new DateTime(2025, 7, 24, 23, 31, 56, 42, DateTimeKind.Unspecified).AddTicks(1147) });

            migrationBuilder.UpdateData(
                table: "Levels",
                keyColumn: "LevelId",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 24, 23, 31, 56, 42, DateTimeKind.Unspecified).AddTicks(1151), new DateTime(2025, 7, 24, 23, 31, 56, 42, DateTimeKind.Unspecified).AddTicks(1152) });

            migrationBuilder.UpdateData(
                table: "Levels",
                keyColumn: "LevelId",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 24, 23, 31, 56, 42, DateTimeKind.Unspecified).AddTicks(1155), new DateTime(2025, 7, 24, 23, 31, 56, 42, DateTimeKind.Unspecified).AddTicks(1156) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 24, 16, 31, 56, 42, DateTimeKind.Utc).AddTicks(743), new DateTime(2025, 7, 24, 16, 31, 56, 42, DateTimeKind.Utc).AddTicks(746) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 24, 16, 31, 56, 42, DateTimeKind.Utc).AddTicks(751), new DateTime(2025, 7, 24, 16, 31, 56, 42, DateTimeKind.Utc).AddTicks(751) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 24, 16, 31, 56, 42, DateTimeKind.Utc).AddTicks(752), new DateTime(2025, 7, 24, 16, 31, 56, 42, DateTimeKind.Utc).AddTicks(752) });

            migrationBuilder.UpdateData(
                table: "SubscriptionTypes",
                keyColumn: "SubscriptionTypeId",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 24, 16, 31, 56, 48, DateTimeKind.Utc).AddTicks(8228), new DateTime(2025, 7, 24, 16, 31, 56, 48, DateTimeKind.Utc).AddTicks(8230) });

            migrationBuilder.UpdateData(
                table: "SubscriptionTypes",
                keyColumn: "SubscriptionTypeId",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 24, 16, 31, 56, 48, DateTimeKind.Utc).AddTicks(8235), new DateTime(2025, 7, 24, 16, 31, 56, 48, DateTimeKind.Utc).AddTicks(8236) });

            migrationBuilder.UpdateData(
                table: "SubscriptionTypes",
                keyColumn: "SubscriptionTypeId",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 24, 16, 31, 56, 48, DateTimeKind.Utc).AddTicks(8240), new DateTime(2025, 7, 24, 16, 31, 56, 48, DateTimeKind.Utc).AddTicks(8241) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "CompanySubscriptionTypes",
                keyColumn: "CompanySubscriptionTypeId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "CompanySubscriptionTypes",
                keyColumn: "CompanySubscriptionTypeId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "CompanySubscriptionTypes",
                keyColumn: "CompanySubscriptionTypeId",
                keyValue: 3);

            migrationBuilder.UpdateData(
                table: "Industries",
                keyColumn: "IndustryId",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 24, 23, 27, 59, 20, DateTimeKind.Unspecified).AddTicks(5876), new DateTime(2025, 7, 24, 23, 27, 59, 20, DateTimeKind.Unspecified).AddTicks(5902) });

            migrationBuilder.UpdateData(
                table: "Industries",
                keyColumn: "IndustryId",
                keyValue: 4,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 24, 23, 27, 59, 20, DateTimeKind.Unspecified).AddTicks(5905), new DateTime(2025, 7, 24, 23, 27, 59, 20, DateTimeKind.Unspecified).AddTicks(5907) });

            migrationBuilder.UpdateData(
                table: "Industries",
                keyColumn: "IndustryId",
                keyValue: 5,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 24, 23, 27, 59, 20, DateTimeKind.Unspecified).AddTicks(5910), new DateTime(2025, 7, 24, 23, 27, 59, 20, DateTimeKind.Unspecified).AddTicks(5911) });

            migrationBuilder.UpdateData(
                table: "Industries",
                keyColumn: "IndustryId",
                keyValue: 6,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 24, 23, 27, 59, 20, DateTimeKind.Unspecified).AddTicks(5914), new DateTime(2025, 7, 24, 23, 27, 59, 20, DateTimeKind.Unspecified).AddTicks(5915) });

            migrationBuilder.UpdateData(
                table: "Industries",
                keyColumn: "IndustryId",
                keyValue: 7,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 24, 23, 27, 59, 20, DateTimeKind.Unspecified).AddTicks(5917), new DateTime(2025, 7, 24, 23, 27, 59, 20, DateTimeKind.Unspecified).AddTicks(5919) });

            migrationBuilder.UpdateData(
                table: "Industries",
                keyColumn: "IndustryId",
                keyValue: 8,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 24, 23, 27, 59, 20, DateTimeKind.Unspecified).AddTicks(5921), new DateTime(2025, 7, 24, 23, 27, 59, 20, DateTimeKind.Unspecified).AddTicks(5922) });

            migrationBuilder.UpdateData(
                table: "Industries",
                keyColumn: "IndustryId",
                keyValue: 9,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 24, 23, 27, 59, 20, DateTimeKind.Unspecified).AddTicks(5924), new DateTime(2025, 7, 24, 23, 27, 59, 20, DateTimeKind.Unspecified).AddTicks(5926) });

            migrationBuilder.UpdateData(
                table: "JobTypes",
                keyColumn: "JobTypeId",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 24, 23, 27, 59, 20, DateTimeKind.Unspecified).AddTicks(5971), new DateTime(2025, 7, 24, 23, 27, 59, 20, DateTimeKind.Unspecified).AddTicks(5974) });

            migrationBuilder.UpdateData(
                table: "JobTypes",
                keyColumn: "JobTypeId",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 24, 23, 27, 59, 20, DateTimeKind.Unspecified).AddTicks(5977), new DateTime(2025, 7, 24, 23, 27, 59, 20, DateTimeKind.Unspecified).AddTicks(5978) });

            migrationBuilder.UpdateData(
                table: "JobTypes",
                keyColumn: "JobTypeId",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 24, 23, 27, 59, 20, DateTimeKind.Unspecified).AddTicks(5981), new DateTime(2025, 7, 24, 23, 27, 59, 20, DateTimeKind.Unspecified).AddTicks(5982) });

            migrationBuilder.UpdateData(
                table: "Levels",
                keyColumn: "LevelId",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 24, 23, 27, 59, 20, DateTimeKind.Unspecified).AddTicks(6014), new DateTime(2025, 7, 24, 23, 27, 59, 20, DateTimeKind.Unspecified).AddTicks(6015) });

            migrationBuilder.UpdateData(
                table: "Levels",
                keyColumn: "LevelId",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 24, 23, 27, 59, 20, DateTimeKind.Unspecified).AddTicks(6053), new DateTime(2025, 7, 24, 23, 27, 59, 20, DateTimeKind.Unspecified).AddTicks(6054) });

            migrationBuilder.UpdateData(
                table: "Levels",
                keyColumn: "LevelId",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 24, 23, 27, 59, 20, DateTimeKind.Unspecified).AddTicks(6057), new DateTime(2025, 7, 24, 23, 27, 59, 20, DateTimeKind.Unspecified).AddTicks(6058) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 24, 16, 27, 59, 20, DateTimeKind.Utc).AddTicks(5634), new DateTime(2025, 7, 24, 16, 27, 59, 20, DateTimeKind.Utc).AddTicks(5637) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 24, 16, 27, 59, 20, DateTimeKind.Utc).AddTicks(5642), new DateTime(2025, 7, 24, 16, 27, 59, 20, DateTimeKind.Utc).AddTicks(5642) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 24, 16, 27, 59, 20, DateTimeKind.Utc).AddTicks(5643), new DateTime(2025, 7, 24, 16, 27, 59, 20, DateTimeKind.Utc).AddTicks(5643) });

            migrationBuilder.UpdateData(
                table: "SubscriptionTypes",
                keyColumn: "SubscriptionTypeId",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 24, 16, 27, 59, 27, DateTimeKind.Utc).AddTicks(3527), new DateTime(2025, 7, 24, 16, 27, 59, 27, DateTimeKind.Utc).AddTicks(3529) });

            migrationBuilder.UpdateData(
                table: "SubscriptionTypes",
                keyColumn: "SubscriptionTypeId",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 24, 16, 27, 59, 27, DateTimeKind.Utc).AddTicks(3534), new DateTime(2025, 7, 24, 16, 27, 59, 27, DateTimeKind.Utc).AddTicks(3534) });

            migrationBuilder.UpdateData(
                table: "SubscriptionTypes",
                keyColumn: "SubscriptionTypeId",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 24, 16, 27, 59, 27, DateTimeKind.Utc).AddTicks(3539), new DateTime(2025, 7, 24, 16, 27, 59, 27, DateTimeKind.Utc).AddTicks(3539) });
        }
    }
}
