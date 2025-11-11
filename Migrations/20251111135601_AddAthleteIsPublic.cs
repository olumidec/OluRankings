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
            if (migrationBuilder.ActiveProvider == "Npgsql.EntityFrameworkCore.PostgreSQL")
            {
                // Postgres: add only if missing (safe re-run on Render)
                migrationBuilder.Sql("""
            ALTER TABLE "Athletes"
            ADD COLUMN IF NOT EXISTS "IsPublic" boolean NOT NULL DEFAULT false;
        """);
            }
            else
            {
                // SQLite (dev): normal add
                migrationBuilder.AddColumn<bool>(
                    name: "IsPublic",
                    table: "Athletes",
                    type: "INTEGER",
                    nullable: false,
                    defaultValue: false);
            }
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            if (migrationBuilder.ActiveProvider == "Npgsql.EntityFrameworkCore.PostgreSQL")
            {
                migrationBuilder.Sql("""
            ALTER TABLE "Athletes"
            DROP COLUMN IF EXISTS "IsPublic";
        """);
            }
            else
            {
                migrationBuilder.DropColumn(
                    name: "IsPublic",
                    table: "Athletes");
            }
        }
    }
}

