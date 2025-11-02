using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;

namespace CustomOOBE.Pages
{
    public static class ReviewDatabase
    {
        private static string GetDatabasePath()
        {
            string appData = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
            string dbFolder = Path.Combine(appData, "CustomOOBE");
            Directory.CreateDirectory(dbFolder);
            return Path.Combine(dbFolder, "reviews.db");
        }

        private static async Task InitializeDatabaseAsync()
        {
            string dbPath = GetDatabasePath();
            string connectionString = $"Data Source={dbPath}";

            using var connection = new SqliteConnection(connectionString);
            await connection.OpenAsync();

            string createTableQuery = @"
                CREATE TABLE IF NOT EXISTS Reviews (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Rating INTEGER NOT NULL,
                    Comment TEXT,
                    Timestamp TEXT NOT NULL,
                    MachineName TEXT,
                    UserName TEXT
                )";

            using var command = new SqliteCommand(createTableQuery, connection);
            await command.ExecuteNonQueryAsync();
        }

        public static async Task SaveReviewAsync(Review review)
        {
            await InitializeDatabaseAsync();

            string dbPath = GetDatabasePath();
            string connectionString = $"Data Source={dbPath}";

            using var connection = new SqliteConnection(connectionString);
            await connection.OpenAsync();

            string insertQuery = @"
                INSERT INTO Reviews (Rating, Comment, Timestamp, MachineName, UserName)
                VALUES (@Rating, @Comment, @Timestamp, @MachineName, @UserName)";

            using var command = new SqliteCommand(insertQuery, connection);
            command.Parameters.AddWithValue("@Rating", review.Rating);
            command.Parameters.AddWithValue("@Comment", review.Comment ?? string.Empty);
            command.Parameters.AddWithValue("@Timestamp", review.Timestamp.ToString("o"));
            command.Parameters.AddWithValue("@MachineName", review.MachineName);
            command.Parameters.AddWithValue("@UserName", review.UserName);

            await command.ExecuteNonQueryAsync();
        }

        public static async Task<List<Review>> GetAllReviewsAsync()
        {
            await InitializeDatabaseAsync();

            var reviews = new List<Review>();
            string dbPath = GetDatabasePath();
            string connectionString = $"Data Source={dbPath}";

            using var connection = new SqliteConnection(connectionString);
            await connection.OpenAsync();

            string selectQuery = "SELECT * FROM Reviews ORDER BY Timestamp DESC";

            using var command = new SqliteCommand(selectQuery, connection);
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                reviews.Add(new Review
                {
                    Id = reader.GetInt32(0),
                    Rating = reader.GetInt32(1),
                    Comment = reader.IsDBNull(2) ? string.Empty : reader.GetString(2),
                    Timestamp = DateTime.Parse(reader.GetString(3)),
                    MachineName = reader.IsDBNull(4) ? string.Empty : reader.GetString(4),
                    UserName = reader.IsDBNull(5) ? string.Empty : reader.GetString(5)
                });
            }

            return reviews;
        }
    }
}
