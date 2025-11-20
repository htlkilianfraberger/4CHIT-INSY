using System;
using System.Diagnostics;
using System.Collections.Generic;
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

        // ---------------------------------------------
        // Zufallswerte vorberechnen
        // ---------------------------------------------
        (double lat, double lon, double latMinus, double latPlus, double lonMinus, double lonPlus)[] coordinates =
            new (double, double, double, double, double, double)[numIterations];

        for (int i = 0; i < numIterations; i++)
        {
            double lat = 47.0 + rnd.NextDouble() * 2.0;
            double lon = 13.0 + rnd.NextDouble() * 3.0;

            double dy = radiusKm / 111.0;
            double dx = radiusKm / (111.0 * Math.Cos(lat * Math.PI / 180.0));

            coordinates[i] = (lat, lon, lat - dy, lat + dy, lon - dx, lon + dx);
        }

        // ---------------------------------------------
        // MySQL TEST — ohne MBR
        // ---------------------------------------------
        int totalMySqlNoMbrHits = 0;
        var swMySqlNoMbr = Stopwatch.StartNew();
        using (var mysql2 = new MySqlConnection(mysqlConnection))
        {
            mysql2.Open();

            foreach (var coord in coordinates)
            {
                using var cmd = new MySqlCommand(@"
                    SELECT TippID, ST_AsText(Coordinates)
                    FROM tipp_locations
                    WHERE ST_Distance_Sphere(
                        Coordinates,
                        ST_GeomFromText(CONCAT('POINT(', @lon, ' ', @lat, ')'), 4326)
                    ) <= @radius * 1000;
                ", mysql2);

                cmd.Parameters.AddWithValue("@lat", coord.lat);
                cmd.Parameters.AddWithValue("@lon", coord.lon);
                cmd.Parameters.AddWithValue("@radius", radiusKm);

                using var reader = cmd.ExecuteReader();
                int localCount = 0;
                while (reader.Read())
                    localCount++;

                totalMySqlNoMbrHits += localCount;
            }
        }
        swMySqlNoMbr.Stop();
        Console.WriteLine($"MySQL ohne MBR: {numIterations} Abfragen in {swMySqlNoMbr.Elapsed.TotalSeconds:F2} s");
        Console.WriteLine($"Gesamtzahl gefundener Orte: {totalMySqlNoMbrHits}\n");

        // ---------------------------------------------
        // MySQL TEST — mit MBR
        // ---------------------------------------------
        int totalMySqlHits = 0;
        var swMySql = Stopwatch.StartNew();
        using (var mysql = new MySqlConnection(mysqlConnection))
        {
            mysql.Open();

            foreach (var coord in coordinates)
            {
                using var cmd = new MySqlCommand(@"
                    SELECT TippID, ST_AsText(Coordinates)
                    FROM tipp_locations 
                    WHERE MBRContains(
                        ST_GeomFromText(
                            CONCAT('POLYGON((',
                                   @lonMinus, ' ', @latMinus, ', ',
                                   @lonPlus, ' ', @latMinus, ', ',
                                   @lonPlus, ' ', @latPlus, ', ',
                                   @lonMinus, ' ', @latPlus, ', ',
                                   @lonMinus, ' ', @latMinus, '))'), 4326
                        ),
                        Coordinates
                    )
                    AND ST_Distance_Sphere(
                        Coordinates,
                        ST_GeomFromText(CONCAT('POINT(', @lon, ' ', @lat, ')'), 4326)
                    ) <= @radius * 1000;
                ", mysql);

                cmd.Parameters.AddWithValue("@lat", coord.lat);
                cmd.Parameters.AddWithValue("@lon", coord.lon);
                cmd.Parameters.AddWithValue("@radius", radiusKm);
                cmd.Parameters.AddWithValue("@latMinus", coord.latMinus);
                cmd.Parameters.AddWithValue("@latPlus", coord.latPlus);
                cmd.Parameters.AddWithValue("@lonMinus", coord.lonMinus);
                cmd.Parameters.AddWithValue("@lonPlus", coord.lonPlus);

                using var reader = cmd.ExecuteReader();
                int localCount = 0;
                while (reader.Read())
                    localCount++;

                totalMySqlHits += localCount;
            }
        }
        swMySql.Stop();
        Console.WriteLine($"MySQL mit MBR: {numIterations} Abfragen in {swMySql.Elapsed.TotalSeconds:F2} s");
        Console.WriteLine($"Gesamtzahl gefundener Orte: {totalMySqlHits}\n");

// ---------------------------------------------
// MongoDB TEST
// ---------------------------------------------
        int totalMongoHits = 0;
        var swMongo = Stopwatch.StartNew();
        var client = new MongoClient(mongoConnection);
        var db = client.GetDatabase(mongoDatabase);
        var collection = db.GetCollection<BsonDocument>(mongoCollection);

        foreach (var coord in coordinates)
        {
            var point = new BsonDocument
            {
                { "type", "Point" },
                { "coordinates", new BsonArray { coord.lon, coord.lat } }
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
                })
            };
            
            var results = collection
                .Aggregate<BsonDocument>(pipeline)
                .ToList();

            totalMongoHits += results.Count;
        }

        swMongo.Stop();

        Console.WriteLine($"MongoDB: {numIterations} Abfragen in {swMongo.Elapsed.TotalSeconds:F2} s");
        Console.WriteLine($"Gesamtzahl gefundener Orte: {totalMongoHits}\n");

        // ---------------------------------------------
        // Vergleich
        // ---------------------------------------------
        Console.WriteLine("=== Ergebnis ===");
        if (swMySql.Elapsed < swMongo.Elapsed && swMySql.Elapsed < swMySqlNoMbr.Elapsed)
            Console.WriteLine($"MySQL mit MBR war am schnellsten ({swMySql.Elapsed.TotalSeconds:F2}s)");
        else if (swMySqlNoMbr.Elapsed < swMongo.Elapsed)
            Console.WriteLine($"MySQL ohne MBR war am schnellsten ({swMySqlNoMbr.Elapsed.TotalSeconds:F2}s)");
        else
            Console.WriteLine($"MongoDB war am schnellsten ({swMongo.Elapsed.TotalSeconds:F2}s)");
    }
}
