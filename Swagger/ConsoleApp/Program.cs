// See https://aka.ms/new-console-template for more information

using Model; // Achte darauf, dass der Namespace zu deinem Scaffold-Ordner passt

// Die 'using'-Klammer sorgt dafür, dass die DB-Verbindung sauber geschlossen wird
using (var db = new SwaggerContext())
{
    var liste = db.Demos.ToList();

        foreach (var item in liste)
        {
            // Gibt ID und Value aus
            Console.WriteLine($"ID: {item.Id} | Value: {item.Value}");
        }
    }
