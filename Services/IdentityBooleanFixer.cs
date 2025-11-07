using System.Data;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace OluRankings.Services
{
    /// <summary>
    /// One-time safety fixer. If Identity boolean columns were created as ints (0/1) during
    /// the SQLite phase, convert them to TRUE/FALSE booleans in Postgres.
    /// Safe to run repeatedly; it only changes columns that are not boolean.
    /// </summary>
    public static class IdentityBooleanFixer
    {
        private const string CheckSql = @"
SELECT column_name, data_type
FROM information_schema.columns
WHERE table_schema = 'public'
  AND table_name   = 'AspNetUsers'
  AND column_name IN ('EmailConfirmed','PhoneNumberConfirmed','TwoFactorEnabled','LockoutEnabled');
";

        private const string FixSql = @"
ALTER TABLE ""AspNetUsers""
  ALTER COLUMN ""EmailConfirmed""       TYPE boolean USING (""EmailConfirmed""<>0),
  ALTER COLUMN ""PhoneNumberConfirmed"" TYPE boolean USING (""PhoneNumberConfirmed""<>0),
  ALTER COLUMN ""TwoFactorEnabled""     TYPE boolean USING (""TwoFactorEnabled""<>0),
  ALTER COLUMN ""LockoutEnabled""       TYPE boolean USING (""LockoutEnabled""<>0);

ALTER TABLE ""AspNetUsers""
  ALTER COLUMN ""EmailConfirmed""       SET DEFAULT false,
  ALTER COLUMN ""PhoneNumberConfirmed"" SET DEFAULT false,
  ALTER COLUMN ""TwoFactorEnabled""     SET DEFAULT false,
  ALTER COLUMN ""LockoutEnabled""       SET DEFAULT false;
";

        public static async Task RunAsync(DbContext db, CancellationToken ct = default)
        {
            // Only for Postgres
            if (db.Database.ProviderName?.Contains("Npgsql") != true) return;

            await using var conn = (NpgsqlConnection)db.Database.GetDbConnection();
            var mustClose = conn.State != ConnectionState.Open;
            if (mustClose) await conn.OpenAsync(ct);

            try
            {
                // read current types
                var cmd = new NpgsqlCommand(CheckSql, conn);
                var map = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                await using (var reader = await cmd.ExecuteReaderAsync(ct))
                {
                    while (await reader.ReadAsync(ct))
                    {
                        var col = reader.GetString(0);
                        var typ = reader.GetString(1);
                        map[col] = typ;
                    }
                }

                // if any target column is not boolean -> run fix
                bool needsFix =
                    map.TryGetValue("EmailConfirmed", out var t1) && t1 != "boolean" ||
                    map.TryGetValue("PhoneNumberConfirmed", out var t2) && t2 != "boolean" ||
                    map.TryGetValue("TwoFactorEnabled", out var t3) && t3 != "boolean" ||
                    map.TryGetValue("LockoutEnabled", out var t4) && t4 != "boolean";

                if (needsFix)
                {
                    var alter = new NpgsqlCommand(FixSql, conn);
                    await alter.ExecuteNonQueryAsync(ct);
                    Console.WriteLine("✅ Identity boolean columns converted to real booleans.");
                }
                else
                {
                    Console.WriteLine("ℹ️ Identity boolean columns already correct.");
                }
            }
            finally
            {
                if (mustClose) await conn.CloseAsync();
            }
        }
    }
}
