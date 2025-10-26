using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OluRankings.Migrations
{
    /// <inheritdoc />
    public partial class Update_App_20251006 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_RankingEntries_RankingId_Rank",
                table: "RankingEntries");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Rankings",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAt",
                table: "Athletes",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Athletes",
                type: "TEXT",
                maxLength: 450,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "VerifiedAt",
                table: "Athletes",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VerifiedByUserId",
                table: "Athletes",
                type: "TEXT",
                maxLength: 450,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ApplicationUser",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    DisplayName = table.Column<string>(type: "TEXT", nullable: true),
                    TeamId = table.Column<string>(type: "TEXT", nullable: true),
                    UserEntityId = table.Column<string>(type: "TEXT", nullable: true),
                    UserName = table.Column<string>(type: "TEXT", nullable: true),
                    NormalizedUserName = table.Column<string>(type: "TEXT", nullable: true),
                    Email = table.Column<string>(type: "TEXT", nullable: true),
                    NormalizedEmail = table.Column<string>(type: "TEXT", nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "INTEGER", nullable: false),
                    PasswordHash = table.Column<string>(type: "TEXT", nullable: true),
                    SecurityStamp = table.Column<string>(type: "TEXT", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "TEXT", nullable: true),
                    PhoneNumber = table.Column<string>(type: "TEXT", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "INTEGER", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "INTEGER", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "TEXT", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "INTEGER", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicationUser", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ApplicationUser_ApplicationUser_UserEntityId",
                        column: x => x.UserEntityId,
                        principalTable: "ApplicationUser",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Rankings_Publisher_Sport_AgeGroup_Position_Region_Period",
                table: "Rankings",
                columns: new[] { "Publisher", "Sport", "AgeGroup", "Position", "Region", "Period" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AthleteSubmissions_Status",
                table: "AthleteSubmissions",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Athletes_FamilyName_GivenName",
                table: "Athletes",
                columns: new[] { "FamilyName", "GivenName" });

            migrationBuilder.CreateIndex(
                name: "IX_Athletes_UserId",
                table: "Athletes",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationUser_UserEntityId",
                table: "ApplicationUser",
                column: "UserEntityId");

            migrationBuilder.AddForeignKey(
                name: "FK_Athletes_ApplicationUser_UserId",
                table: "Athletes",
                column: "UserId",
                principalTable: "ApplicationUser",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Athletes_ApplicationUser_UserId",
                table: "Athletes");

            migrationBuilder.DropTable(
                name: "ApplicationUser");

            migrationBuilder.DropIndex(
                name: "IX_Rankings_Publisher_Sport_AgeGroup_Position_Region_Period",
                table: "Rankings");

            migrationBuilder.DropIndex(
                name: "IX_AthleteSubmissions_Status",
                table: "AthleteSubmissions");

            migrationBuilder.DropIndex(
                name: "IX_Athletes_FamilyName_GivenName",
                table: "Athletes");

            migrationBuilder.DropIndex(
                name: "IX_Athletes_UserId",
                table: "Athletes");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Rankings");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Athletes");

            migrationBuilder.DropColumn(
                name: "VerifiedAt",
                table: "Athletes");

            migrationBuilder.DropColumn(
                name: "VerifiedByUserId",
                table: "Athletes");

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAt",
                table: "Athletes",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "TEXT");

            migrationBuilder.CreateIndex(
                name: "IX_RankingEntries_RankingId_Rank",
                table: "RankingEntries",
                columns: new[] { "RankingId", "Rank" },
                unique: true);
        }
    }
}
