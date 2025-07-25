using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JOB_FINDER_API.Migrations
{
    /// <inheritdoc />
    public partial class updateDbCompnayPay : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PaymentType",
                table: "Payments",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "CompanySubscriptionTypes",
                columns: table => new
                {
                    CompanySubscriptionTypeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PackageType = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    JobPostLimit = table.Column<int>(type: "int", nullable: false),
                    CvMatchLimit = table.Column<int>(type: "int", nullable: false),
                    DurationInDays = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompanySubscriptionTypes", x => x.CompanySubscriptionTypeId);
                });

            migrationBuilder.CreateTable(
                name: "CompanySubscriptions",
                columns: table => new
                {
                    CompanySubscriptionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    CompanySubscriptionTypeId = table.Column<int>(type: "int", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    RemainingJobPosts = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompanySubscriptions", x => x.CompanySubscriptionId);
                    table.ForeignKey(
                        name: "FK_CompanySubscriptions_CompanySubscriptionTypes_CompanySubscriptionTypeId",
                        column: x => x.CompanySubscriptionTypeId,
                        principalTable: "CompanySubscriptionTypes",
                        principalColumn: "CompanySubscriptionTypeId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CompanySubscriptions_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

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

            migrationBuilder.CreateIndex(
                name: "IX_CompanySubscriptions_CompanySubscriptionTypeId",
                table: "CompanySubscriptions",
                column: "CompanySubscriptionTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_CompanySubscriptions_UserId",
                table: "CompanySubscriptions",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CompanySubscriptions");

            migrationBuilder.DropTable(
                name: "CompanySubscriptionTypes");

            migrationBuilder.DropColumn(
                name: "PaymentType",
                table: "Payments");

            migrationBuilder.UpdateData(
                table: "Industries",
                keyColumn: "IndustryId",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 24, 21, 55, 39, 967, DateTimeKind.Unspecified).AddTicks(9929), new DateTime(2025, 7, 24, 21, 55, 39, 967, DateTimeKind.Unspecified).AddTicks(9954) });

            migrationBuilder.UpdateData(
                table: "Industries",
                keyColumn: "IndustryId",
                keyValue: 4,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 24, 21, 55, 39, 967, DateTimeKind.Unspecified).AddTicks(9958), new DateTime(2025, 7, 24, 21, 55, 39, 967, DateTimeKind.Unspecified).AddTicks(9959) });

            migrationBuilder.UpdateData(
                table: "Industries",
                keyColumn: "IndustryId",
                keyValue: 5,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 24, 21, 55, 39, 967, DateTimeKind.Unspecified).AddTicks(9962), new DateTime(2025, 7, 24, 21, 55, 39, 967, DateTimeKind.Unspecified).AddTicks(9963) });

            migrationBuilder.UpdateData(
                table: "Industries",
                keyColumn: "IndustryId",
                keyValue: 6,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 24, 21, 55, 39, 967, DateTimeKind.Unspecified).AddTicks(9966), new DateTime(2025, 7, 24, 21, 55, 39, 967, DateTimeKind.Unspecified).AddTicks(9967) });

            migrationBuilder.UpdateData(
                table: "Industries",
                keyColumn: "IndustryId",
                keyValue: 7,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 24, 21, 55, 39, 967, DateTimeKind.Unspecified).AddTicks(9969), new DateTime(2025, 7, 24, 21, 55, 39, 967, DateTimeKind.Unspecified).AddTicks(9970) });

            migrationBuilder.UpdateData(
                table: "Industries",
                keyColumn: "IndustryId",
                keyValue: 8,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 24, 21, 55, 39, 967, DateTimeKind.Unspecified).AddTicks(9973), new DateTime(2025, 7, 24, 21, 55, 39, 967, DateTimeKind.Unspecified).AddTicks(9974) });

            migrationBuilder.UpdateData(
                table: "Industries",
                keyColumn: "IndustryId",
                keyValue: 9,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 24, 21, 55, 39, 967, DateTimeKind.Unspecified).AddTicks(9976), new DateTime(2025, 7, 24, 21, 55, 39, 967, DateTimeKind.Unspecified).AddTicks(9977) });

            migrationBuilder.UpdateData(
                table: "JobTypes",
                keyColumn: "JobTypeId",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 24, 21, 55, 39, 968, DateTimeKind.Unspecified).AddTicks(19), new DateTime(2025, 7, 24, 21, 55, 39, 968, DateTimeKind.Unspecified).AddTicks(21) });

            migrationBuilder.UpdateData(
                table: "JobTypes",
                keyColumn: "JobTypeId",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 24, 21, 55, 39, 968, DateTimeKind.Unspecified).AddTicks(24), new DateTime(2025, 7, 24, 21, 55, 39, 968, DateTimeKind.Unspecified).AddTicks(26) });

            migrationBuilder.UpdateData(
                table: "JobTypes",
                keyColumn: "JobTypeId",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 24, 21, 55, 39, 968, DateTimeKind.Unspecified).AddTicks(28), new DateTime(2025, 7, 24, 21, 55, 39, 968, DateTimeKind.Unspecified).AddTicks(30) });

            migrationBuilder.UpdateData(
                table: "Levels",
                keyColumn: "LevelId",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 24, 21, 55, 39, 968, DateTimeKind.Unspecified).AddTicks(63), new DateTime(2025, 7, 24, 21, 55, 39, 968, DateTimeKind.Unspecified).AddTicks(65) });

            migrationBuilder.UpdateData(
                table: "Levels",
                keyColumn: "LevelId",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 24, 21, 55, 39, 968, DateTimeKind.Unspecified).AddTicks(69), new DateTime(2025, 7, 24, 21, 55, 39, 968, DateTimeKind.Unspecified).AddTicks(70) });

            migrationBuilder.UpdateData(
                table: "Levels",
                keyColumn: "LevelId",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 24, 21, 55, 39, 968, DateTimeKind.Unspecified).AddTicks(73), new DateTime(2025, 7, 24, 21, 55, 39, 968, DateTimeKind.Unspecified).AddTicks(74) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 24, 14, 55, 39, 967, DateTimeKind.Utc).AddTicks(9735), new DateTime(2025, 7, 24, 14, 55, 39, 967, DateTimeKind.Utc).AddTicks(9738) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 24, 14, 55, 39, 967, DateTimeKind.Utc).AddTicks(9744), new DateTime(2025, 7, 24, 14, 55, 39, 967, DateTimeKind.Utc).AddTicks(9744) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 24, 14, 55, 39, 967, DateTimeKind.Utc).AddTicks(9745), new DateTime(2025, 7, 24, 14, 55, 39, 967, DateTimeKind.Utc).AddTicks(9745) });

            migrationBuilder.UpdateData(
                table: "SubscriptionTypes",
                keyColumn: "SubscriptionTypeId",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 24, 14, 55, 39, 974, DateTimeKind.Utc).AddTicks(847), new DateTime(2025, 7, 24, 14, 55, 39, 974, DateTimeKind.Utc).AddTicks(851) });

            migrationBuilder.UpdateData(
                table: "SubscriptionTypes",
                keyColumn: "SubscriptionTypeId",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 24, 14, 55, 39, 974, DateTimeKind.Utc).AddTicks(858), new DateTime(2025, 7, 24, 14, 55, 39, 974, DateTimeKind.Utc).AddTicks(859) });

            migrationBuilder.UpdateData(
                table: "SubscriptionTypes",
                keyColumn: "SubscriptionTypeId",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 24, 14, 55, 39, 974, DateTimeKind.Utc).AddTicks(863), new DateTime(2025, 7, 24, 14, 55, 39, 974, DateTimeKind.Utc).AddTicks(863) });
        }
    }
}
