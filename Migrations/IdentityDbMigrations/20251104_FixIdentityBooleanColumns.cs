#nullable disable
using Microsoft.EntityFrameworkCore.Infrastructure;   // <-- DbContextAttribute lives here
using Microsoft.EntityFrameworkCore.Migrations;
using OluRankings.Identity;

namespace OluRankings.Migrations.IdentityDbMigrations
{
    [DbContext(typeof(IdentityDb))]   // now resolves correctly
    public partial class FixIdentityBooleanColumns : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
DO $$
BEGIN
    -- EmailConfirmed
    IF EXISTS (
        SELECT 1 FROM information_schema.columns
        WHERE table_schema = 'public'
          AND table_name   = 'AspNetUsers'
          AND column_name  = 'EmailConfirmed'
          AND data_type    <> 'boolean'
    ) THEN
        EXECUTE 'ALTER TABLE ""AspNetUsers""
                 ALTER COLUMN ""EmailConfirmed""
                 TYPE boolean USING (""EmailConfirmed"" <> 0)';
    END IF;

    -- PhoneNumberConfirmed
    IF EXISTS (
        SELECT 1 FROM information_schema.columns
        WHERE table_schema = 'public'
          AND table_name   = 'AspNetUsers'
          AND column_name  = 'PhoneNumberConfirmed'
          AND data_type    <> 'boolean'
    ) THEN
        EXECUTE 'ALTER TABLE ""AspNetUsers""
                 ALTER COLUMN ""PhoneNumberConfirmed""
                 TYPE boolean USING (""PhoneNumberConfirmed"" <> 0)';
    END IF;

    -- TwoFactorEnabled
    IF EXISTS (
        SELECT 1 FROM information_schema.columns
        WHERE table_schema = 'public'
          AND table_name   = 'AspNetUsers'
          AND column_name  = 'TwoFactorEnabled'
          AND data_type    <> 'boolean'
    ) THEN
        EXECUTE 'ALTER TABLE ""AspNetUsers""
                 ALTER COLUMN ""TwoFactorEnabled""
                 TYPE boolean USING (""TwoFactorEnabled"" <> 0)';
    END IF;

    -- LockoutEnabled
    IF EXISTS (
        SELECT 1 FROM information_schema.columns
        WHERE table_schema = 'public'
          AND table_name   = 'AspNetUsers'
          AND column_name  = 'LockoutEnabled'
          AND data_type    <> 'boolean'
    ) THEN
        EXECUTE 'ALTER TABLE ""AspNetUsers""
                 ALTER COLUMN ""LockoutEnabled""
                 TYPE boolean USING (""LockoutEnabled"" <> 0)';
    END IF;

    -- Defaults (idempotent)
    EXECUTE 'ALTER TABLE ""AspNetUsers""
             ALTER COLUMN ""EmailConfirmed""       SET DEFAULT false,
             ALTER COLUMN ""PhoneNumberConfirmed"" SET DEFAULT false,
             ALTER COLUMN ""TwoFactorEnabled""     SET DEFAULT false,
             ALTER COLUMN ""LockoutEnabled""       SET DEFAULT false';
END
$$;
");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
ALTER TABLE ""AspNetUsers""
  ALTER COLUMN ""EmailConfirmed""       TYPE integer USING (CASE WHEN ""EmailConfirmed"" THEN 1 ELSE 0 END),
  ALTER COLUMN ""PhoneNumberConfirmed"" TYPE integer USING (CASE WHEN ""PhoneNumberConfirmed"" THEN 1 ELSE 0 END),
  ALTER COLUMN ""TwoFactorEnabled""     TYPE integer USING (CASE WHEN ""TwoFactorEnabled"" THEN 1 ELSE 0 END),
  ALTER COLUMN ""LockoutEnabled""       TYPE integer USING (CASE WHEN ""LockoutEnabled"" THEN 1 ELSE 0 END);
");
        }
    }
}
