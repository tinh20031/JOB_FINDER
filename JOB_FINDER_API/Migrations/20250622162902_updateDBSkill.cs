using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JOB_FINDER_API.Migrations
{
    /// <inheritdoc />
    public partial class updateDBSkill : Migration
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

            migrationBuilder.AlterColumn<int>(
                name: "CandidateProfileId",
                table: "Skills",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.UpdateData(
                table: "Industries",
                keyColumn: "IndustryId",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 22, 16, 29, 0, 846, DateTimeKind.Utc).AddTicks(3066), new DateTime(2025, 6, 22, 16, 29, 0, 846, DateTimeKind.Utc).AddTicks(3067) });

            migrationBuilder.UpdateData(
                table: "Industries",
                keyColumn: "IndustryId",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 22, 16, 29, 0, 846, DateTimeKind.Utc).AddTicks(3069), new DateTime(2025, 6, 22, 16, 29, 0, 846, DateTimeKind.Utc).AddTicks(3069) });

            migrationBuilder.UpdateData(
                table: "JobTypes",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 22, 16, 29, 0, 846, DateTimeKind.Utc).AddTicks(3110), new DateTime(2025, 6, 22, 16, 29, 0, 846, DateTimeKind.Utc).AddTicks(3112) });

            migrationBuilder.UpdateData(
                table: "JobTypes",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 22, 16, 29, 0, 846, DateTimeKind.Utc).AddTicks(3113), new DateTime(2025, 6, 22, 16, 29, 0, 846, DateTimeKind.Utc).AddTicks(3114) });

            migrationBuilder.UpdateData(
                table: "JobTypes",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 22, 16, 29, 0, 846, DateTimeKind.Utc).AddTicks(3115), new DateTime(2025, 6, 22, 16, 29, 0, 846, DateTimeKind.Utc).AddTicks(3116) });

            migrationBuilder.UpdateData(
                table: "Levels",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 22, 16, 29, 0, 846, DateTimeKind.Utc).AddTicks(3150), new DateTime(2025, 6, 22, 16, 29, 0, 846, DateTimeKind.Utc).AddTicks(3151) });

            migrationBuilder.UpdateData(
                table: "Levels",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 22, 16, 29, 0, 846, DateTimeKind.Utc).AddTicks(3153), new DateTime(2025, 6, 22, 16, 29, 0, 846, DateTimeKind.Utc).AddTicks(3154) });

            migrationBuilder.UpdateData(
                table: "Levels",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 22, 16, 29, 0, 846, DateTimeKind.Utc).AddTicks(3155), new DateTime(2025, 6, 22, 16, 29, 0, 846, DateTimeKind.Utc).AddTicks(3156) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 22, 16, 29, 0, 846, DateTimeKind.Utc).AddTicks(2919), new DateTime(2025, 6, 22, 16, 29, 0, 846, DateTimeKind.Utc).AddTicks(2921) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 22, 16, 29, 0, 846, DateTimeKind.Utc).AddTicks(2927), new DateTime(2025, 6, 22, 16, 29, 0, 846, DateTimeKind.Utc).AddTicks(2928) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 22, 16, 29, 0, 846, DateTimeKind.Utc).AddTicks(2929), new DateTime(2025, 6, 22, 16, 29, 0, 846, DateTimeKind.Utc).AddTicks(2929) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "CandidateProfileId",
                table: "Skills",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

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
                values: new object[] { new DateTime(2025, 6, 18, 7, 44, 12, 397, DateTimeKind.Utc).AddTicks(7801), new DateTime(2025, 6, 18, 7, 44, 12, 397, DateTimeKind.Utc).AddTicks(7802) });

            migrationBuilder.UpdateData(
                table: "Industries",
                keyColumn: "IndustryId",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 18, 7, 44, 12, 397, DateTimeKind.Utc).AddTicks(7804), new DateTime(2025, 6, 18, 7, 44, 12, 397, DateTimeKind.Utc).AddTicks(7804) });

            migrationBuilder.UpdateData(
                table: "JobTypes",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 18, 7, 44, 12, 397, DateTimeKind.Utc).AddTicks(7838), new DateTime(2025, 6, 18, 7, 44, 12, 397, DateTimeKind.Utc).AddTicks(7838) });

            migrationBuilder.UpdateData(
                table: "JobTypes",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 18, 7, 44, 12, 397, DateTimeKind.Utc).AddTicks(7840), new DateTime(2025, 6, 18, 7, 44, 12, 397, DateTimeKind.Utc).AddTicks(7841) });

            migrationBuilder.UpdateData(
                table: "JobTypes",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 18, 7, 44, 12, 397, DateTimeKind.Utc).AddTicks(7842), new DateTime(2025, 6, 18, 7, 44, 12, 397, DateTimeKind.Utc).AddTicks(7843) });

            migrationBuilder.UpdateData(
                table: "Levels",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 18, 7, 44, 12, 397, DateTimeKind.Utc).AddTicks(7871), new DateTime(2025, 6, 18, 7, 44, 12, 397, DateTimeKind.Utc).AddTicks(7872) });

            migrationBuilder.UpdateData(
                table: "Levels",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 18, 7, 44, 12, 397, DateTimeKind.Utc).AddTicks(7874), new DateTime(2025, 6, 18, 7, 44, 12, 397, DateTimeKind.Utc).AddTicks(7874) });

            migrationBuilder.UpdateData(
                table: "Levels",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 18, 7, 44, 12, 397, DateTimeKind.Utc).AddTicks(7876), new DateTime(2025, 6, 18, 7, 44, 12, 397, DateTimeKind.Utc).AddTicks(7876) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 18, 7, 44, 12, 397, DateTimeKind.Utc).AddTicks(7624), new DateTime(2025, 6, 18, 7, 44, 12, 397, DateTimeKind.Utc).AddTicks(7628) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 18, 7, 44, 12, 397, DateTimeKind.Utc).AddTicks(7633), new DateTime(2025, 6, 18, 7, 44, 12, 397, DateTimeKind.Utc).AddTicks(7633) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 18, 7, 44, 12, 397, DateTimeKind.Utc).AddTicks(7634), new DateTime(2025, 6, 18, 7, 44, 12, 397, DateTimeKind.Utc).AddTicks(7635) });

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
