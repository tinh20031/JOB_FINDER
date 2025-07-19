using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace JOB_FINDER_API.Migrations
{
    /// <inheritdoc />
    public partial class updatePayOS : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SubscriptionPlans",
                columns: table => new
                {
                    PlanId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DurationDays = table.Column<int>(type: "int", nullable: false),
                    MatchingLimit = table.Column<int>(type: "int", nullable: false),
                    CVFeedbackLimit = table.Column<int>(type: "int", nullable: false),
                    CVDownloadLimit = table.Column<int>(type: "int", nullable: false),
                    AllowWatermarkRemoval = table.Column<bool>(type: "bit", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubscriptionPlans", x => x.PlanId);
                });

            migrationBuilder.CreateTable(
                name: "CandidateSubscriptions",
                columns: table => new
                {
                    SubscriptionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    PlanId = table.Column<int>(type: "int", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    MatchingUsed = table.Column<int>(type: "int", nullable: false),
                    CVFeedbackUsed = table.Column<int>(type: "int", nullable: false),
                    CVDownloaded = table.Column<int>(type: "int", nullable: false),
                    TransactionId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AmountPaid = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CandidateSubscriptions", x => x.SubscriptionId);
                    table.ForeignKey(
                        name: "FK_CandidateSubscriptions_SubscriptionPlans_PlanId",
                        column: x => x.PlanId,
                        principalTable: "SubscriptionPlans",
                        principalColumn: "PlanId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CandidateSubscriptions_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SubscriptionTransactions",
                columns: table => new
                {
                    TransactionId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    PlanId = table.Column<int>(type: "int", nullable: true),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    PaymentMethod = table.Column<int>(type: "int", nullable: false),
                    PaymentReference = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubscriptionTransactions", x => x.TransactionId);
                    table.ForeignKey(
                        name: "FK_SubscriptionTransactions_SubscriptionPlans_PlanId",
                        column: x => x.PlanId,
                        principalTable: "SubscriptionPlans",
                        principalColumn: "PlanId",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_SubscriptionTransactions_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

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

            migrationBuilder.InsertData(
                table: "SubscriptionPlans",
                columns: new[] { "PlanId", "AllowWatermarkRemoval", "CVDownloadLimit", "CVFeedbackLimit", "CreatedAt", "Description", "DurationDays", "IsActive", "MatchingLimit", "Name", "Price", "Type", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, false, 1, 1, new DateTime(2025, 7, 18, 16, 15, 4, 610, DateTimeKind.Utc).AddTicks(2244), "Gói dịch vụ miễn phí với các tính năng cơ bản", 0, true, 1, "Gói Miễn Phí", 0m, 0, new DateTime(2025, 7, 18, 16, 15, 4, 610, DateTimeKind.Utc).AddTicks(2246) },
                    { 2, true, 3, 3, new DateTime(2025, 7, 18, 16, 15, 4, 610, DateTimeKind.Utc).AddTicks(2255), "Gói dịch vụ cơ bản với các tính năng nâng cao", 30, true, 3, "Gói Cơ Bản", 10000m, 1, new DateTime(2025, 7, 18, 16, 15, 4, 610, DateTimeKind.Utc).AddTicks(2255) },
                    { 3, true, -1, 7, new DateTime(2025, 7, 18, 16, 15, 4, 610, DateTimeKind.Utc).AddTicks(2260), "Gói dịch vụ nâng cao với đầy đủ tính năng và không giới hạn tải CV", 30, true, 7, "Gói Nâng Cao", 20000m, 2, new DateTime(2025, 7, 18, 16, 15, 4, 610, DateTimeKind.Utc).AddTicks(2260) }
                });

            migrationBuilder.CreateIndex(
                name: "IX_CandidateSubscriptions_PlanId",
                table: "CandidateSubscriptions",
                column: "PlanId");

            migrationBuilder.CreateIndex(
                name: "IX_CandidateSubscriptions_UserId",
                table: "CandidateSubscriptions",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_SubscriptionTransactions_PlanId",
                table: "SubscriptionTransactions",
                column: "PlanId");

            migrationBuilder.CreateIndex(
                name: "IX_SubscriptionTransactions_UserId",
                table: "SubscriptionTransactions",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CandidateSubscriptions");

            migrationBuilder.DropTable(
                name: "SubscriptionTransactions");

            migrationBuilder.DropTable(
                name: "SubscriptionPlans");

            migrationBuilder.UpdateData(
                table: "Industries",
                keyColumn: "IndustryId",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 18, 17, 13, 41, 336, DateTimeKind.Unspecified).AddTicks(4909), new DateTime(2025, 7, 18, 17, 13, 41, 336, DateTimeKind.Unspecified).AddTicks(4937) });

            migrationBuilder.UpdateData(
                table: "Industries",
                keyColumn: "IndustryId",
                keyValue: 4,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 18, 17, 13, 41, 336, DateTimeKind.Unspecified).AddTicks(4941), new DateTime(2025, 7, 18, 17, 13, 41, 336, DateTimeKind.Unspecified).AddTicks(4943) });

            migrationBuilder.UpdateData(
                table: "Industries",
                keyColumn: "IndustryId",
                keyValue: 5,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 18, 17, 13, 41, 336, DateTimeKind.Unspecified).AddTicks(4945), new DateTime(2025, 7, 18, 17, 13, 41, 336, DateTimeKind.Unspecified).AddTicks(4946) });

            migrationBuilder.UpdateData(
                table: "Industries",
                keyColumn: "IndustryId",
                keyValue: 6,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 18, 17, 13, 41, 336, DateTimeKind.Unspecified).AddTicks(4949), new DateTime(2025, 7, 18, 17, 13, 41, 336, DateTimeKind.Unspecified).AddTicks(4950) });

            migrationBuilder.UpdateData(
                table: "Industries",
                keyColumn: "IndustryId",
                keyValue: 7,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 18, 17, 13, 41, 336, DateTimeKind.Unspecified).AddTicks(4952), new DateTime(2025, 7, 18, 17, 13, 41, 336, DateTimeKind.Unspecified).AddTicks(4953) });

            migrationBuilder.UpdateData(
                table: "Industries",
                keyColumn: "IndustryId",
                keyValue: 8,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 18, 17, 13, 41, 336, DateTimeKind.Unspecified).AddTicks(4955), new DateTime(2025, 7, 18, 17, 13, 41, 336, DateTimeKind.Unspecified).AddTicks(4956) });

            migrationBuilder.UpdateData(
                table: "Industries",
                keyColumn: "IndustryId",
                keyValue: 9,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 18, 17, 13, 41, 336, DateTimeKind.Unspecified).AddTicks(4958), new DateTime(2025, 7, 18, 17, 13, 41, 336, DateTimeKind.Unspecified).AddTicks(4959) });

            migrationBuilder.UpdateData(
                table: "JobTypes",
                keyColumn: "JobTypeId",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 18, 17, 13, 41, 336, DateTimeKind.Unspecified).AddTicks(5002), new DateTime(2025, 7, 18, 17, 13, 41, 336, DateTimeKind.Unspecified).AddTicks(5005) });

            migrationBuilder.UpdateData(
                table: "JobTypes",
                keyColumn: "JobTypeId",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 18, 17, 13, 41, 336, DateTimeKind.Unspecified).AddTicks(5007), new DateTime(2025, 7, 18, 17, 13, 41, 336, DateTimeKind.Unspecified).AddTicks(5009) });

            migrationBuilder.UpdateData(
                table: "JobTypes",
                keyColumn: "JobTypeId",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 18, 17, 13, 41, 336, DateTimeKind.Unspecified).AddTicks(5011), new DateTime(2025, 7, 18, 17, 13, 41, 336, DateTimeKind.Unspecified).AddTicks(5012) });

            migrationBuilder.UpdateData(
                table: "Levels",
                keyColumn: "LevelId",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 18, 17, 13, 41, 336, DateTimeKind.Unspecified).AddTicks(5047), new DateTime(2025, 7, 18, 17, 13, 41, 336, DateTimeKind.Unspecified).AddTicks(5049) });

            migrationBuilder.UpdateData(
                table: "Levels",
                keyColumn: "LevelId",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 18, 17, 13, 41, 336, DateTimeKind.Unspecified).AddTicks(5053), new DateTime(2025, 7, 18, 17, 13, 41, 336, DateTimeKind.Unspecified).AddTicks(5054) });

            migrationBuilder.UpdateData(
                table: "Levels",
                keyColumn: "LevelId",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 18, 17, 13, 41, 336, DateTimeKind.Unspecified).AddTicks(5057), new DateTime(2025, 7, 18, 17, 13, 41, 336, DateTimeKind.Unspecified).AddTicks(5058) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 18, 10, 13, 41, 336, DateTimeKind.Utc).AddTicks(4715), new DateTime(2025, 7, 18, 10, 13, 41, 336, DateTimeKind.Utc).AddTicks(4718) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 18, 10, 13, 41, 336, DateTimeKind.Utc).AddTicks(4724), new DateTime(2025, 7, 18, 10, 13, 41, 336, DateTimeKind.Utc).AddTicks(4725) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 18, 10, 13, 41, 336, DateTimeKind.Utc).AddTicks(4725), new DateTime(2025, 7, 18, 10, 13, 41, 336, DateTimeKind.Utc).AddTicks(4726) });
        }
    }
}
