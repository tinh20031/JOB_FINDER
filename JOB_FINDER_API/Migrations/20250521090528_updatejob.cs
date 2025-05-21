using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JOB_FINDER_API.Migrations
{
    /// <inheritdoc />
    public partial class updatejob : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Jobs_Experiences_ExperienceId",
                table: "Jobs");

            migrationBuilder.RenameColumn(
                name: "ExperienceId",
                table: "Jobs",
                newName: "ExperienceLevelId");

            migrationBuilder.RenameIndex(
                name: "IX_Jobs_ExperienceId",
                table: "Jobs",
                newName: "IX_Jobs_ExperienceLevelId");

            migrationBuilder.CreateTable(
                name: "ExperienceLevel",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExperienceLevel", x => x.id);
                });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 5, 21, 9, 5, 28, 394, DateTimeKind.Utc).AddTicks(7611), new DateTime(2025, 5, 21, 9, 5, 28, 394, DateTimeKind.Utc).AddTicks(7612) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 5, 21, 9, 5, 28, 394, DateTimeKind.Utc).AddTicks(7616), new DateTime(2025, 5, 21, 9, 5, 28, 394, DateTimeKind.Utc).AddTicks(7617) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 5, 21, 9, 5, 28, 394, DateTimeKind.Utc).AddTicks(7641), new DateTime(2025, 5, 21, 9, 5, 28, 394, DateTimeKind.Utc).AddTicks(7642) });

            migrationBuilder.AddForeignKey(
                name: "FK_Jobs_ExperienceLevel_ExperienceLevelId",
                table: "Jobs",
                column: "ExperienceLevelId",
                principalTable: "ExperienceLevel",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Jobs_ExperienceLevel_ExperienceLevelId",
                table: "Jobs");

            migrationBuilder.DropTable(
                name: "ExperienceLevel");

            migrationBuilder.RenameColumn(
                name: "ExperienceLevelId",
                table: "Jobs",
                newName: "ExperienceId");

            migrationBuilder.RenameIndex(
                name: "IX_Jobs_ExperienceLevelId",
                table: "Jobs",
                newName: "IX_Jobs_ExperienceId");

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 5, 21, 8, 41, 50, 891, DateTimeKind.Utc).AddTicks(7698), new DateTime(2025, 5, 21, 8, 41, 50, 891, DateTimeKind.Utc).AddTicks(7700) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 5, 21, 8, 41, 50, 891, DateTimeKind.Utc).AddTicks(7704), new DateTime(2025, 5, 21, 8, 41, 50, 891, DateTimeKind.Utc).AddTicks(7704) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 5, 21, 8, 41, 50, 891, DateTimeKind.Utc).AddTicks(7705), new DateTime(2025, 5, 21, 8, 41, 50, 891, DateTimeKind.Utc).AddTicks(7706) });

            migrationBuilder.AddForeignKey(
                name: "FK_Jobs_Experiences_ExperienceId",
                table: "Jobs",
                column: "ExperienceId",
                principalTable: "Experiences",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
