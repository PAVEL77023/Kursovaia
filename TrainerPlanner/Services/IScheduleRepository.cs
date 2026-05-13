using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrainerPlanner.Models;

namespace TrainerPlanner.Services
{
    public interface IScheduleRepository
    {
        List<ScheduleItem> GetSchedule(DateTime date);
        List<ScheduleItem> GetAllSchedule();
        void AddScheduleItem(ScheduleItem item);
        void DeleteScheduleItem(int id);
        void SaveChanges();
    }
}