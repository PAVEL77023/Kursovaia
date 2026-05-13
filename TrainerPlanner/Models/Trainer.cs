using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrainerPlanner.Models
{
    public class Trainer
    {
        public int Id { get; set; }
        public string FullName { get; set; }       // ФИО
        public string Specialization { get; set; } // Специализация 
        public string Phone { get; set; }          // Телефон
        public bool IsActive { get; set; }
        public bool IsSelected { get; set; }

        public override string ToString() => FullName;
    }
}
