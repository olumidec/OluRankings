using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OluRankings.Migrations
{
    /// <inheritdoc />
    public partial class Add_Ranking_Publisher : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Publisher",
                table: "Rankings",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Publisher",
                table: "Rankings");
        }
    }
}
