using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrainerPlanner.Services
{
    public class DatabaseService
    {
        private readonly string _connectionString;

        public DatabaseService()
        {
            string exePath = System.Reflection.Assembly.GetExecutingAssembly().Location;
            string projectRoot = Path.GetFullPath(Path.Combine(exePath, @"..\..\..\.."));
            string dataPath = Path.Combine(projectRoot, "Data");
            if (!Directory.Exists(dataPath))
                Directory.CreateDirectory(dataPath);
            string dbPath = Path.Combine(dataPath, "fitness.db");
            System.Diagnostics.Debug.WriteLine($"Путь к БД: {dbPath}");
            if (!File.Exists(dbPath))
            {
                CreateDatabase(dbPath);
            }
            _connectionString = $"Data Source={dbPath};Version=3;Pooling=True;";
        }

        private void CreateDatabase(string dbPath)
        {
            using (var connection = new SQLiteConnection($"Data Source={dbPath};Version=3;"))
            {
                connection.Open();
                string createTrainers = @"
            CREATE TABLE Trainers (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                FullName TEXT NOT NULL,
                Specialization TEXT NOT NULL,
                Phone TEXT,
                IsActive BOOLEAN DEFAULT 1
            );";

                string createClients = @"
            CREATE TABLE Clients (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                FullName TEXT NOT NULL,
                Phone TEXT,
                Email TEXT,
                RegistrationDate DATETIME DEFAULT CURRENT_TIMESTAMP
            );";

                string createSchedule = @"
            CREATE TABLE Schedule (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                TrainerId INTEGER NOT NULL,
                StartTime DATETIME NOT NULL,
                EndTime DATETIME NOT NULL,
                Type TEXT NOT NULL,
                ClientName TEXT,
                FOREIGN KEY (TrainerId) REFERENCES Trainers(Id)
                    ON UPDATE CASCADE ON DELETE CASCADE
            );";

                using (var cmd = new SQLiteCommand(createTrainers, connection))
                    cmd.ExecuteNonQuery();

                using (var cmd = new SQLiteCommand(createClients, connection))
                    cmd.ExecuteNonQuery();

                using (var cmd = new SQLiteCommand(createSchedule, connection))
                    cmd.ExecuteNonQuery();
                string insertTrainers = @"
            INSERT INTO Trainers (FullName, Specialization, Phone, IsActive) 
            VALUES 
            ('Никита', 'Набор', '89096997889', 1),
            ('Даша', 'Кардио', '89096997889', 1);";

                using (var cmd = new SQLiteCommand(insertTrainers, connection))
                    cmd.ExecuteNonQuery();
            }
        }

        public SQLiteConnection GetConnection()
        {
            return new SQLiteConnection(_connectionString);
        }
    }
}