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

        int numIterations = 100;
        Random rnd = new Random();

        Console.WriteLine("=== MySQL vs. MongoDB Geo-Abfrage Benchmark ===\n");
        
        var swMySql = Stopwatch.StartNew();
        using (var mysql = new MySqlConnection(mysqlConnection))
        {
            mysql.Open();

            for (int i = 0; i < numIterations; i++)
            {
                double lat = 47.0 + rnd.NextDouble() * 2.0;
                double lon = 12.0 + rnd.NextDouble() * 3.0;

                using var cmd = new MySqlCommand(@"
                    SELECT TippID, UrlKurzText, Latitude, Longitude,
                           ST_Distance_Sphere(
                               Coordinates,
                               ST_GeomFromText(CONCAT('POINT(', @lon, ' ', @lat, ')'), 4326)
                           ) / 1000 AS distance_km
                    FROM tipp_locations
                    ORDER BY distance_km ASC
                    LIMIT 5;", mysql);

                cmd.Parameters.AddWithValue("@lat", lat);
                cmd.Parameters.AddWithValue("@lon", lon);

                using var reader = cmd.ExecuteReader();
                while (reader.Read()) { } // Ergebnisse lesen
                reader.Close();
            }
        }
        swMySql.Stop();
        Console.WriteLine($"MySQL:   {numIterations} Abfragen in {swMySql.Elapsed.TotalSeconds:F2} s");
        
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

            // $geoNear ohne "limit" – stattdessen extra $limit-Stage
            var pipeline = new[]
            {
                new BsonDocument("$geoNear", new BsonDocument
                {
                    { "near", point },
                    { "distanceField", "distance_km" },
                    { "spherical", true },
                    { "distanceMultiplier", 0.001 }
                }),
                new BsonDocument("$limit", 5) // <- separat hinzufügen
            };

            var results = collection.Aggregate<BsonDocument>(pipeline).ToList();
        }

        swMongo.Stop();
        Console.WriteLine($"MongoDB: {numIterations} Abfragen in {swMongo.Elapsed.TotalSeconds:F2} s");


        // -------------------------------
        // Vergleich
        // -------------------------------
        Console.WriteLine("\n=== Ergebnis ===");
        if (swMySql.Elapsed < swMongo.Elapsed)
            Console.WriteLine($"MySQL war schneller ({swMySql.Elapsed.TotalSeconds:F2}s vs. {swMongo.Elapsed.TotalSeconds:F2}s)");
        else
            Console.WriteLine($"MongoDB war schneller ({swMongo.Elapsed.TotalSeconds:F2}s vs. {swMySql.Elapsed.TotalSeconds:F2}s)");
    }
}
