using System;
using System.Diagnostics;
using MySql.Data.MySqlClient;
using MongoDB.Bson;
using MongoDB.Driver;

class Program
{
    static void Main()
    {
        string mysqlConnection = "server=localhost;userid=root;password=insy;database=geo_tips";
        string mongoConnection = "mongodb://localhost:27017";
        string mongoDatabase = "geo_tips";
        string mongoCollection = "tipp_locations";

        int numIterations = 100;   // Anzahl der Testläufe
        double radiusKm = 10.0;    // Suchradius in Kilometern
        Random rnd = new Random();

        Console.WriteLine("=== MySQL vs. MongoDB Geo-Abfrage Benchmark (Radius) ===\n");
        Console.WriteLine($"Suchradius: {radiusKm} km\n{numIterations} Durchgänge:\n");

        // ---------------------------------------------
        // MySQL TEST
        // ---------------------------------------------
        int totalMySqlHits = 0;
        var swMySql = Stopwatch.StartNew();
        using (var mysql = new MySqlConnection(mysqlConnection))
        {
            mysql.Open();

            for (int i = 0; i < numIterations; i++)
            {
                double lat = 47.0 + rnd.NextDouble() * 2.0;
                double lon = 12.0 + rnd.NextDouble() * 3.0;

                using var cmd = new MySqlCommand(@"
                    SELECT COUNT(*) 
                    FROM tipp_locations
                    WHERE ST_Distance_Sphere(
                               Coordinates,
                               ST_GeomFromText(CONCAT('POINT(', @lon, ' ', @lat, ')'), 4326)
                          ) <= @radius * 1000;", mysql);

                cmd.Parameters.AddWithValue("@lat", lat);
                cmd.Parameters.AddWithValue("@lon", lon);
                cmd.Parameters.AddWithValue("@radius", radiusKm);

                totalMySqlHits += Convert.ToInt32(cmd.ExecuteScalar());
            }
        }
        swMySql.Stop();
        Console.WriteLine($"MySQL: {numIterations} Abfragen in {swMySql.Elapsed.TotalSeconds:F2} s");
        Console.WriteLine($"Gesamtzahl gefundener Orte: {totalMySqlHits}\n");

        // ---------------------------------------------
        // MongoDB TEST
        // ---------------------------------------------
        int totalMongoHits = 0;
        var swMongo = Stopwatch.StartNew();
        var client = new MongoClient(mongoConnection);
        var db = client.GetDatabase(mongoDatabase);
        var collection = db.GetCollection<BsonDocument>(mongoCollection);

        for (int i = 0; i < numIterations; i++)
        {
            double lat = 47.0 + rnd.NextDouble() * 2.0;
            double lon = 12.0 + rnd.NextDouble() * 3.0;

            var point = new BsonDocument
            {
                { "type", "Point" },
                { "coordinates", new BsonArray { lon, lat } }
            };

            var pipeline = new[]
            {
                new BsonDocument("$geoNear", new BsonDocument
                {
                    { "near", point },
                    { "distanceField", "distance_km" },
                    { "spherical", true },
                    { "distanceMultiplier", 0.001 },
                    { "maxDistance", radiusKm * 1000 }, 
                    { "key", "Coordinates" }
                }),
                new BsonDocument("$count", "count") // zählt die Treffer
            };

            var results = collection.Aggregate<BsonDocument>(pipeline).FirstOrDefault();
            if (results != null)
                totalMongoHits += results["count"].AsInt32;
        }

        swMongo.Stop();
        Console.WriteLine($"MongoDB: {numIterations} Abfragen in {swMongo.Elapsed.TotalSeconds:F2} s");
        Console.WriteLine($"Gesamtzahl gefundener Orte: {totalMongoHits}\n");

        // ---------------------------------------------
        // Vergleich
        // ---------------------------------------------
        Console.WriteLine("=== Ergebnis ===");
        if (swMySql.Elapsed < swMongo.Elapsed)
            Console.WriteLine($"MySQL war schneller ({swMySql.Elapsed.TotalSeconds:F2}s vs. {swMongo.Elapsed.TotalSeconds:F2}s)");
        else
            Console.WriteLine($"MongoDB war schneller ({swMongo.Elapsed.TotalSeconds:F2}s vs. {swMySql.Elapsed.TotalSeconds:F2}s)");
    }
}
