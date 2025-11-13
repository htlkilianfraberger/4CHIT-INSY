using System;
using System.Data;
using System.Globalization;
using System.IO;
using MySql.Data.MySqlClient;

class Program
{
    static void Main(string[] args)
    {
        string csvPath = "tipps.csv";
        string connectionString = "server=localhost;userid=root;password=insy;database=geo_tips";

        using var connection = new MySqlConnection(connectionString);
        connection.Open();

        using var command = new MySqlCommand(@"
            INSERT INTO tipp_locations (TippID, UrlKurzText, Latitude, Longitude)
            VALUES (@TippID, @UrlKurzText, @Latitude, @Longitude)
            ON DUPLICATE KEY UPDATE
                UrlKurzText = VALUES(UrlKurzText),
                Latitude = VALUES(Latitude),
                Longitude = VALUES(Longitude);", connection);

        command.Parameters.Add("@TippID", MySqlDbType.Int32);
        command.Parameters.Add("@UrlKurzText", MySqlDbType.VarChar);
        command.Parameters.Add("@Latitude", MySqlDbType.Decimal);
        command.Parameters.Add("@Longitude", MySqlDbType.Decimal);

        int lineCount = 0;

        foreach (var line in File.ReadLines(csvPath))
        {
            if (lineCount++ == 0) continue;
            
            var parts = line.Split(';');
            if (parts.Length < 4)
                continue;

            try
            {
                int tippId = int.Parse(parts[0]);
                string urlKurzText = parts[1].Trim();
                
                string latRaw = parts[2].Replace(" ", "").Trim();
                string lonRaw = parts[3].Replace(" ", "").Trim();
                
                string latStr = latRaw.Length > 6 ? latRaw.Insert(latRaw.Length - 6, ".") : latRaw;
                string lonStr = lonRaw.Length > 6 ? lonRaw.Insert(lonRaw.Length - 6, ".") : lonRaw;
                
                decimal lat = decimal.Parse(latStr, CultureInfo.InvariantCulture);
                decimal lon = decimal.Parse(lonStr, CultureInfo.InvariantCulture);
                
                command.Parameters["@TippID"].Value = tippId;
                command.Parameters["@UrlKurzText"].Value = urlKurzText;
                command.Parameters["@Latitude"].Value = lat;
                command.Parameters["@Longitude"].Value = lon;

                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Fehler in Zeile {lineCount}: {ex.Message}");
            }
        }

        Console.WriteLine("Import abgeschlossen!");
    }
}
