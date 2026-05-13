using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using TrainerPlanner.Services;
using TrainerPlanner.ViewModels;

namespace TrainerPlanner
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            var culture = new CultureInfo("ru-RU");

            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = culture;

            // Для .NET 6+
            CultureInfo.DefaultThreadCurrentCulture = culture;
            CultureInfo.DefaultThreadCurrentUICulture = culture;

            // Общая инфраструктура
            var dbService = new DatabaseService();

            // Зависимости
            var scheduleRepo = new SqlScheduleRepository(dbService);
            var trainerRepo = new TrainerService(dbService);
            var viewModel = new MainViewModel(scheduleRepo, trainerRepo);

            // К главному окну
            var mainWindow = new MainWindow();
            mainWindow.DataContext = viewModel;
            mainWindow.Show();

            base.OnStartup(e);
        }
    }
}