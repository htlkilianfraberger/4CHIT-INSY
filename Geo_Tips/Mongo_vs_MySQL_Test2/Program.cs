using System;
using System.Diagnostics;
using System.Threading;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using MongoDB.Bson;
using MongoDB.Driver;

class Program
{
    static void Main()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        string mysqlConnection = "server=localhost;userid=root;password=insy;database=geo_tips";
        string mongoConnection = "mongodb://localhost:27017";
        string mongoDatabase = "geo_tips";
        string mongoCollection = "tipp_locations";

        int numIterations = 100;
        Random rnd = new Random();

        // HEADER
        PrintHeader("MySQL vs MongoDB Geo-Benchmark");

        // ------------------------------
        // Testpunkte generieren
        // ------------------------------
        PrintSection("Zufällige Testpunkte werden erstellt ... \n");
        var coordinates = GenerateCoordinates(numIterations, rnd);
        SpinnerAnimation(700, "Testpunkte erstellen");
        Console.WriteLine("\r→ Fertige Testpunkte wurden erstellt!          \n");

        // ------------------------------
        // MySQL ohne MBR
        // ------------------------------
        PrintSection("MySQL (ohne MBR) wird ausgeführt ...");
        var (totalMySqlNoMbrHits, timeMySqlNoMbr) = RunMySqlNoMbr(mysqlConnection, coordinates, "Abfrage läuft");
        PrintResult("MySQL ohne MBR", timeMySqlNoMbr, totalMySqlNoMbrHits);

        // ------------------------------
        // MySQL mit MBR
        // ------------------------------
        PrintSection("MySQL (mit MBR) wird ausgeführt ...");
        var (totalMySqlHits, timeMySqlMbr) = RunMySqlWithMbr(mysqlConnection, coordinates, "Abfrage läuft");
        PrintResult("MySQL mit MBR", timeMySqlMbr, totalMySqlHits);

        // ------------------------------
        // MongoDB Test
        // ------------------------------
        PrintSection("MongoDB Geo-Abfragen ...");
        var (totalMongoHits, timeMongo) = RunMongo(mongoConnection, mongoDatabase, mongoCollection, coordinates, "Abfrage läuft");
        PrintResult("MongoDB", timeMongo, totalMongoHits);

        // ------------------------------
        // Vergleich
        // ------------------------------
        PrintSection("GESAMTVERGLEICH");

        if (timeMySqlMbr < timeMySqlNoMbr && timeMySqlMbr < timeMongo)
            Console.WriteLine($" → Schnellster: MySQL (mit MBR) – {timeMySqlMbr:F2} s");
        else if (timeMySqlNoMbr < timeMongo)
            Console.WriteLine($" → Schnellster: MySQL (ohne MBR) – {timeMySqlNoMbr:F2} s");
        else
            Console.WriteLine($" → Schnellster: MongoDB – {timeMongo:F2} s");

        Console.WriteLine("\nBenchmark abgeschlossen.\n");
    }

    // =======================
    // SCHÖNE AUSGABEN
    // =======================

    static void PrintHeader(string text)
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("===============================================");
        Console.WriteLine($"   {text.ToUpper()}");
        Console.WriteLine("===============================================\n");
        Console.ResetColor();
    }

    static void PrintSection(string text)
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine($"\n--- {text} ---");
        Console.ResetColor();
    }

    static void PrintResult(string name, double sec, int hits)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"[{name}]");
        Console.ResetColor();
        Console.WriteLine($"  Dauer: {sec:F2} Sekunden");
        Console.WriteLine($"  Treffer: {hits}\n");
    }

    // =======================
    // ANIMATIONEN
    // =======================
    static void SpinnerAnimation(int ms, string text)
    {
        char[] seq = { '⠙', '⠹', '⠸', '⠼', '⠴', '⠦', '⠧', '⠇', '⠏' };
        int steps = ms / 80;
        int idx = 0;
        Console.Write(text + " ");
        for (int i = 0; i < steps; i++)
        {
            Console.Write(seq[idx]);
            Console.SetCursorPosition(Console.CursorLeft - 1, Console.CursorTop);
            idx = (idx + 1) % seq.Length;
            Thread.Sleep(80);
        }
        Console.WriteLine();
    }

    static void ProgressBar(double progress, int width = 40)
    {
        int filled = (int)(progress * width);
        Console.Write("[");
        Console.Write(new string('█', filled));
        Console.Write(new string('-', width - filled));
        Console.Write($"] {progress*100:F1}%\r");
    }

    // =======================
    // GENERATE COORDINATES
    // =======================
    static (double lat, double lon, double latMinus, double latPlus, double lonMinus, double lonPlus, double radius)[] GenerateCoordinates(int num, Random rnd)
    {
        var coords = new (double, double, double, double, double, double, double)[num];
        for (int i = 0; i < num; i++)
        {
            double lat = 47.0 + rnd.NextDouble() * 2.0;
            double lon = 13.0 + rnd.NextDouble() * 3.0;
            double radius = 10.0 + rnd.NextDouble() * 10.0;
            double dy = radius / 55.0;
            double dx = radius / (111.0 * Math.Cos(lat * Math.PI / 180.0));
            coords[i] = (lat, lon, lat - dy, lat + dy, lon - dx, lon + dx, radius);
        }
        return coords;
    }

    // =======================
    // MYSQL & MONGO RUNNERS MIT ANIMATION
    // =======================
    static (int totalHits, double seconds) RunMySqlNoMbr(string connStr, (double lat, double lon, double latMinus, double latPlus, double lonMinus, double lonPlus, double radius)[] coordinates, string animText)
    {
        int totalHits = 0;
        var sw = Stopwatch.StartNew();

        using var conn = new MySqlConnection(connStr);
        conn.Open();

        int counter = 0;
        foreach (var coord in coordinates)
        {
            using var cmd = new MySqlCommand(@"
                SELECT TippID, ST_AsText(Coordinates)
                FROM tipp_locations
                WHERE ST_Distance_Sphere(
                    Coordinates,
                    ST_SRID(POINT(@lon, @lat), 4326)
                ) <= @radius * 1000;", conn);

            cmd.Parameters.AddWithValue("@lat", coord.lat);
            cmd.Parameters.AddWithValue("@lon", coord.lon);
            cmd.Parameters.AddWithValue("@radius", coord.radius);

            using var reader = cmd.ExecuteReader();
            while (reader.Read()) totalHits++;

            // Fortschritt
            counter++;
            ProgressBar((double)counter / coordinates.Length);
        }

        conn.Close();
        sw.Stop();
        Console.WriteLine();
        return (totalHits, sw.Elapsed.TotalSeconds);
    }

    static (int totalHits, double seconds) RunMySqlWithMbr(string connStr, (double lat, double lon, double latMinus, double latPlus, double lonMinus, double lonPlus, double radius)[] coordinates, string animText)
    {
        int totalHits = 0;
        var sw = Stopwatch.StartNew();

        using var conn = new MySqlConnection(connStr);
        conn.Open();

        int counter = 0;
        foreach (var coord in coordinates)
        {
            using var cmd = new MySqlCommand(@"
                SELECT TippID, ST_AsText(Coordinates)
                FROM tipp_locations
                WHERE MBRContains(
                    ST_GeomFromText(
                        CONCAT('POLYGON((',
                               @latMinus, ' ', @lonMinus, ', ',
                               @latPlus, ' ', @lonMinus, ', ',
                               @latPlus, ' ', @lonPlus, ', ',
                               @latMinus, ' ', @lonPlus, ', ',
                               @latMinus, ' ', @lonMinus, '))'), 4326
                    ),
                    Coordinates
                )
                AND ST_Distance_Sphere(
                    Coordinates,
                    ST_GeomFromText(CONCAT('POINT(', @lat, ' ', @lon, ')'), 4326)
                ) <= @radius * 1000;", conn);

            cmd.Parameters.AddWithValue("@lat", coord.lat);
            cmd.Parameters.AddWithValue("@lon", coord.lon);
            cmd.Parameters.AddWithValue("@radius", coord.radius);
            cmd.Parameters.AddWithValue("@latMinus", coord.latMinus);
            cmd.Parameters.AddWithValue("@latPlus", coord.latPlus);
            cmd.Parameters.AddWithValue("@lonMinus", coord.lonMinus);
            cmd.Parameters.AddWithValue("@lonPlus", coord.lonPlus);

            using var reader = cmd.ExecuteReader();
            while (reader.Read()) totalHits++;

            // Fortschritt
            counter++;
            ProgressBar((double)counter / coordinates.Length);
        }

        conn.Close();
        sw.Stop();
        Console.WriteLine();
        return (totalHits, sw.Elapsed.TotalSeconds);
    }

    static (int totalHits, double seconds) RunMongo(string connStr, string dbName, string collName, (double lat, double lon, double latMinus, double latPlus, double lonMinus, double lonPlus, double radius)[] coordinates, string animText)
    {
        int totalHits = 0;
        var sw = Stopwatch.StartNew();

        var client = new MongoClient(connStr);
        var db = client.GetDatabase(dbName);
        var collection = db.GetCollection<BsonDocument>(collName);

        int counter = 0;
        foreach (var coord in coordinates)
        {
            var point = new BsonDocument
            {
                { "type", "Point" },
                { "coordinates", new BsonArray { coord.lon, coord.lat } }
            };

            var pipeline = new[] { new BsonDocument("$geoNear", new BsonDocument
                {
                    { "near", point },
                    { "distanceField", "distance_km" },
                    { "spherical", true },
                    { "distanceMultiplier", 0.001 },
                    { "maxDistance", coord.radius * 1000 },
                    { "key", "Coordinates" }
                })
            };

            var results = collection.Aggregate<BsonDocument>(pipeline).ToList();
            totalHits += results.Count;

            counter++;
            ProgressBar((double)counter / coordinates.Length);
        }

        sw.Stop();
        Console.WriteLine();
        return (totalHits, sw.Elapsed.TotalSeconds);
    }
}
