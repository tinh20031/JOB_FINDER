using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JOB_FINDER_API.Migrations
{
    /// <inheritdoc />
    public partial class upweweqqdfffwdawwa : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "JobId",
                table: "Embeddings",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "CandidateToCompanyRequests",
                type: "int",
                nullable: false,
                defaultValue: 0);

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

            migrationBuilder.CreateIndex(
                name: "IX_Embeddings_JobId",
                table: "Embeddings",
                column: "JobId");

            migrationBuilder.AddForeignKey(
                name: "FK_Embeddings_Jobs_JobId",
                table: "Embeddings",
                column: "JobId",
                principalTable: "Jobs",
                principalColumn: "JobId",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Embeddings_Jobs_JobId",
                table: "Embeddings");

            migrationBuilder.DropIndex(
                name: "IX_Embeddings_JobId",
                table: "Embeddings");

            migrationBuilder.DropColumn(
                name: "JobId",
                table: "Embeddings");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "CandidateToCompanyRequests");

            migrationBuilder.UpdateData(
                table: "CompanySubscriptionTypes",
                keyColumn: "CompanySubscriptionTypeId",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 23, 22, 20, 14, 459, DateTimeKind.Unspecified).AddTicks(5382), new DateTime(2025, 8, 23, 22, 20, 14, 459, DateTimeKind.Unspecified).AddTicks(5403) });

            migrationBuilder.UpdateData(
                table: "CompanySubscriptionTypes",
                keyColumn: "CompanySubscriptionTypeId",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 23, 22, 20, 14, 459, DateTimeKind.Unspecified).AddTicks(5407), new DateTime(2025, 8, 23, 22, 20, 14, 459, DateTimeKind.Unspecified).AddTicks(5408) });

            migrationBuilder.UpdateData(
                table: "CompanySubscriptionTypes",
                keyColumn: "CompanySubscriptionTypeId",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 23, 22, 20, 14, 459, DateTimeKind.Unspecified).AddTicks(5412), new DateTime(2025, 8, 23, 22, 20, 14, 459, DateTimeKind.Unspecified).AddTicks(5414) });

            migrationBuilder.UpdateData(
                table: "Industries",
                keyColumn: "IndustryId",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 23, 22, 20, 14, 452, DateTimeKind.Unspecified).AddTicks(8072), new DateTime(2025, 8, 23, 22, 20, 14, 452, DateTimeKind.Unspecified).AddTicks(8098) });

            migrationBuilder.UpdateData(
                table: "Industries",
                keyColumn: "IndustryId",
                keyValue: 4,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 23, 22, 20, 14, 452, DateTimeKind.Unspecified).AddTicks(8101), new DateTime(2025, 8, 23, 22, 20, 14, 452, DateTimeKind.Unspecified).AddTicks(8102) });

            migrationBuilder.UpdateData(
                table: "Industries",
                keyColumn: "IndustryId",
                keyValue: 5,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 23, 22, 20, 14, 452, DateTimeKind.Unspecified).AddTicks(8106), new DateTime(2025, 8, 23, 22, 20, 14, 452, DateTimeKind.Unspecified).AddTicks(8107) });

            migrationBuilder.UpdateData(
                table: "Industries",
                keyColumn: "IndustryId",
                keyValue: 6,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 23, 22, 20, 14, 452, DateTimeKind.Unspecified).AddTicks(8109), new DateTime(2025, 8, 23, 22, 20, 14, 452, DateTimeKind.Unspecified).AddTicks(8111) });

            migrationBuilder.UpdateData(
                table: "Industries",
                keyColumn: "IndustryId",
                keyValue: 7,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 23, 22, 20, 14, 452, DateTimeKind.Unspecified).AddTicks(8113), new DateTime(2025, 8, 23, 22, 20, 14, 452, DateTimeKind.Unspecified).AddTicks(8115) });

            migrationBuilder.UpdateData(
                table: "Industries",
                keyColumn: "IndustryId",
                keyValue: 8,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 23, 22, 20, 14, 452, DateTimeKind.Unspecified).AddTicks(8117), new DateTime(2025, 8, 23, 22, 20, 14, 452, DateTimeKind.Unspecified).AddTicks(8118) });

            migrationBuilder.UpdateData(
                table: "Industries",
                keyColumn: "IndustryId",
                keyValue: 9,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 23, 22, 20, 14, 452, DateTimeKind.Unspecified).AddTicks(8121), new DateTime(2025, 8, 23, 22, 20, 14, 452, DateTimeKind.Unspecified).AddTicks(8122) });

            migrationBuilder.UpdateData(
                table: "JobTypes",
                keyColumn: "JobTypeId",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 23, 22, 20, 14, 452, DateTimeKind.Unspecified).AddTicks(8170), new DateTime(2025, 8, 23, 22, 20, 14, 452, DateTimeKind.Unspecified).AddTicks(8171) });

            migrationBuilder.UpdateData(
                table: "JobTypes",
                keyColumn: "JobTypeId",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 23, 22, 20, 14, 452, DateTimeKind.Unspecified).AddTicks(8176), new DateTime(2025, 8, 23, 22, 20, 14, 452, DateTimeKind.Unspecified).AddTicks(8177) });

            migrationBuilder.UpdateData(
                table: "JobTypes",
                keyColumn: "JobTypeId",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 23, 22, 20, 14, 452, DateTimeKind.Unspecified).AddTicks(8180), new DateTime(2025, 8, 23, 22, 20, 14, 452, DateTimeKind.Unspecified).AddTicks(8181) });

            migrationBuilder.UpdateData(
                table: "Levels",
                keyColumn: "LevelId",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 23, 22, 20, 14, 452, DateTimeKind.Unspecified).AddTicks(8225), new DateTime(2025, 8, 23, 22, 20, 14, 452, DateTimeKind.Unspecified).AddTicks(8226) });

            migrationBuilder.UpdateData(
                table: "Levels",
                keyColumn: "LevelId",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 23, 22, 20, 14, 452, DateTimeKind.Unspecified).AddTicks(8230), new DateTime(2025, 8, 23, 22, 20, 14, 452, DateTimeKind.Unspecified).AddTicks(8231) });

            migrationBuilder.UpdateData(
                table: "Levels",
                keyColumn: "LevelId",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 23, 22, 20, 14, 452, DateTimeKind.Unspecified).AddTicks(8234), new DateTime(2025, 8, 23, 22, 20, 14, 452, DateTimeKind.Unspecified).AddTicks(8236) });

            migrationBuilder.UpdateData(
                table: "Levels",
                keyColumn: "LevelId",
                keyValue: 4,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 23, 22, 20, 14, 452, DateTimeKind.Unspecified).AddTicks(8238), new DateTime(2025, 8, 23, 22, 20, 14, 452, DateTimeKind.Unspecified).AddTicks(8240) });

            migrationBuilder.UpdateData(
                table: "Levels",
                keyColumn: "LevelId",
                keyValue: 5,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 23, 22, 20, 14, 452, DateTimeKind.Unspecified).AddTicks(8242), new DateTime(2025, 8, 23, 22, 20, 14, 452, DateTimeKind.Unspecified).AddTicks(8243) });

            migrationBuilder.UpdateData(
                table: "Levels",
                keyColumn: "LevelId",
                keyValue: 6,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 23, 22, 20, 14, 452, DateTimeKind.Unspecified).AddTicks(8246), new DateTime(2025, 8, 23, 22, 20, 14, 452, DateTimeKind.Unspecified).AddTicks(8248) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 23, 15, 20, 14, 452, DateTimeKind.Utc).AddTicks(7823), new DateTime(2025, 8, 23, 15, 20, 14, 452, DateTimeKind.Utc).AddTicks(7827) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 23, 15, 20, 14, 452, DateTimeKind.Utc).AddTicks(7833), new DateTime(2025, 8, 23, 15, 20, 14, 452, DateTimeKind.Utc).AddTicks(7833) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 23, 15, 20, 14, 452, DateTimeKind.Utc).AddTicks(7834), new DateTime(2025, 8, 23, 15, 20, 14, 452, DateTimeKind.Utc).AddTicks(7834) });

            migrationBuilder.UpdateData(
                table: "SubscriptionTypes",
                keyColumn: "SubscriptionTypeId",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 23, 15, 20, 14, 459, DateTimeKind.Utc).AddTicks(5266), new DateTime(2025, 8, 23, 15, 20, 14, 459, DateTimeKind.Utc).AddTicks(5270) });

            migrationBuilder.UpdateData(
                table: "SubscriptionTypes",
                keyColumn: "SubscriptionTypeId",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 23, 15, 20, 14, 459, DateTimeKind.Utc).AddTicks(5277), new DateTime(2025, 8, 23, 15, 20, 14, 459, DateTimeKind.Utc).AddTicks(5278) });

            migrationBuilder.UpdateData(
                table: "SubscriptionTypes",
                keyColumn: "SubscriptionTypeId",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 23, 15, 20, 14, 459, DateTimeKind.Utc).AddTicks(5283), new DateTime(2025, 8, 23, 15, 20, 14, 459, DateTimeKind.Utc).AddTicks(5284) });
        }
    }
}
