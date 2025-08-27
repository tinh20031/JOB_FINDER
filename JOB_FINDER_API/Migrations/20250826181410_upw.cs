using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JOB_FINDER_API.Migrations
{
    /// <inheritdoc />
    public partial class upw : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "CompanySubscriptionTypes",
                keyColumn: "CompanySubscriptionTypeId",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 27, 1, 14, 9, 542, DateTimeKind.Unspecified).AddTicks(9584), new DateTime(2025, 8, 27, 1, 14, 9, 542, DateTimeKind.Unspecified).AddTicks(9606) });

            migrationBuilder.UpdateData(
                table: "CompanySubscriptionTypes",
                keyColumn: "CompanySubscriptionTypeId",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 27, 1, 14, 9, 542, DateTimeKind.Unspecified).AddTicks(9611), new DateTime(2025, 8, 27, 1, 14, 9, 542, DateTimeKind.Unspecified).AddTicks(9613) });

            migrationBuilder.UpdateData(
                table: "CompanySubscriptionTypes",
                keyColumn: "CompanySubscriptionTypeId",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 27, 1, 14, 9, 542, DateTimeKind.Unspecified).AddTicks(9617), new DateTime(2025, 8, 27, 1, 14, 9, 542, DateTimeKind.Unspecified).AddTicks(9618) });

            migrationBuilder.UpdateData(
                table: "Industries",
                keyColumn: "IndustryId",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 27, 1, 14, 9, 536, DateTimeKind.Unspecified).AddTicks(5324), new DateTime(2025, 8, 27, 1, 14, 9, 536, DateTimeKind.Unspecified).AddTicks(5357) });

            migrationBuilder.UpdateData(
                table: "Industries",
                keyColumn: "IndustryId",
                keyValue: 4,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 27, 1, 14, 9, 536, DateTimeKind.Unspecified).AddTicks(5360), new DateTime(2025, 8, 27, 1, 14, 9, 536, DateTimeKind.Unspecified).AddTicks(5362) });

            migrationBuilder.UpdateData(
                table: "Industries",
                keyColumn: "IndustryId",
                keyValue: 5,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 27, 1, 14, 9, 536, DateTimeKind.Unspecified).AddTicks(5365), new DateTime(2025, 8, 27, 1, 14, 9, 536, DateTimeKind.Unspecified).AddTicks(5366) });

            migrationBuilder.UpdateData(
                table: "Industries",
                keyColumn: "IndustryId",
                keyValue: 6,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 27, 1, 14, 9, 536, DateTimeKind.Unspecified).AddTicks(5368), new DateTime(2025, 8, 27, 1, 14, 9, 536, DateTimeKind.Unspecified).AddTicks(5370) });

            migrationBuilder.UpdateData(
                table: "Industries",
                keyColumn: "IndustryId",
                keyValue: 7,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 27, 1, 14, 9, 536, DateTimeKind.Unspecified).AddTicks(5372), new DateTime(2025, 8, 27, 1, 14, 9, 536, DateTimeKind.Unspecified).AddTicks(5373) });

            migrationBuilder.UpdateData(
                table: "Industries",
                keyColumn: "IndustryId",
                keyValue: 8,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 27, 1, 14, 9, 536, DateTimeKind.Unspecified).AddTicks(5376), new DateTime(2025, 8, 27, 1, 14, 9, 536, DateTimeKind.Unspecified).AddTicks(5377) });

            migrationBuilder.UpdateData(
                table: "Industries",
                keyColumn: "IndustryId",
                keyValue: 9,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 27, 1, 14, 9, 536, DateTimeKind.Unspecified).AddTicks(5380), new DateTime(2025, 8, 27, 1, 14, 9, 536, DateTimeKind.Unspecified).AddTicks(5381) });

            migrationBuilder.UpdateData(
                table: "JobTypes",
                keyColumn: "JobTypeId",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 27, 1, 14, 9, 536, DateTimeKind.Unspecified).AddTicks(5415), new DateTime(2025, 8, 27, 1, 14, 9, 536, DateTimeKind.Unspecified).AddTicks(5417) });

            migrationBuilder.UpdateData(
                table: "JobTypes",
                keyColumn: "JobTypeId",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 27, 1, 14, 9, 536, DateTimeKind.Unspecified).AddTicks(5420), new DateTime(2025, 8, 27, 1, 14, 9, 536, DateTimeKind.Unspecified).AddTicks(5422) });

            migrationBuilder.UpdateData(
                table: "JobTypes",
                keyColumn: "JobTypeId",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 27, 1, 14, 9, 536, DateTimeKind.Unspecified).AddTicks(5425), new DateTime(2025, 8, 27, 1, 14, 9, 536, DateTimeKind.Unspecified).AddTicks(5426) });

            migrationBuilder.UpdateData(
                table: "Levels",
                keyColumn: "LevelId",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 27, 1, 14, 9, 536, DateTimeKind.Unspecified).AddTicks(5453), new DateTime(2025, 8, 27, 1, 14, 9, 536, DateTimeKind.Unspecified).AddTicks(5454) });

            migrationBuilder.UpdateData(
                table: "Levels",
                keyColumn: "LevelId",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 27, 1, 14, 9, 536, DateTimeKind.Unspecified).AddTicks(5458), new DateTime(2025, 8, 27, 1, 14, 9, 536, DateTimeKind.Unspecified).AddTicks(5459) });

            migrationBuilder.UpdateData(
                table: "Levels",
                keyColumn: "LevelId",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 27, 1, 14, 9, 536, DateTimeKind.Unspecified).AddTicks(5462), new DateTime(2025, 8, 27, 1, 14, 9, 536, DateTimeKind.Unspecified).AddTicks(5463) });

            migrationBuilder.UpdateData(
                table: "Levels",
                keyColumn: "LevelId",
                keyValue: 4,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 27, 1, 14, 9, 536, DateTimeKind.Unspecified).AddTicks(5466), new DateTime(2025, 8, 27, 1, 14, 9, 536, DateTimeKind.Unspecified).AddTicks(5467) });

            migrationBuilder.UpdateData(
                table: "Levels",
                keyColumn: "LevelId",
                keyValue: 5,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 27, 1, 14, 9, 536, DateTimeKind.Unspecified).AddTicks(5470), new DateTime(2025, 8, 27, 1, 14, 9, 536, DateTimeKind.Unspecified).AddTicks(5471) });

            migrationBuilder.UpdateData(
                table: "Levels",
                keyColumn: "LevelId",
                keyValue: 6,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 27, 1, 14, 9, 536, DateTimeKind.Unspecified).AddTicks(5474), new DateTime(2025, 8, 27, 1, 14, 9, 536, DateTimeKind.Unspecified).AddTicks(5475) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 26, 18, 14, 9, 536, DateTimeKind.Utc).AddTicks(5143), new DateTime(2025, 8, 26, 18, 14, 9, 536, DateTimeKind.Utc).AddTicks(5147) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 26, 18, 14, 9, 536, DateTimeKind.Utc).AddTicks(5152), new DateTime(2025, 8, 26, 18, 14, 9, 536, DateTimeKind.Utc).AddTicks(5152) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 26, 18, 14, 9, 536, DateTimeKind.Utc).AddTicks(5153), new DateTime(2025, 8, 26, 18, 14, 9, 536, DateTimeKind.Utc).AddTicks(5153) });

            migrationBuilder.UpdateData(
                table: "SubscriptionTypes",
                keyColumn: "SubscriptionTypeId",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 26, 18, 14, 9, 542, DateTimeKind.Utc).AddTicks(9490), new DateTime(2025, 8, 26, 18, 14, 9, 542, DateTimeKind.Utc).AddTicks(9496) });

            migrationBuilder.UpdateData(
                table: "SubscriptionTypes",
                keyColumn: "SubscriptionTypeId",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 26, 18, 14, 9, 542, DateTimeKind.Utc).AddTicks(9502), new DateTime(2025, 8, 26, 18, 14, 9, 542, DateTimeKind.Utc).AddTicks(9502) });

            migrationBuilder.UpdateData(
                table: "SubscriptionTypes",
                keyColumn: "SubscriptionTypeId",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 26, 18, 14, 9, 542, DateTimeKind.Utc).AddTicks(9507), new DateTime(2025, 8, 26, 18, 14, 9, 542, DateTimeKind.Utc).AddTicks(9508) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "CompanySubscriptionTypes",
                keyColumn: "CompanySubscriptionTypeId",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 26, 14, 40, 45, 457, DateTimeKind.Unspecified).AddTicks(7303), new DateTime(2025, 8, 26, 14, 40, 45, 457, DateTimeKind.Unspecified).AddTicks(7336) });

            migrationBuilder.UpdateData(
                table: "CompanySubscriptionTypes",
                keyColumn: "CompanySubscriptionTypeId",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 26, 14, 40, 45, 457, DateTimeKind.Unspecified).AddTicks(7341), new DateTime(2025, 8, 26, 14, 40, 45, 457, DateTimeKind.Unspecified).AddTicks(7342) });

            migrationBuilder.UpdateData(
                table: "CompanySubscriptionTypes",
                keyColumn: "CompanySubscriptionTypeId",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 26, 14, 40, 45, 457, DateTimeKind.Unspecified).AddTicks(7346), new DateTime(2025, 8, 26, 14, 40, 45, 457, DateTimeKind.Unspecified).AddTicks(7347) });

            migrationBuilder.UpdateData(
                table: "Industries",
                keyColumn: "IndustryId",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 26, 14, 40, 45, 450, DateTimeKind.Unspecified).AddTicks(4442), new DateTime(2025, 8, 26, 14, 40, 45, 450, DateTimeKind.Unspecified).AddTicks(4470) });

            migrationBuilder.UpdateData(
                table: "Industries",
                keyColumn: "IndustryId",
                keyValue: 4,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 26, 14, 40, 45, 450, DateTimeKind.Unspecified).AddTicks(4474), new DateTime(2025, 8, 26, 14, 40, 45, 450, DateTimeKind.Unspecified).AddTicks(4475) });

            migrationBuilder.UpdateData(
                table: "Industries",
                keyColumn: "IndustryId",
                keyValue: 5,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 26, 14, 40, 45, 450, DateTimeKind.Unspecified).AddTicks(4478), new DateTime(2025, 8, 26, 14, 40, 45, 450, DateTimeKind.Unspecified).AddTicks(4479) });

            migrationBuilder.UpdateData(
                table: "Industries",
                keyColumn: "IndustryId",
                keyValue: 6,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 26, 14, 40, 45, 450, DateTimeKind.Unspecified).AddTicks(4482), new DateTime(2025, 8, 26, 14, 40, 45, 450, DateTimeKind.Unspecified).AddTicks(4483) });

            migrationBuilder.UpdateData(
                table: "Industries",
                keyColumn: "IndustryId",
                keyValue: 7,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 26, 14, 40, 45, 450, DateTimeKind.Unspecified).AddTicks(4486), new DateTime(2025, 8, 26, 14, 40, 45, 450, DateTimeKind.Unspecified).AddTicks(4488) });

            migrationBuilder.UpdateData(
                table: "Industries",
                keyColumn: "IndustryId",
                keyValue: 8,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 26, 14, 40, 45, 450, DateTimeKind.Unspecified).AddTicks(4490), new DateTime(2025, 8, 26, 14, 40, 45, 450, DateTimeKind.Unspecified).AddTicks(4492) });

            migrationBuilder.UpdateData(
                table: "Industries",
                keyColumn: "IndustryId",
                keyValue: 9,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 26, 14, 40, 45, 450, DateTimeKind.Unspecified).AddTicks(4495), new DateTime(2025, 8, 26, 14, 40, 45, 450, DateTimeKind.Unspecified).AddTicks(4496) });

            migrationBuilder.UpdateData(
                table: "JobTypes",
                keyColumn: "JobTypeId",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 26, 14, 40, 45, 450, DateTimeKind.Unspecified).AddTicks(4531), new DateTime(2025, 8, 26, 14, 40, 45, 450, DateTimeKind.Unspecified).AddTicks(4533) });

            migrationBuilder.UpdateData(
                table: "JobTypes",
                keyColumn: "JobTypeId",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 26, 14, 40, 45, 450, DateTimeKind.Unspecified).AddTicks(4536), new DateTime(2025, 8, 26, 14, 40, 45, 450, DateTimeKind.Unspecified).AddTicks(4537) });

            migrationBuilder.UpdateData(
                table: "JobTypes",
                keyColumn: "JobTypeId",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 26, 14, 40, 45, 450, DateTimeKind.Unspecified).AddTicks(4540), new DateTime(2025, 8, 26, 14, 40, 45, 450, DateTimeKind.Unspecified).AddTicks(4541) });

            migrationBuilder.UpdateData(
                table: "Levels",
                keyColumn: "LevelId",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 26, 14, 40, 45, 450, DateTimeKind.Unspecified).AddTicks(4569), new DateTime(2025, 8, 26, 14, 40, 45, 450, DateTimeKind.Unspecified).AddTicks(4571) });

            migrationBuilder.UpdateData(
                table: "Levels",
                keyColumn: "LevelId",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 26, 14, 40, 45, 450, DateTimeKind.Unspecified).AddTicks(4574), new DateTime(2025, 8, 26, 14, 40, 45, 450, DateTimeKind.Unspecified).AddTicks(4575) });

            migrationBuilder.UpdateData(
                table: "Levels",
                keyColumn: "LevelId",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 26, 14, 40, 45, 450, DateTimeKind.Unspecified).AddTicks(4578), new DateTime(2025, 8, 26, 14, 40, 45, 450, DateTimeKind.Unspecified).AddTicks(4579) });

            migrationBuilder.UpdateData(
                table: "Levels",
                keyColumn: "LevelId",
                keyValue: 4,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 26, 14, 40, 45, 450, DateTimeKind.Unspecified).AddTicks(4582), new DateTime(2025, 8, 26, 14, 40, 45, 450, DateTimeKind.Unspecified).AddTicks(4583) });

            migrationBuilder.UpdateData(
                table: "Levels",
                keyColumn: "LevelId",
                keyValue: 5,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 26, 14, 40, 45, 450, DateTimeKind.Unspecified).AddTicks(4585), new DateTime(2025, 8, 26, 14, 40, 45, 450, DateTimeKind.Unspecified).AddTicks(4587) });

            migrationBuilder.UpdateData(
                table: "Levels",
                keyColumn: "LevelId",
                keyValue: 6,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 26, 14, 40, 45, 450, DateTimeKind.Unspecified).AddTicks(4589), new DateTime(2025, 8, 26, 14, 40, 45, 450, DateTimeKind.Unspecified).AddTicks(4590) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 26, 7, 40, 45, 450, DateTimeKind.Utc).AddTicks(4241), new DateTime(2025, 8, 26, 7, 40, 45, 450, DateTimeKind.Utc).AddTicks(4243) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 26, 7, 40, 45, 450, DateTimeKind.Utc).AddTicks(4248), new DateTime(2025, 8, 26, 7, 40, 45, 450, DateTimeKind.Utc).AddTicks(4248) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 26, 7, 40, 45, 450, DateTimeKind.Utc).AddTicks(4249), new DateTime(2025, 8, 26, 7, 40, 45, 450, DateTimeKind.Utc).AddTicks(4249) });

            migrationBuilder.UpdateData(
                table: "SubscriptionTypes",
                keyColumn: "SubscriptionTypeId",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 26, 7, 40, 45, 457, DateTimeKind.Utc).AddTicks(7188), new DateTime(2025, 8, 26, 7, 40, 45, 457, DateTimeKind.Utc).AddTicks(7191) });

            migrationBuilder.UpdateData(
                table: "SubscriptionTypes",
                keyColumn: "SubscriptionTypeId",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 26, 7, 40, 45, 457, DateTimeKind.Utc).AddTicks(7202), new DateTime(2025, 8, 26, 7, 40, 45, 457, DateTimeKind.Utc).AddTicks(7203) });

            migrationBuilder.UpdateData(
                table: "SubscriptionTypes",
                keyColumn: "SubscriptionTypeId",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 26, 7, 40, 45, 457, DateTimeKind.Utc).AddTicks(7207), new DateTime(2025, 8, 26, 7, 40, 45, 457, DateTimeKind.Utc).AddTicks(7207) });
        }
    }
}
