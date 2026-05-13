using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrainerPlanner.Models;

namespace TrainerPlanner.Services
{
    public interface ITrainerService
    {
        List<Trainer> GetAllTrainers();
        void AddTrainer(Trainer trainer);
        void DeleteTrainer(int id); 
    }
}

