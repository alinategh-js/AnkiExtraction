using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
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
            string destinationPath = "D:/wfd";
            // Extracting audios from the .apkg file //
            Extract(archivePath, archiveName, destinationPath);

            List<string> flds = new List<string>();
            // Get audio texts from the 'collection' sqlite database //
            GetFieldsFromCollection(flds, destinationPath);

            // renaming audio files and adding .mp3 to them //
            DirectoryInfo d = new DirectoryInfo(destinationPath);
            FileInfo[] infos = d.GetFiles();
            foreach (FileInfo f in infos)
            {
                if(f.Name != "collection.anki2" && f.Name != "media") 
                {
                    File.Move(f.FullName, f.FullName.Insert(f.FullName.Length, ".mp3"));
                }
            }


            // storing audio paths and texts into the database //
            foreach (var str in flds)
            {
                int audioNumber;
                Int32.TryParse(str.Substring(0, 4), out audioNumber);
                //string audioName = str.Substring(12, 20);
                string audioText = str.Substring(34);
                InsertIntoDatabase($"{destinationPath}/{audioNumber}.mp3", audioText);
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

        private static void InsertIntoDatabase(string audioPath, string audioText)
        {
            using (SqlConnection connection = new SqlConnection("server=.;database=LectureDB;Trusted_Connection=true"))
            {
                connection.Open();
                string sql = "INSERT INTO Audio(AudioPath, AudioText) VALUES(@param1,@param2)";
                using (SqlCommand cmd = new SqlCommand(sql, connection))
                {
                    cmd.Parameters.Add("@param1", SqlDbType.NVarChar).Value = audioPath;
                    cmd.Parameters.Add("@param2", SqlDbType.NVarChar).Value = audioText;
                    cmd.CommandType = CommandType.Text;
                    cmd.ExecuteNonQuery();
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
