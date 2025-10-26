using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OluRankings.Migrations
{
    /// <inheritdoc />
    public partial class Init_App : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Athletes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    GivenName = table.Column<string>(type: "TEXT", maxLength: 80, nullable: false),
                    FamilyName = table.Column<string>(type: "TEXT", maxLength: 80, nullable: false),
                    Slug = table.Column<string>(type: "TEXT", maxLength: 120, nullable: false),
                    Sport = table.Column<string>(type: "TEXT", maxLength: 40, nullable: false),
                    Position = table.Column<string>(type: "TEXT", maxLength: 40, nullable: true),
                    ClassYear = table.Column<int>(type: "INTEGER", nullable: true),
                    School = table.Column<string>(type: "TEXT", maxLength: 120, nullable: true),
                    Club = table.Column<string>(type: "TEXT", maxLength: 120, nullable: true),
                    Region = table.Column<string>(type: "TEXT", maxLength: 80, nullable: true),
                    HeightCm = table.Column<int>(type: "INTEGER", nullable: true),
                    WeightKg = table.Column<int>(type: "INTEGER", nullable: true),
                    HeadshotUrl = table.Column<string>(type: "TEXT", nullable: true),
                    Status = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Athletes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Rankings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Sport = table.Column<string>(type: "TEXT", nullable: false),
                    AgeGroup = table.Column<string>(type: "TEXT", nullable: false),
                    Position = table.Column<string>(type: "TEXT", nullable: true),
                    Region = table.Column<string>(type: "TEXT", nullable: true),
                    Period = table.Column<string>(type: "TEXT", nullable: false),
                    PublishedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rankings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AthleteSubmissions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    SubmittedByUserId = table.Column<string>(type: "TEXT", nullable: false),
                    GivenName = table.Column<string>(type: "TEXT", maxLength: 80, nullable: false),
                    FamilyName = table.Column<string>(type: "TEXT", maxLength: 80, nullable: false),
                    School = table.Column<string>(type: "TEXT", maxLength: 120, nullable: true),
                    Club = table.Column<string>(type: "TEXT", maxLength: 120, nullable: true),
                    ClassYear = table.Column<int>(type: "INTEGER", nullable: true),
                    Dob = table.Column<DateTime>(type: "TEXT", nullable: true),
                    EvidencePath = table.Column<string>(type: "TEXT", maxLength: 512, nullable: false),
                    EvidenceContentType = table.Column<string>(type: "TEXT", maxLength: 64, nullable: true),
                    EvidenceBytes = table.Column<long>(type: "INTEGER", nullable: false),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    ReviewerNote = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    ReviewedByUserId = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ReviewedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    AthleteId = table.Column<Guid>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AthleteSubmissions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AthleteSubmissions_Athletes_AthleteId",
                        column: x => x.AthleteId,
                        principalTable: "Athletes",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "StatLines",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    AthleteId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Season = table.Column<string>(type: "TEXT", nullable: false),
                    Competition = table.Column<string>(type: "TEXT", nullable: true),
                    GamesPlayed = table.Column<int>(type: "INTEGER", nullable: false),
                    PointsPerGame = table.Column<double>(type: "REAL", nullable: false),
                    AssistsPerGame = table.Column<double>(type: "REAL", nullable: false),
                    ReboundsPerGame = table.Column<double>(type: "REAL", nullable: false),
                    IsVerified = table.Column<bool>(type: "INTEGER", nullable: false),
                    EvidenceUrl = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StatLines", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StatLines_Athletes_AthleteId",
                        column: x => x.AthleteId,
                        principalTable: "Athletes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RankingEntries",
                columns: table => new
                {
                    RankingId = table.Column<Guid>(type: "TEXT", nullable: false),
                    AthleteId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Rank = table.Column<int>(type: "INTEGER", nullable: false),
                    Score = table.Column<double>(type: "REAL", nullable: false),
                    Notes = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RankingEntries", x => new { x.RankingId, x.AthleteId });
                    table.ForeignKey(
                        name: "FK_RankingEntries_Athletes_AthleteId",
                        column: x => x.AthleteId,
                        principalTable: "Athletes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RankingEntries_Rankings_RankingId",
                        column: x => x.RankingId,
                        principalTable: "Rankings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Athletes_Slug",
                table: "Athletes",
                column: "Slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AthleteSubmissions_AthleteId",
                table: "AthleteSubmissions",
                column: "AthleteId");

            migrationBuilder.CreateIndex(
                name: "IX_RankingEntries_AthleteId",
                table: "RankingEntries",
                column: "AthleteId");

            migrationBuilder.CreateIndex(
                name: "IX_RankingEntries_RankingId_Rank",
                table: "RankingEntries",
                columns: new[] { "RankingId", "Rank" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StatLines_AthleteId",
                table: "StatLines",
                column: "AthleteId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AthleteSubmissions");

            migrationBuilder.DropTable(
                name: "RankingEntries");

            migrationBuilder.DropTable(
                name: "StatLines");

            migrationBuilder.DropTable(
                name: "Rankings");

            migrationBuilder.DropTable(
                name: "Athletes");
        }
    }
}
