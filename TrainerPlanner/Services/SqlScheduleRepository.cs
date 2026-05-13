using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrainerPlanner.Models;
using TrainerPlanner.Services;

namespace TrainerPlanner.Services
{
    public class SqlScheduleRepository : IScheduleRepository
    {
        private readonly DatabaseService _dbService;

        public SqlScheduleRepository(DatabaseService dbService)
        {
            _dbService = dbService;
        }

        public List<Trainer> GetTrainers()
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

        public List<ScheduleItem> GetSchedule(DateTime date)
        {
            var schedule = new List<ScheduleItem>();
            var startOfDay = date.Date;
            var endOfDay = startOfDay.AddDays(1).AddTicks(-1);

            using (var connection = _dbService.GetConnection())
            {
                connection.Open();

                using (var command = new SQLiteCommand(
                    "SELECT s.Id, s.TrainerId, t.FullName, s.StartTime, s.EndTime, s.Type, s.ClientName " +
                    "FROM Schedule s " +
                    "JOIN Trainers t ON s.TrainerId = t.Id " +
                    "WHERE s.StartTime >= @start AND s.StartTime <= @end " +
                    "ORDER BY s.StartTime", connection))
                {
                    command.Parameters.AddWithValue("@start", startOfDay);
                    command.Parameters.AddWithValue("@end", endOfDay);

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            schedule.Add(new ScheduleItem
                            {
                                Id = reader.GetInt32(0),
                                TrainerId = reader.GetInt32(1),
                                TrainerName = reader.GetString(2),
                                StartTime = reader.GetDateTime(3),
                                EndTime = reader.GetDateTime(4),
                                Type = reader.GetString(5),
                                ClientName = reader.IsDBNull(6) ? "" : reader.GetString(6),
                                IsSelected = false
                            });
                        }
                    }
                }
            }

            return schedule;
        }

        public List<ScheduleItem> GetAllSchedule()
        {
            var schedule = new List<ScheduleItem>();

            using (var connection = _dbService.GetConnection())
            {
                connection.Open();

                using (var command = new SQLiteCommand(
                    "SELECT s.Id, s.TrainerId, t.FullName, s.StartTime, s.EndTime, s.Type, s.ClientName " +
                    "FROM Schedule s " +
                    "JOIN Trainers t ON s.TrainerId = t.Id " +
                    "ORDER BY s.StartTime", connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            schedule.Add(new ScheduleItem
                            {
                                Id = reader.GetInt32(0),
                                TrainerId = reader.GetInt32(1),
                                TrainerName = reader.GetString(2),
                                StartTime = reader.GetDateTime(3),
                                EndTime = reader.GetDateTime(4),
                                Type = reader.GetString(5),
                                ClientName = reader.IsDBNull(6) ? "" : reader.GetString(6),
                                IsSelected = false
                            });
                        }
                    }
                }
            }

            return schedule;
        }

        public void AddScheduleItem(ScheduleItem item)
        {
            using (var connection = _dbService.GetConnection())
            {
                connection.Open();

                using (var command = new SQLiteCommand(
                    "INSERT INTO Schedule (TrainerId, StartTime, EndTime, Type, ClientName) " +
                    "VALUES (@trainerId, @startTime, @endTime, @type, @clientName)", connection))
                {
                    command.Parameters.AddWithValue("@trainerId", item.TrainerId);
                    command.Parameters.AddWithValue("@startTime", item.StartTime);
                    command.Parameters.AddWithValue("@endTime", item.EndTime);
                    command.Parameters.AddWithValue("@type", item.Type);
                    command.Parameters.AddWithValue("@clientName", item.ClientName ?? "");
                    command.ExecuteNonQuery();
                }
            }
        }

        public void DeleteScheduleItem(int id)
        {
            using (var connection = _dbService.GetConnection())
            {
                connection.Open();

                using (var command = new SQLiteCommand(
                    "DELETE FROM Schedule WHERE Id = @id", connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    command.ExecuteNonQuery();
                }
            }
        }

        public void SaveChanges()
        {
        }
    }
}