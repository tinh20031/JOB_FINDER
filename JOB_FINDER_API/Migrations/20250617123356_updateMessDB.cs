using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JOB_FINDER_API.Migrations
{
    /// <inheritdoc />
    public partial class updateMessDB : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Messages_Jobs_RelatedJobId",
                table: "Messages");

            migrationBuilder.DropIndex(
                name: "IX_Messages_RelatedJobId",
                table: "Messages");

            migrationBuilder.DropColumn(
                name: "RelatedJobId",
                table: "Messages");

            migrationBuilder.UpdateData(
                table: "Industries",
                keyColumn: "IndustryId",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 17, 12, 33, 55, 660, DateTimeKind.Utc).AddTicks(3343), new DateTime(2025, 6, 17, 12, 33, 55, 660, DateTimeKind.Utc).AddTicks(3343) });

            migrationBuilder.UpdateData(
                table: "Industries",
                keyColumn: "IndustryId",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 17, 12, 33, 55, 660, DateTimeKind.Utc).AddTicks(3345), new DateTime(2025, 6, 17, 12, 33, 55, 660, DateTimeKind.Utc).AddTicks(3346) });

            migrationBuilder.UpdateData(
                table: "JobTypes",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 17, 12, 33, 55, 660, DateTimeKind.Utc).AddTicks(3377), new DateTime(2025, 6, 17, 12, 33, 55, 660, DateTimeKind.Utc).AddTicks(3377) });

            migrationBuilder.UpdateData(
                table: "JobTypes",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 17, 12, 33, 55, 660, DateTimeKind.Utc).AddTicks(3380), new DateTime(2025, 6, 17, 12, 33, 55, 660, DateTimeKind.Utc).AddTicks(3380) });

            migrationBuilder.UpdateData(
                table: "JobTypes",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 17, 12, 33, 55, 660, DateTimeKind.Utc).AddTicks(3382), new DateTime(2025, 6, 17, 12, 33, 55, 660, DateTimeKind.Utc).AddTicks(3382) });

            migrationBuilder.UpdateData(
                table: "Levels",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 17, 12, 33, 55, 660, DateTimeKind.Utc).AddTicks(3409), new DateTime(2025, 6, 17, 12, 33, 55, 660, DateTimeKind.Utc).AddTicks(3410) });

            migrationBuilder.UpdateData(
                table: "Levels",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 17, 12, 33, 55, 660, DateTimeKind.Utc).AddTicks(3412), new DateTime(2025, 6, 17, 12, 33, 55, 660, DateTimeKind.Utc).AddTicks(3412) });

            migrationBuilder.UpdateData(
                table: "Levels",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 17, 12, 33, 55, 660, DateTimeKind.Utc).AddTicks(3414), new DateTime(2025, 6, 17, 12, 33, 55, 660, DateTimeKind.Utc).AddTicks(3414) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 17, 12, 33, 55, 660, DateTimeKind.Utc).AddTicks(3205), new DateTime(2025, 6, 17, 12, 33, 55, 660, DateTimeKind.Utc).AddTicks(3208) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 17, 12, 33, 55, 660, DateTimeKind.Utc).AddTicks(3213), new DateTime(2025, 6, 17, 12, 33, 55, 660, DateTimeKind.Utc).AddTicks(3213) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 17, 12, 33, 55, 660, DateTimeKind.Utc).AddTicks(3214), new DateTime(2025, 6, 17, 12, 33, 55, 660, DateTimeKind.Utc).AddTicks(3214) });

            migrationBuilder.UpdateData(
                table: "Skills",
                keyColumn: "SkillId",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 17, 12, 33, 55, 660, DateTimeKind.Utc).AddTicks(3444), new DateTime(2025, 6, 17, 12, 33, 55, 660, DateTimeKind.Utc).AddTicks(3445) });

            migrationBuilder.UpdateData(
                table: "Skills",
                keyColumn: "SkillId",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 17, 12, 33, 55, 660, DateTimeKind.Utc).AddTicks(3447), new DateTime(2025, 6, 17, 12, 33, 55, 660, DateTimeKind.Utc).AddTicks(3447) });

            migrationBuilder.UpdateData(
                table: "Skills",
                keyColumn: "SkillId",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 17, 12, 33, 55, 660, DateTimeKind.Utc).AddTicks(3449), new DateTime(2025, 6, 17, 12, 33, 55, 660, DateTimeKind.Utc).AddTicks(3449) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RelatedJobId",
                table: "Messages",
                type: "int",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Industries",
                keyColumn: "IndustryId",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 17, 12, 29, 37, 275, DateTimeKind.Utc).AddTicks(2140), new DateTime(2025, 6, 17, 12, 29, 37, 275, DateTimeKind.Utc).AddTicks(2141) });

            migrationBuilder.UpdateData(
                table: "Industries",
                keyColumn: "IndustryId",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 17, 12, 29, 37, 275, DateTimeKind.Utc).AddTicks(2144), new DateTime(2025, 6, 17, 12, 29, 37, 275, DateTimeKind.Utc).AddTicks(2144) });

            migrationBuilder.UpdateData(
                table: "JobTypes",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 17, 12, 29, 37, 275, DateTimeKind.Utc).AddTicks(2188), new DateTime(2025, 6, 17, 12, 29, 37, 275, DateTimeKind.Utc).AddTicks(2188) });

            migrationBuilder.UpdateData(
                table: "JobTypes",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 17, 12, 29, 37, 275, DateTimeKind.Utc).AddTicks(2191), new DateTime(2025, 6, 17, 12, 29, 37, 275, DateTimeKind.Utc).AddTicks(2191) });

            migrationBuilder.UpdateData(
                table: "JobTypes",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 17, 12, 29, 37, 275, DateTimeKind.Utc).AddTicks(2193), new DateTime(2025, 6, 17, 12, 29, 37, 275, DateTimeKind.Utc).AddTicks(2194) });

            migrationBuilder.UpdateData(
                table: "Levels",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 17, 12, 29, 37, 275, DateTimeKind.Utc).AddTicks(2228), new DateTime(2025, 6, 17, 12, 29, 37, 275, DateTimeKind.Utc).AddTicks(2228) });

            migrationBuilder.UpdateData(
                table: "Levels",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 17, 12, 29, 37, 275, DateTimeKind.Utc).AddTicks(2231), new DateTime(2025, 6, 17, 12, 29, 37, 275, DateTimeKind.Utc).AddTicks(2231) });

            migrationBuilder.UpdateData(
                table: "Levels",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 17, 12, 29, 37, 275, DateTimeKind.Utc).AddTicks(2233), new DateTime(2025, 6, 17, 12, 29, 37, 275, DateTimeKind.Utc).AddTicks(2233) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 17, 12, 29, 37, 275, DateTimeKind.Utc).AddTicks(1949), new DateTime(2025, 6, 17, 12, 29, 37, 275, DateTimeKind.Utc).AddTicks(1952) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 17, 12, 29, 37, 275, DateTimeKind.Utc).AddTicks(1957), new DateTime(2025, 6, 17, 12, 29, 37, 275, DateTimeKind.Utc).AddTicks(1957) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 17, 12, 29, 37, 275, DateTimeKind.Utc).AddTicks(1958), new DateTime(2025, 6, 17, 12, 29, 37, 275, DateTimeKind.Utc).AddTicks(1958) });

            migrationBuilder.UpdateData(
                table: "Skills",
                keyColumn: "SkillId",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 17, 12, 29, 37, 275, DateTimeKind.Utc).AddTicks(2276), new DateTime(2025, 6, 17, 12, 29, 37, 275, DateTimeKind.Utc).AddTicks(2277) });

            migrationBuilder.UpdateData(
                table: "Skills",
                keyColumn: "SkillId",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 17, 12, 29, 37, 275, DateTimeKind.Utc).AddTicks(2279), new DateTime(2025, 6, 17, 12, 29, 37, 275, DateTimeKind.Utc).AddTicks(2279) });

            migrationBuilder.UpdateData(
                table: "Skills",
                keyColumn: "SkillId",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 17, 12, 29, 37, 275, DateTimeKind.Utc).AddTicks(2281), new DateTime(2025, 6, 17, 12, 29, 37, 275, DateTimeKind.Utc).AddTicks(2281) });

            migrationBuilder.CreateIndex(
                name: "IX_Messages_RelatedJobId",
                table: "Messages",
                column: "RelatedJobId");

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_Jobs_RelatedJobId",
                table: "Messages",
                column: "RelatedJobId",
                principalTable: "Jobs",
                principalColumn: "JobId",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
