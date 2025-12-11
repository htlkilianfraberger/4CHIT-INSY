using System;
using System.Collections.Generic;
using System.Globalization;
using MySql.Data.MySqlClient;
using MongoDB.Bson;
using MongoDB.Driver;

class Program
{
    static void Main()
    {
        string mysqlConnectionString = "server=localhost;userid=root;password=insy;database=geo_tips";
        string mongoConnectionString = "mongodb://localhost:27017";
        string mongoDatabaseName = "geo_tips";
        string mongoCollectionName = "tipp_locations";

        var random = new Random();

        // --- MySQL Setup ---
        using var mysqlConnection = new MySqlConnection(mysqlConnectionString);
        mysqlConnection.Open();

        // --- MongoDB Setup ---
        var mongoClient = new MongoClient(mongoConnectionString);
        var mongoDatabase = mongoClient.GetDatabase(mongoDatabaseName);
        var mongoCollection = mongoDatabase.GetCollection<BsonDocument>(mongoCollectionName);

        Console.WriteLine("Alte Daten werden gelöscht...");

        // --- Alte Daten löschen ---
        using (var deleteCmd = new MySqlCommand("TRUNCATE TABLE tipp_locations;", mysqlConnection))
        {
            deleteCmd.ExecuteNonQuery();
        }
        mongoCollection.DeleteMany(Builders<BsonDocument>.Filter.Empty);

        Console.WriteLine("Alte Daten gelöscht. Generiere neue Datensätze...");

        const int totalEntries = 100_000;
        const int batchSize = 10_000;

        var mysqlValues = new List<string>(batchSize);
        var mongoDocs = new List<BsonDocument>(batchSize);

        for (int i = 0; i < totalEntries; i++)
        {
            int tippId = i + 1; // Eindeutige ID für MySQL und MongoDB
            string urlKurzText = $"Tip_{tippId}";

            // Weltweite Koordinaten
            double lat = random.NextDouble() * 180.0 - 90.0;     // -90 bis 90
            double lon = random.NextDouble() * 180.0 - 90.0;

            // --- MySQL Batch ---
            mysqlValues.Add($"({tippId}, '{MySqlHelper.EscapeString(urlKurzText)}', {lat.ToString(CultureInfo.InvariantCulture)}, {lon.ToString(CultureInfo.InvariantCulture)}, ST_GeomFromText('POINT({lon.ToString(CultureInfo.InvariantCulture)} {lat.ToString(CultureInfo.InvariantCulture)})', 4326))");

            // --- MongoDB Batch ---
            var doc = new BsonDocument
            {
                { "TippID", tippId },
                { "UrlKurzText", urlKurzText },
                { "Latitude", lat },
                { "Longitude", lon },
                { "Coordinates", new BsonDocument { { "type", "Point" }, { "coordinates", new BsonArray { lon, lat } } } },
                { "CoordinatesText", $"{lat}, {lon}" }
            };
            mongoDocs.Add(doc);

            // Wenn Batch voll, einfügen
            if ((i + 1) % batchSize == 0 || (i + 1) == totalEntries)
            {
                // --- MySQL Insert ---
                string sql = @"
                    INSERT INTO tipp_locations (TippID, UrlKurzText, Latitude, Longitude, Coordinates)
                    VALUES " + string.Join(",", mysqlValues) + ";";

                using var cmd = new MySqlCommand(sql, mysqlConnection);
                cmd.ExecuteNonQuery();
                mysqlValues.Clear();

                // --- MongoDB Insert ---
                mongoCollection.InsertMany(mongoDocs);
                mongoDocs.Clear();

                Console.WriteLine($"{i + 1} Datensätze eingefügt...");
            }
        }

        Console.WriteLine("Bulk-Import in MySQL und MongoDB abgeschlossen!");
    }
}
