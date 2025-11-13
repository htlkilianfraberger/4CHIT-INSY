using System;
using System.Globalization;
using System.IO;
using MongoDB.Bson;
using MongoDB.Driver;

class Program
{
    static void Main(string[] args)
    {
        string csvPath = "tipps.csv";
        string connectionString = "mongodb://localhost:27017"; // Verbindung zur MongoDB
        string databaseName = "geo_tips";
        string collectionName = "tipp_locations";

        var client = new MongoClient(connectionString);
        var database = client.GetDatabase(databaseName);
        var collection = database.GetCollection<BsonDocument>(collectionName);

        int lineCount = 0;

        foreach (var line in File.ReadLines(csvPath))
        {
            if (lineCount++ == 0) continue; // Header überspringen

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

                double lat = double.Parse(latStr, CultureInfo.InvariantCulture);
                double lon = double.Parse(lonStr, CultureInfo.InvariantCulture);

                var filter = Builders<BsonDocument>.Filter.Eq("TippID", tippId);

                var update = Builders<BsonDocument>.Update
                    .Set("UrlKurzText", urlKurzText)
                    .Set("Latitude", lat)
                    .Set("Longitude", lon)
                    .Set("Coordinates", new BsonDocument
                    {
                        { "type", "Point" },
                        { "coordinates", new BsonArray { lon, lat } }
                    })
                    .Set("CoordinatesText", $"{lat}, {lon}");

                collection.UpdateOne(filter, update, new UpdateOptions { IsUpsert = true });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Fehler in Zeile {lineCount}: {ex.Message}");
            }
        }

        Console.WriteLine("Import in MongoDB abgeschlossen!");
    }
}
