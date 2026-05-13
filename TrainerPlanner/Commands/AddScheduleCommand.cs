using System.Windows.Input;
using System;
using TrainerPlanner.Models;
using TrainerPlanner.ViewModels;

namespace TrainerPlanner.Commands
{
    public class AddScheduleCommand : ICommand
    {
        private readonly MainViewModel _viewModel;

        public AddScheduleCommand(MainViewModel viewModel)
        {
            _viewModel = viewModel;
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter) => true;

        public void Execute(object parameter)
        {
            //логика добавления занятия
            var newItem = new ScheduleItem
            {
                TrainerId = 1,
                StartTime = DateTime.Now.AddHours(1),
                EndTime = DateTime.Now.AddHours(2),
                MaxClients = 10,
                ClientName = "Новый клиент"
            };
            _viewModel.Schedule.Add(newItem);
        }
    }
}