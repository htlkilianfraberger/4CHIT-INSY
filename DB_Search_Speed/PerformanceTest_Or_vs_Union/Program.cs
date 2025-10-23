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
/*
Ergebnisse OR vs UNION Performance-Test
----------------------------------
Ergebnisse (Durchschnittswerte):
- value: OR = 3,88s | UNION = 2,33s
  -> UNION schneller, weil "value" viele unterschiedliche Werte hat 
     und ein Index genutzt werden kann.

- number: OR = 10,14s | UNION = 6,70s
  -> UNION schneller, da "number" ebenfalls viele verschiedene Werte hat 
     und der Index Abfragen stark beschleunigt.

- category: OR = 4,30s | UNION = 26,36s
  -> OR schneller, obwohl es einen Index gibt, weil nur wenige verschiedene Werte existieren,
     sodass UNION mehrere Abfragen ausführt, die sich stark überschneiden.

- createdAt: OR = 6,96s | UNION = 6,42s
  -> Bei Datum beide ähnlich schnell

- isActive: OR = 2,55s | UNION = 14,16s
  -> OR deutlich schneller, da "isActive" boolesch ist (nur wenige verschiedene Werte),
     der Index bringt kaum Vorteil und UNION führt unnötig mehrere Scans aus.

- description: OR = 4,81s | UNION = 6,25s
  -> OR schneller, weil auf "description" kein Index existiert, 
     UNION aber zwei volle Table-Scans erzeugt, die langsamer sind.

Fazit
----------------------------------
UNION ist schneller, wenn viele verschiedene Werte vorkommen 
(z. B. "value", "number") und ein Index effektiv genutzt werden kann.

OR ist schneller, wenn oft die gleichen Werte auftreten 
(z. B. boolesche Felder wie "isActive" oder wenige Kategorien),
weil die Abfrage nur einmal über die Tabelle läuft, während UNION 
mehrere vollständige Abfragen ausführt und dadurch langsamer wird.
*/