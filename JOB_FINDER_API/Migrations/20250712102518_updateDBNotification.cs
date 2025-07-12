using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JOB_FINDER_API.Migrations
{
    /// <inheritdoc />
    public partial class updateDBNotification : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Notifications",
                columns: table => new
                {
                    NotificationId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Link = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Type = table.Column<int>(type: "int", nullable: false),
                    IsRead = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notifications", x => x.NotificationId);
                    table.ForeignKey(
                        name: "FK_Notifications_Users_UserId",
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
                values: new object[] { new DateTime(2025, 7, 12, 10, 25, 16, 119, DateTimeKind.Utc).AddTicks(2539), new DateTime(2025, 7, 12, 10, 25, 16, 119, DateTimeKind.Utc).AddTicks(2540) });

            migrationBuilder.UpdateData(
                table: "Industries",
                keyColumn: "IndustryId",
                keyValue: 4,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 12, 10, 25, 16, 119, DateTimeKind.Utc).AddTicks(2543), new DateTime(2025, 7, 12, 10, 25, 16, 119, DateTimeKind.Utc).AddTicks(2544) });

            migrationBuilder.UpdateData(
                table: "Industries",
                keyColumn: "IndustryId",
                keyValue: 5,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 12, 10, 25, 16, 119, DateTimeKind.Utc).AddTicks(2545), new DateTime(2025, 7, 12, 10, 25, 16, 119, DateTimeKind.Utc).AddTicks(2545) });

            migrationBuilder.UpdateData(
                table: "Industries",
                keyColumn: "IndustryId",
                keyValue: 6,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 12, 10, 25, 16, 119, DateTimeKind.Utc).AddTicks(2547), new DateTime(2025, 7, 12, 10, 25, 16, 119, DateTimeKind.Utc).AddTicks(2547) });

            migrationBuilder.UpdateData(
                table: "Industries",
                keyColumn: "IndustryId",
                keyValue: 7,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 12, 10, 25, 16, 119, DateTimeKind.Utc).AddTicks(2549), new DateTime(2025, 7, 12, 10, 25, 16, 119, DateTimeKind.Utc).AddTicks(2549) });

            migrationBuilder.UpdateData(
                table: "Industries",
                keyColumn: "IndustryId",
                keyValue: 8,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 12, 10, 25, 16, 119, DateTimeKind.Utc).AddTicks(2551), new DateTime(2025, 7, 12, 10, 25, 16, 119, DateTimeKind.Utc).AddTicks(2551) });

            migrationBuilder.UpdateData(
                table: "Industries",
                keyColumn: "IndustryId",
                keyValue: 9,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 12, 10, 25, 16, 119, DateTimeKind.Utc).AddTicks(2552), new DateTime(2025, 7, 12, 10, 25, 16, 119, DateTimeKind.Utc).AddTicks(2552) });

            migrationBuilder.UpdateData(
                table: "JobTypes",
                keyColumn: "JobTypeId",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 12, 10, 25, 16, 119, DateTimeKind.Utc).AddTicks(2597), new DateTime(2025, 7, 12, 10, 25, 16, 119, DateTimeKind.Utc).AddTicks(2599) });

            migrationBuilder.UpdateData(
                table: "JobTypes",
                keyColumn: "JobTypeId",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 12, 10, 25, 16, 119, DateTimeKind.Utc).AddTicks(2601), new DateTime(2025, 7, 12, 10, 25, 16, 119, DateTimeKind.Utc).AddTicks(2601) });

            migrationBuilder.UpdateData(
                table: "JobTypes",
                keyColumn: "JobTypeId",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 12, 10, 25, 16, 119, DateTimeKind.Utc).AddTicks(2603), new DateTime(2025, 7, 12, 10, 25, 16, 119, DateTimeKind.Utc).AddTicks(2603) });

            migrationBuilder.UpdateData(
                table: "Levels",
                keyColumn: "LevelId",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 12, 10, 25, 16, 119, DateTimeKind.Utc).AddTicks(2639), new DateTime(2025, 7, 12, 10, 25, 16, 119, DateTimeKind.Utc).AddTicks(2640) });

            migrationBuilder.UpdateData(
                table: "Levels",
                keyColumn: "LevelId",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 12, 10, 25, 16, 119, DateTimeKind.Utc).AddTicks(2643), new DateTime(2025, 7, 12, 10, 25, 16, 119, DateTimeKind.Utc).AddTicks(2643) });

            migrationBuilder.UpdateData(
                table: "Levels",
                keyColumn: "LevelId",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 12, 10, 25, 16, 119, DateTimeKind.Utc).AddTicks(2645), new DateTime(2025, 7, 12, 10, 25, 16, 119, DateTimeKind.Utc).AddTicks(2646) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 12, 10, 25, 16, 119, DateTimeKind.Utc).AddTicks(2310), new DateTime(2025, 7, 12, 10, 25, 16, 119, DateTimeKind.Utc).AddTicks(2312) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 12, 10, 25, 16, 119, DateTimeKind.Utc).AddTicks(2317), new DateTime(2025, 7, 12, 10, 25, 16, 119, DateTimeKind.Utc).AddTicks(2318) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 12, 10, 25, 16, 119, DateTimeKind.Utc).AddTicks(2319), new DateTime(2025, 7, 12, 10, 25, 16, 119, DateTimeKind.Utc).AddTicks(2319) });

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_UserId",
                table: "Notifications",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Notifications");

            migrationBuilder.UpdateData(
                table: "Industries",
                keyColumn: "IndustryId",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 11, 10, 51, 29, 286, DateTimeKind.Utc).AddTicks(6777), new DateTime(2025, 7, 11, 10, 51, 29, 286, DateTimeKind.Utc).AddTicks(6779) });

            migrationBuilder.UpdateData(
                table: "Industries",
                keyColumn: "IndustryId",
                keyValue: 4,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 11, 10, 51, 29, 286, DateTimeKind.Utc).AddTicks(6781), new DateTime(2025, 7, 11, 10, 51, 29, 286, DateTimeKind.Utc).AddTicks(6781) });

            migrationBuilder.UpdateData(
                table: "Industries",
                keyColumn: "IndustryId",
                keyValue: 5,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 11, 10, 51, 29, 286, DateTimeKind.Utc).AddTicks(6783), new DateTime(2025, 7, 11, 10, 51, 29, 286, DateTimeKind.Utc).AddTicks(6783) });

            migrationBuilder.UpdateData(
                table: "Industries",
                keyColumn: "IndustryId",
                keyValue: 6,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 11, 10, 51, 29, 286, DateTimeKind.Utc).AddTicks(6784), new DateTime(2025, 7, 11, 10, 51, 29, 286, DateTimeKind.Utc).AddTicks(6785) });

            migrationBuilder.UpdateData(
                table: "Industries",
                keyColumn: "IndustryId",
                keyValue: 7,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 11, 10, 51, 29, 286, DateTimeKind.Utc).AddTicks(6786), new DateTime(2025, 7, 11, 10, 51, 29, 286, DateTimeKind.Utc).AddTicks(6786) });

            migrationBuilder.UpdateData(
                table: "Industries",
                keyColumn: "IndustryId",
                keyValue: 8,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 11, 10, 51, 29, 286, DateTimeKind.Utc).AddTicks(6788), new DateTime(2025, 7, 11, 10, 51, 29, 286, DateTimeKind.Utc).AddTicks(6788) });

            migrationBuilder.UpdateData(
                table: "Industries",
                keyColumn: "IndustryId",
                keyValue: 9,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 11, 10, 51, 29, 286, DateTimeKind.Utc).AddTicks(6789), new DateTime(2025, 7, 11, 10, 51, 29, 286, DateTimeKind.Utc).AddTicks(6790) });

            migrationBuilder.UpdateData(
                table: "JobTypes",
                keyColumn: "JobTypeId",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 11, 10, 51, 29, 286, DateTimeKind.Utc).AddTicks(6831), new DateTime(2025, 7, 11, 10, 51, 29, 286, DateTimeKind.Utc).AddTicks(6831) });

            migrationBuilder.UpdateData(
                table: "JobTypes",
                keyColumn: "JobTypeId",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 11, 10, 51, 29, 286, DateTimeKind.Utc).AddTicks(6834), new DateTime(2025, 7, 11, 10, 51, 29, 286, DateTimeKind.Utc).AddTicks(6834) });

            migrationBuilder.UpdateData(
                table: "JobTypes",
                keyColumn: "JobTypeId",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 11, 10, 51, 29, 286, DateTimeKind.Utc).AddTicks(6836), new DateTime(2025, 7, 11, 10, 51, 29, 286, DateTimeKind.Utc).AddTicks(6836) });

            migrationBuilder.UpdateData(
                table: "Levels",
                keyColumn: "LevelId",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 11, 10, 51, 29, 286, DateTimeKind.Utc).AddTicks(6873), new DateTime(2025, 7, 11, 10, 51, 29, 286, DateTimeKind.Utc).AddTicks(6874) });

            migrationBuilder.UpdateData(
                table: "Levels",
                keyColumn: "LevelId",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 11, 10, 51, 29, 286, DateTimeKind.Utc).AddTicks(6875), new DateTime(2025, 7, 11, 10, 51, 29, 286, DateTimeKind.Utc).AddTicks(6876) });

            migrationBuilder.UpdateData(
                table: "Levels",
                keyColumn: "LevelId",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 11, 10, 51, 29, 286, DateTimeKind.Utc).AddTicks(6877), new DateTime(2025, 7, 11, 10, 51, 29, 286, DateTimeKind.Utc).AddTicks(6878) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 11, 10, 51, 29, 286, DateTimeKind.Utc).AddTicks(6601), new DateTime(2025, 7, 11, 10, 51, 29, 286, DateTimeKind.Utc).AddTicks(6604) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 11, 10, 51, 29, 286, DateTimeKind.Utc).AddTicks(6611), new DateTime(2025, 7, 11, 10, 51, 29, 286, DateTimeKind.Utc).AddTicks(6611) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 11, 10, 51, 29, 286, DateTimeKind.Utc).AddTicks(6612), new DateTime(2025, 7, 11, 10, 51, 29, 286, DateTimeKind.Utc).AddTicks(6613) });
        }
    }
}
