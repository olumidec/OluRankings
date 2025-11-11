using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OluRankings.Migrations
{
    /// <inheritdoc />
    public partial class AddAthleteIsPublic : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsPublic",
                table: "Athletes",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsPublic",
                table: "Athletes");
        }
    }
}
