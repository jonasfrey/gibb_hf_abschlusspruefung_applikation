using MySqlConnector;

namespace UBahnApi.Data;

/// <summary>
/// Stellt vor dem ersten EF-Zugriff sicher, dass Datenbank und Applikations-User
/// existieren. Verbindet sich dafuer einmalig als root. Auf der Pruefungs-VM, wo
/// "admin"/"UBahn" bereits eingerichtet sind, scheitert das still und wird
/// uebersprungen – der normale Migrate-Schritt uebernimmt dann.
/// </summary>
public static class DbBootstrapper
{
    public static void EnsureDatabaseAndUser(IConfiguration config)
    {
        var server   = config["Bootstrap:Server"]       ?? "localhost";
        var port     = config["Bootstrap:Port"]         ?? "3306";
        var rootUser = config["Bootstrap:RootUser"]     ?? "root";
        var rootPass = config["Bootstrap:RootPassword"] ?? "";
        var dbName   = config["Bootstrap:Database"]     ?? "UBahn";
        var appUser  = config["Bootstrap:AppUser"]      ?? "admin";
        var appPass  = config["Bootstrap:AppPassword"]  ?? "sml12345";

        var rootConn = $"Server={server};Port={port};User={rootUser};Password={rootPass};";

        try
        {
            using var conn = new MySqlConnection(rootConn);
            conn.Open();

            Execute(conn, $"CREATE DATABASE IF NOT EXISTS `{dbName}` " +
                          "CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;");

            // User fuer 'localhost' und '%' anlegen (lokal bzw. via Container/Netzwerk)
            foreach (var host in new[] { "localhost", "%" })
            {
                Execute(conn, $"CREATE USER IF NOT EXISTS '{appUser}'@'{host}' IDENTIFIED BY '{appPass}';");
                Execute(conn, $"GRANT ALL PRIVILEGES ON `{dbName}`.* TO '{appUser}'@'{host}';");
            }

            Execute(conn, "FLUSH PRIVILEGES;");

            Console.WriteLine($"[Bootstrap] Datenbank '{dbName}' und User '{appUser}' sind bereit.");
        }
        catch (Exception ex)
        {
            // Kein root-Zugang noetig, wenn admin/DB schon existieren (z.B. Pruefungs-VM).
            Console.WriteLine($"[Bootstrap] Uebersprungen ({ex.Message}). " +
                              "Fahre mit bestehendem User/DB fort.");
        }
    }

    private static void Execute(MySqlConnection conn, string sql)
    {
        using var cmd = new MySqlCommand(sql, conn);
        cmd.ExecuteNonQuery();
    }
}
