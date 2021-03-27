using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;

namespace AnkiExtraction
{
    public class Program
    {
        static void Main(string[] args)
        {
            string archivePath = "D:";
            string archiveName = "MyArchive.apkg";
            string destinationPath = "D:/DocumentsUnzipped";
            //Extract(archivePath, archiveName, destinationPath);

            List<string> flds = new List<string>();
            GetFieldsFromCollection(flds, destinationPath);

            foreach (var str in flds)
            {
                int audioNumber;
                Int32.TryParse(str.Substring(0, 4), out audioNumber);
                string audioName = str.Substring(12, 20);
                string aduioText = str.Substring(34);
                Console.WriteLine(audioNumber);
                Console.WriteLine(audioName);
                Console.WriteLine(aduioText);
            }

        }

        private static void GetFieldsFromCollection(List<string> flds, string collectionPath)
        {
            using (var connection = new SqliteConnection($"Data Source={collectionPath}/collection.anki2"))
            {
                connection.Open();

                var command = connection.CreateCommand();
                command.CommandText =
                @"
                    SELECT flds
                    FROM notes
                ";

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var name = reader.GetString(0);
                        flds.Add(name);
                    }
                }
            }
        }

        private static void Extract(string archivePath, string archiveName, string destinationPath)
        {
            if (Directory.Exists(archivePath))
            {
                ZipFile.ExtractToDirectory($"{archivePath}/{archiveName}", destinationPath);
            }
        }
    }
}
