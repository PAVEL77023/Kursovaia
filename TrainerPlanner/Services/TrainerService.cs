using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using TrainerPlanner.Models;
using TrainerPlanner.Services;

namespace TrainerPlanner.Services
{
    public class TrainerService : ITrainerService
    {
        private readonly DatabaseService _dbService;

        public TrainerService(DatabaseService dbService)
        {
            _dbService = dbService;
        }

        public List<Trainer> GetAllTrainers()
        {
            var trainers = new List<Trainer>();

            using (var connection = _dbService.GetConnection())
            {
                connection.Open();

                using (var command = new SQLiteCommand(
                    "SELECT Id, FullName, Specialization, Phone, IsActive FROM Trainers",
                    connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            trainers.Add(new Trainer
                            {
                                Id = reader.GetInt32(0),
                                FullName = reader.GetString(1),
                                Specialization = reader.GetString(2),
                                Phone = reader.IsDBNull(3) ? "" : reader.GetString(3),
                                IsActive = reader.GetBoolean(4)
                            });
                        }
                    }
                }
            }

            return trainers;
        }

        public void AddTrainer(Trainer trainer)
        {
            using (var connection = _dbService.GetConnection())
            {
                connection.Open();

                using (var command = new SQLiteCommand(
                    "INSERT INTO Trainers (FullName, Specialization, Phone, IsActive) " +
                    "VALUES (@fullName, @specialization, @phone, @isActive)", connection))
                {
                    command.Parameters.AddWithValue("@fullName", trainer.FullName);
                    command.Parameters.AddWithValue("@specialization", trainer.Specialization);
                    command.Parameters.AddWithValue("@phone", trainer.Phone ?? "");
                    command.Parameters.AddWithValue("@isActive", trainer.IsActive);
                    command.ExecuteNonQuery();
                }
            }
        }

        public void DeleteTrainer(int id)
        {
            using (var connection = _dbService.GetConnection())
            {
                connection.Open();
                using (var deleteScheduleCmd = new SQLiteCommand(
                    "DELETE FROM Schedule WHERE TrainerId = @trainerId", connection))
                {
                    deleteScheduleCmd.Parameters.AddWithValue("@trainerId", id);
                    deleteScheduleCmd.ExecuteNonQuery();
                }
                using (var deleteTrainerCmd = new SQLiteCommand(
                    "DELETE FROM Trainers WHERE Id = @id", connection))
                {
                    deleteTrainerCmd.Parameters.AddWithValue("@id", id);
                    deleteTrainerCmd.ExecuteNonQuery();
                }
            }
        }
    }
}