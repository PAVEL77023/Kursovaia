using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using TrainerPlanner.Models;
using TrainerPlanner.Services;

namespace TrainerPlanner.Views
{
    public partial class AddTrainerWindow : Window
    {
        private readonly ITrainerService _trainerService;

        public AddTrainerWindow(ITrainerService trainerService)
        {
            InitializeComponent();
            _trainerService = trainerService;
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TbFullName.Text) ||
                string.IsNullOrWhiteSpace(TbSpecialization.Text))
            {
                MessageBox.Show("Заполните все обязательные поля", "Ошибка",
                                MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var trainer = new Trainer
            {
                FullName = TbFullName.Text,
                Specialization = TbSpecialization.Text,
                IsActive = true
            };

            _trainerService.AddTrainer(trainer);
            DialogResult = true;
            Close();
        }
    }
}
