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

            // -------- OR-Suche --------
            Console.WriteLine("Starte OR-Suche...");
            Stopwatch swOr = Stopwatch.StartNew();

            using (MySqlCommand cmdOr = new MySqlCommand(
                "SELECT number FROM Words WHERE number = @num1 OR number = @num2", conn))
            {
                var param1 = cmdOr.Parameters.Add("@num1", MySqlDbType.Int32);
                var param2 = cmdOr.Parameters.Add("@num2", MySqlDbType.Int32);

                for (int i = 0; i < searchCount; i++)
                {
                    int num1 = rnd.Next(1, 21);
                    int num2 = rnd.Next(1, 21);

                    param1.Value = num1;
                    param2.Value = num2;

                    using (var reader = cmdOr.ExecuteReader())
                    {
                    }
                }
            }

            swOr.Stop();
            Console.WriteLine($"OR-Suche abgeschlossen. Benötigte Zeit: {swOr.Elapsed.TotalMilliseconds:N0} ms\n");

            // -------- UNION-Suche --------
            Console.WriteLine("Starte UNION-Suche...");
            Stopwatch swUnion = Stopwatch.StartNew();

            using (MySqlCommand cmdUnion = new MySqlCommand(
                "SELECT number FROM Words WHERE number = @num1 " +
                "UNION " +
                "SELECT number FROM Words WHERE number = @num2", conn))
            {
                var param1 = cmdUnion.Parameters.Add("@num1", MySqlDbType.Int32);
                var param2 = cmdUnion.Parameters.Add("@num2", MySqlDbType.Int32);

                for (int i = 0; i < searchCount; i++)
                {
                    int num1 = rnd.Next(1, 21);
                    int num2 = rnd.Next(1, 21);

                    param1.Value = num1;
                    param2.Value = num2;

                    using (var reader = cmdUnion.ExecuteReader())
                    {
                    }
                }
            }

            swUnion.Stop();
            Console.WriteLine($"UNION-Suche abgeschlossen. Benötigte Zeit: {swUnion.Elapsed.TotalMilliseconds:N0} ms");
        }
    }
}
