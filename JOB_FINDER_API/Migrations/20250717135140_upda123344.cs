using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JOB_FINDER_API.Migrations
{
    /// <inheritdoc />
    public partial class upda123344 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Jobs_ExperienceLevel_ExperienceLevelId",
                table: "Jobs");

            migrationBuilder.RenameColumn(
                name: "ExperienceLevelId",
                table: "Jobs",
                newName: "ExperienceLevelid");

            migrationBuilder.RenameIndex(
                name: "IX_Jobs_ExperienceLevelId",
                table: "Jobs",
                newName: "IX_Jobs_ExperienceLevelid");

            migrationBuilder.AlterColumn<int>(
                name: "ExperienceLevelid",
                table: "Jobs",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "Quantity",
                table: "Jobs",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "ExperienceLevel",
                keyColumn: "ExperienceLevelid",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 17, 13, 51, 31, 486, DateTimeKind.Utc).AddTicks(3847), new DateTime(2025, 7, 17, 13, 51, 31, 486, DateTimeKind.Utc).AddTicks(3848) });

            migrationBuilder.UpdateData(
                table: "ExperienceLevel",
                keyColumn: "ExperienceLevelid",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 17, 13, 51, 31, 486, DateTimeKind.Utc).AddTicks(3851), new DateTime(2025, 7, 17, 13, 51, 31, 486, DateTimeKind.Utc).AddTicks(3851) });

            migrationBuilder.UpdateData(
                table: "ExperienceLevel",
                keyColumn: "ExperienceLevelid",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 17, 13, 51, 31, 486, DateTimeKind.Utc).AddTicks(3852), new DateTime(2025, 7, 17, 13, 51, 31, 486, DateTimeKind.Utc).AddTicks(3852) });

            migrationBuilder.UpdateData(
                table: "ExperienceLevel",
                keyColumn: "ExperienceLevelid",
                keyValue: 4,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 17, 13, 51, 31, 486, DateTimeKind.Utc).AddTicks(3854), new DateTime(2025, 7, 17, 13, 51, 31, 486, DateTimeKind.Utc).AddTicks(3854) });

            migrationBuilder.UpdateData(
                table: "Industries",
                keyColumn: "IndustryId",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 17, 20, 51, 31, 486, DateTimeKind.Unspecified).AddTicks(3464), new DateTime(2025, 7, 17, 20, 51, 31, 486, DateTimeKind.Unspecified).AddTicks(3542) });

            migrationBuilder.UpdateData(
                table: "Industries",
                keyColumn: "IndustryId",
                keyValue: 4,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 17, 20, 51, 31, 486, DateTimeKind.Unspecified).AddTicks(3550), new DateTime(2025, 7, 17, 20, 51, 31, 486, DateTimeKind.Unspecified).AddTicks(3553) });

            migrationBuilder.UpdateData(
                table: "Industries",
                keyColumn: "IndustryId",
                keyValue: 5,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 17, 20, 51, 31, 486, DateTimeKind.Unspecified).AddTicks(3558), new DateTime(2025, 7, 17, 20, 51, 31, 486, DateTimeKind.Unspecified).AddTicks(3561) });

            migrationBuilder.UpdateData(
                table: "Industries",
                keyColumn: "IndustryId",
                keyValue: 6,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 17, 20, 51, 31, 486, DateTimeKind.Unspecified).AddTicks(3565), new DateTime(2025, 7, 17, 20, 51, 31, 486, DateTimeKind.Unspecified).AddTicks(3568) });

            migrationBuilder.UpdateData(
                table: "Industries",
                keyColumn: "IndustryId",
                keyValue: 7,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 17, 20, 51, 31, 486, DateTimeKind.Unspecified).AddTicks(3573), new DateTime(2025, 7, 17, 20, 51, 31, 486, DateTimeKind.Unspecified).AddTicks(3576) });

            migrationBuilder.UpdateData(
                table: "Industries",
                keyColumn: "IndustryId",
                keyValue: 8,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 17, 20, 51, 31, 486, DateTimeKind.Unspecified).AddTicks(3582), new DateTime(2025, 7, 17, 20, 51, 31, 486, DateTimeKind.Unspecified).AddTicks(3585) });

            migrationBuilder.UpdateData(
                table: "Industries",
                keyColumn: "IndustryId",
                keyValue: 9,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 17, 20, 51, 31, 486, DateTimeKind.Unspecified).AddTicks(3590), new DateTime(2025, 7, 17, 20, 51, 31, 486, DateTimeKind.Unspecified).AddTicks(3593) });

            migrationBuilder.UpdateData(
                table: "JobTypes",
                keyColumn: "JobTypeId",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 17, 20, 51, 31, 486, DateTimeKind.Unspecified).AddTicks(3698), new DateTime(2025, 7, 17, 20, 51, 31, 486, DateTimeKind.Unspecified).AddTicks(3703) });

            migrationBuilder.UpdateData(
                table: "JobTypes",
                keyColumn: "JobTypeId",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 17, 20, 51, 31, 486, DateTimeKind.Unspecified).AddTicks(3712), new DateTime(2025, 7, 17, 20, 51, 31, 486, DateTimeKind.Unspecified).AddTicks(3715) });

            migrationBuilder.UpdateData(
                table: "JobTypes",
                keyColumn: "JobTypeId",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 17, 20, 51, 31, 486, DateTimeKind.Unspecified).AddTicks(3719), new DateTime(2025, 7, 17, 20, 51, 31, 486, DateTimeKind.Unspecified).AddTicks(3722) });

            migrationBuilder.UpdateData(
                table: "Levels",
                keyColumn: "LevelId",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 17, 20, 51, 31, 486, DateTimeKind.Unspecified).AddTicks(3781), new DateTime(2025, 7, 17, 20, 51, 31, 486, DateTimeKind.Unspecified).AddTicks(3785) });

            migrationBuilder.UpdateData(
                table: "Levels",
                keyColumn: "LevelId",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 17, 20, 51, 31, 486, DateTimeKind.Unspecified).AddTicks(3791), new DateTime(2025, 7, 17, 20, 51, 31, 486, DateTimeKind.Unspecified).AddTicks(3794) });

            migrationBuilder.UpdateData(
                table: "Levels",
                keyColumn: "LevelId",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 17, 20, 51, 31, 486, DateTimeKind.Unspecified).AddTicks(3798), new DateTime(2025, 7, 17, 20, 51, 31, 486, DateTimeKind.Unspecified).AddTicks(3801) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 17, 13, 51, 31, 486, DateTimeKind.Utc).AddTicks(2693), new DateTime(2025, 7, 17, 13, 51, 31, 486, DateTimeKind.Utc).AddTicks(2706) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 17, 13, 51, 31, 486, DateTimeKind.Utc).AddTicks(2764), new DateTime(2025, 7, 17, 13, 51, 31, 486, DateTimeKind.Utc).AddTicks(2765) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 17, 13, 51, 31, 486, DateTimeKind.Utc).AddTicks(2766), new DateTime(2025, 7, 17, 13, 51, 31, 486, DateTimeKind.Utc).AddTicks(2767) });

            migrationBuilder.AddForeignKey(
                name: "FK_Jobs_ExperienceLevel_ExperienceLevelid",
                table: "Jobs",
                column: "ExperienceLevelid",
                principalTable: "ExperienceLevel",
                principalColumn: "ExperienceLevelid");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Jobs_ExperienceLevel_ExperienceLevelid",
                table: "Jobs");

            migrationBuilder.DropColumn(
                name: "Quantity",
                table: "Jobs");

            migrationBuilder.RenameColumn(
                name: "ExperienceLevelid",
                table: "Jobs",
                newName: "ExperienceLevelId");

            migrationBuilder.RenameIndex(
                name: "IX_Jobs_ExperienceLevelid",
                table: "Jobs",
                newName: "IX_Jobs_ExperienceLevelId");

            migrationBuilder.AlterColumn<int>(
                name: "ExperienceLevelId",
                table: "Jobs",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "ExperienceLevel",
                keyColumn: "ExperienceLevelid",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 15, 17, 51, 8, 424, DateTimeKind.Utc).AddTicks(5299), new DateTime(2025, 7, 15, 17, 51, 8, 424, DateTimeKind.Utc).AddTicks(5299) });

            migrationBuilder.UpdateData(
                table: "ExperienceLevel",
                keyColumn: "ExperienceLevelid",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 15, 17, 51, 8, 424, DateTimeKind.Utc).AddTicks(5303), new DateTime(2025, 7, 15, 17, 51, 8, 424, DateTimeKind.Utc).AddTicks(5304) });

            migrationBuilder.UpdateData(
                table: "ExperienceLevel",
                keyColumn: "ExperienceLevelid",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 15, 17, 51, 8, 424, DateTimeKind.Utc).AddTicks(5304), new DateTime(2025, 7, 15, 17, 51, 8, 424, DateTimeKind.Utc).AddTicks(5305) });

            migrationBuilder.UpdateData(
                table: "ExperienceLevel",
                keyColumn: "ExperienceLevelid",
                keyValue: 4,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 15, 17, 51, 8, 424, DateTimeKind.Utc).AddTicks(5305), new DateTime(2025, 7, 15, 17, 51, 8, 424, DateTimeKind.Utc).AddTicks(5306) });

            migrationBuilder.UpdateData(
                table: "Industries",
                keyColumn: "IndustryId",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 16, 0, 51, 8, 424, DateTimeKind.Unspecified).AddTicks(5153), new DateTime(2025, 7, 16, 0, 51, 8, 424, DateTimeKind.Unspecified).AddTicks(5177) });

            migrationBuilder.UpdateData(
                table: "Industries",
                keyColumn: "IndustryId",
                keyValue: 4,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 16, 0, 51, 8, 424, DateTimeKind.Unspecified).AddTicks(5180), new DateTime(2025, 7, 16, 0, 51, 8, 424, DateTimeKind.Unspecified).AddTicks(5181) });

            migrationBuilder.UpdateData(
                table: "Industries",
                keyColumn: "IndustryId",
                keyValue: 5,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 16, 0, 51, 8, 424, DateTimeKind.Unspecified).AddTicks(5183), new DateTime(2025, 7, 16, 0, 51, 8, 424, DateTimeKind.Unspecified).AddTicks(5184) });

            migrationBuilder.UpdateData(
                table: "Industries",
                keyColumn: "IndustryId",
                keyValue: 6,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 16, 0, 51, 8, 424, DateTimeKind.Unspecified).AddTicks(5186), new DateTime(2025, 7, 16, 0, 51, 8, 424, DateTimeKind.Unspecified).AddTicks(5188) });

            migrationBuilder.UpdateData(
                table: "Industries",
                keyColumn: "IndustryId",
                keyValue: 7,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 16, 0, 51, 8, 424, DateTimeKind.Unspecified).AddTicks(5190), new DateTime(2025, 7, 16, 0, 51, 8, 424, DateTimeKind.Unspecified).AddTicks(5191) });

            migrationBuilder.UpdateData(
                table: "Industries",
                keyColumn: "IndustryId",
                keyValue: 8,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 16, 0, 51, 8, 424, DateTimeKind.Unspecified).AddTicks(5193), new DateTime(2025, 7, 16, 0, 51, 8, 424, DateTimeKind.Unspecified).AddTicks(5194) });

            migrationBuilder.UpdateData(
                table: "Industries",
                keyColumn: "IndustryId",
                keyValue: 9,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 16, 0, 51, 8, 424, DateTimeKind.Unspecified).AddTicks(5196), new DateTime(2025, 7, 16, 0, 51, 8, 424, DateTimeKind.Unspecified).AddTicks(5197) });

            migrationBuilder.UpdateData(
                table: "JobTypes",
                keyColumn: "JobTypeId",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 16, 0, 51, 8, 424, DateTimeKind.Unspecified).AddTicks(5232), new DateTime(2025, 7, 16, 0, 51, 8, 424, DateTimeKind.Unspecified).AddTicks(5234) });

            migrationBuilder.UpdateData(
                table: "JobTypes",
                keyColumn: "JobTypeId",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 16, 0, 51, 8, 424, DateTimeKind.Unspecified).AddTicks(5237), new DateTime(2025, 7, 16, 0, 51, 8, 424, DateTimeKind.Unspecified).AddTicks(5238) });

            migrationBuilder.UpdateData(
                table: "JobTypes",
                keyColumn: "JobTypeId",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 16, 0, 51, 8, 424, DateTimeKind.Unspecified).AddTicks(5241), new DateTime(2025, 7, 16, 0, 51, 8, 424, DateTimeKind.Unspecified).AddTicks(5242) });

            migrationBuilder.UpdateData(
                table: "Levels",
                keyColumn: "LevelId",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 16, 0, 51, 8, 424, DateTimeKind.Unspecified).AddTicks(5266), new DateTime(2025, 7, 16, 0, 51, 8, 424, DateTimeKind.Unspecified).AddTicks(5267) });

            migrationBuilder.UpdateData(
                table: "Levels",
                keyColumn: "LevelId",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 16, 0, 51, 8, 424, DateTimeKind.Unspecified).AddTicks(5270), new DateTime(2025, 7, 16, 0, 51, 8, 424, DateTimeKind.Unspecified).AddTicks(5271) });

            migrationBuilder.UpdateData(
                table: "Levels",
                keyColumn: "LevelId",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 16, 0, 51, 8, 424, DateTimeKind.Unspecified).AddTicks(5273), new DateTime(2025, 7, 16, 0, 51, 8, 424, DateTimeKind.Unspecified).AddTicks(5275) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 15, 17, 51, 8, 424, DateTimeKind.Utc).AddTicks(4960), new DateTime(2025, 7, 15, 17, 51, 8, 424, DateTimeKind.Utc).AddTicks(4964) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 15, 17, 51, 8, 424, DateTimeKind.Utc).AddTicks(4968), new DateTime(2025, 7, 15, 17, 51, 8, 424, DateTimeKind.Utc).AddTicks(4969) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 15, 17, 51, 8, 424, DateTimeKind.Utc).AddTicks(4969), new DateTime(2025, 7, 15, 17, 51, 8, 424, DateTimeKind.Utc).AddTicks(4970) });

            migrationBuilder.AddForeignKey(
                name: "FK_Jobs_ExperienceLevel_ExperienceLevelId",
                table: "Jobs",
                column: "ExperienceLevelId",
                principalTable: "ExperienceLevel",
                principalColumn: "ExperienceLevelid");
        }
    }
}
