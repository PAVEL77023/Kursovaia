using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrainerPlanner.Models
{
    public class ScheduleItem
    {
        public int Id { get; set; }
        public int TrainerId { get; set; }
        public DateTime StartTime { get; set; }  // Начало занятия
        public DateTime EndTime { get; set; }    // Окончание
        public string Type { get; set; }         // Групповое/персональное
        public int MaxClients { get; set; }
        public string ClientName { get; set; } = string.Empty;   // ФИО клиента
        public string TrainerName { get; set; }

        // Для отображения в DataGrid
        public string DisplayTime => $"{StartTime:HH:mm} - {EndTime:HH:mm}";

        //для выбора записей
        public bool IsSelected { get; set; }
    }
}
