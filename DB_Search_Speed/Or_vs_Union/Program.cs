using System;
using System.Diagnostics;
using MySql.Data.MySqlClient;

class Program
{
    static void Main()
    {
        string connectionString = @"server=localhost;userid=root;password=insy;database=DB_Search_Speed";

        Random rnd = new Random();

        using (MySqlConnection conn = new MySqlConnection(connectionString))
        {
            conn.Open();
            
            RunTest(conn, "value", 2000, () => GenerateRandomWord(10, rnd), rnd);
            RunTest(conn, "number", 30, () => rnd.Next(1, 101), rnd);
            RunTest(conn, "category", 2, () => new string[] { "A","B","C","D","E" }[rnd.Next(5)], rnd);
            RunTest(conn, "createdAt", 5000, () => RandomDate(rnd), rnd);
            RunTest(conn, "isActive", 1, () => rnd.Next(0,2)==0, rnd);
            RunTest(conn, "description", 5, () => GenerateRandomWord(20, rnd), rnd);
        }
    }

    static void RunTest<T>(MySqlConnection conn, string columnName, int count, Func<T> generateValue, Random rnd)
    {
        Console.WriteLine($"Starte OR vs UNION Test auf '{columnName}' mit {count} Durchläufen...");
        
        Stopwatch swOr = Stopwatch.StartNew();
        for (int i = 0; i < count; i++)
        {
            T val1 = generateValue();
            T val2 = generateValue();

            string sqlOr = $"SELECT * FROM Words WHERE {columnName} = @val1 OR {columnName} = @val2;";
            using var cmd = new MySqlCommand(sqlOr, conn);
            cmd.Parameters.AddWithValue("@val1", val1);
            cmd.Parameters.AddWithValue("@val2", val2);
            using var reader = cmd.ExecuteReader();
            while (reader.Read()) { }
        }
        swOr.Stop();
        Console.WriteLine($"OR-Abfragen: {swOr.Elapsed.TotalSeconds:N2} Sekunden");
        
        Stopwatch swUnion = Stopwatch.StartNew();
        for (int i = 0; i < count; i++)
        {
            T val1 = generateValue();
            T val2 = generateValue();

            string sqlUnion = $@"
                SELECT * FROM Words WHERE {columnName} = @val1
                UNION
                SELECT * FROM Words WHERE {columnName} = @val2;";
            using var cmd = new MySqlCommand(sqlUnion, conn);
            cmd.Parameters.AddWithValue("@val1", val1);
            cmd.Parameters.AddWithValue("@val2", val2);
            using var reader = cmd.ExecuteReader();
            while (reader.Read()) { }
        }
        swUnion.Stop();
        Console.WriteLine($"UNION-Abfragen: {swUnion.Elapsed.TotalSeconds:N2} Sekunden\n");
    }

    static string GenerateRandomWord(int length, Random rnd)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        char[] word = new char[length];
        for (int i = 0; i < length; i++)
            word[i] = chars[rnd.Next(chars.Length)];
        return new string(word);
    }

    static DateTime RandomDate(Random rnd)
    {
        int year = rnd.Next(2020, 2026);
        int month = rnd.Next(1, 13);
        int day = rnd.Next(1, 29);
        int hour = rnd.Next(0, 24);
        int minute = rnd.Next(0, 60);
        int second = rnd.Next(0, 60);
        return new DateTime(year, month, day, hour, minute, second);
    }
}
