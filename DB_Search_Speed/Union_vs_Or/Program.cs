using System;
using System.Diagnostics;
using MySql.Data.MySqlClient;

class Program
{
    static void Main()
    {
        string connectionString = @"server=localhost;userid=root;password=insy;database=DB_Search_Speed";
        int searchCount = 100;
        Random rnd = new Random();

        using (MySqlConnection conn = new MySqlConnection(connectionString))
        {
            conn.Open();

            // -------- OR-Suche auf number --------
            Console.WriteLine("Starte OR-Suche auf 'number'...");
            Stopwatch swOrNumber = Stopwatch.StartNew();

            using (MySqlCommand cmdOr = new MySqlCommand(
                "SELECT number FROM Words WHERE number = @num1 OR number = @num2", conn))
            {
                var param1 = cmdOr.Parameters.Add("@num1", MySqlDbType.Int32);
                var param2 = cmdOr.Parameters.Add("@num2", MySqlDbType.Int32);

                for (int i = 0; i < searchCount; i++)
                {
                    param1.Value = rnd.Next(1, 101);
                    param2.Value = rnd.Next(1, 101);

                    using (var reader = cmdOr.ExecuteReader()) { }
                }
            }
            swOrNumber.Stop();
            Console.WriteLine($"OR-Suche auf 'number' abgeschlossen: {swOrNumber.Elapsed.TotalMilliseconds:N0} ms\n");

            // -------- UNION-Suche auf number --------
            Console.WriteLine("Starte UNION-Suche auf 'number'...");
            Stopwatch swUnionNumber = Stopwatch.StartNew();

            using (MySqlCommand cmdUnion = new MySqlCommand(
                "SELECT number FROM Words WHERE number = @num1 " +
                "UNION " +
                "SELECT number FROM Words WHERE number = @num2", conn))
            {
                var param1 = cmdUnion.Parameters.Add("@num1", MySqlDbType.Int32);
                var param2 = cmdUnion.Parameters.Add("@num2", MySqlDbType.Int32);

                for (int i = 0; i < searchCount; i++)
                {
                    param1.Value = rnd.Next(1, 101);
                    param2.Value = rnd.Next(1, 101);

                    using (var reader = cmdUnion.ExecuteReader()) { }
                }
            }
            swUnionNumber.Stop();
            Console.WriteLine($"UNION-Suche auf 'number' abgeschlossen: {swUnionNumber.Elapsed.TotalMilliseconds:N0} ms\n");

            // -------- OR-Suche auf value --------
            Console.WriteLine("Starte OR-Suche auf 'value'...");
            Stopwatch swOrValue = Stopwatch.StartNew();

            using (MySqlCommand cmdOr = new MySqlCommand(
                "SELECT value FROM Words WHERE value = @val1 OR value = @val2", conn))
            {
                var param1 = cmdOr.Parameters.Add("@val1", MySqlDbType.VarChar);
                var param2 = cmdOr.Parameters.Add("@val2", MySqlDbType.VarChar);

                for (int i = 0; i < searchCount; i++)
                {
                    param1.Value = GenerateRandomWord(10, rnd);
                    param2.Value = GenerateRandomWord(10, rnd);

                    using (var reader = cmdOr.ExecuteReader()) { }
                }
            }
            swOrValue.Stop();
            Console.WriteLine($"OR-Suche auf 'value' abgeschlossen: {swOrValue.Elapsed.TotalMilliseconds:N0} ms\n");

            // -------- UNION-Suche auf value --------
            Console.WriteLine("Starte UNION-Suche auf 'value'...");
            Stopwatch swUnionValue = Stopwatch.StartNew();

            using (MySqlCommand cmdUnion = new MySqlCommand(
                "SELECT value FROM Words WHERE value = @val1 " +
                "UNION " +
                "SELECT value FROM Words WHERE value = @val2", conn))
            {
                var param1 = cmdUnion.Parameters.Add("@val1", MySqlDbType.VarChar);
                var param2 = cmdUnion.Parameters.Add("@val2", MySqlDbType.VarChar);

                for (int i = 0; i < searchCount; i++)
                {
                    param1.Value = GenerateRandomWord(10, rnd);
                    param2.Value = GenerateRandomWord(10, rnd);

                    using (var reader = cmdUnion.ExecuteReader()) { }
                }
            }
            swUnionValue.Stop();
            Console.WriteLine($"UNION-Suche auf 'value' abgeschlossen: {swUnionValue.Elapsed.TotalMilliseconds:N0} ms");
        }
    }

    static string GenerateRandomWord(int length, Random rnd)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        char[] word = new char[length];
        for (int i = 0; i < length; i++)
            word[i] = chars[rnd.Next(chars.Length)];
        return new string(word);
    }
}
