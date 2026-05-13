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
using System.Windows.Navigation;
using System.Windows.Shapes;
using TrainerPlanner.Services;

namespace TrainerPlanner
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            // Проверка подключения к SQLite и чтения данных
            try
            {
                var dbService = new DatabaseService();
                var repo = new SqlScheduleRepository(dbService);
                var trainers = repo.GetTrainers();

                System.Console.WriteLine($"Найдено тренеров: {trainers.Count}");
                foreach (var t in trainers)
                {
                    System.Console.WriteLine($"- {t.FullName} ({t.Specialization})");
                }
            }
            catch (Exception ex)
            {
                System.Console.WriteLine($"Ошибка при подключении к базе данных: {ex.Message}");
                MessageBox.Show($"Не удалось загрузить данные: {ex.Message}", "Ошибка БД",
                                MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}