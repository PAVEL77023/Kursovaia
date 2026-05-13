using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using TrainerPlanner.Commands;
using TrainerPlanner.Models;
using TrainerPlanner.Services;
using TrainerPlanner.Views;
namespace TrainerPlanner.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<Trainer> Trainers { get; set; }
        public ObservableCollection<ScheduleItem> Schedule { get; set; }

        private DateTime _selectedDate = DateTime.Today;
        public DateTime SelectedDate
        {
            get => _selectedDate;
            set
            {
                _selectedDate = value;
                OnPropertyChanged();
            }
        }

        public DateTime Today => DateTime.Today;

        private IScheduleRepository _scheduleRepo;
        private ITrainerService _trainerRepo;

        public ICommand AddScheduleCommand { get; private set; }
        public ICommand DeleteSelectedCommand { get; private set; }
        public ICommand AddTrainerCommand { get; private set; }
        public ICommand DeleteTrainerCommand { get; private set; }

        public MainViewModel()
        {
            try
            {
                InitializeDependencies();
                InitializeCommands();
                LoadData();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка инициализации: {ex.Message}", "Критическая ошибка",
                                MessageBoxButton.OK, MessageBoxImage.Error);
                throw;
            }
        }

        public MainViewModel(IScheduleRepository scheduleRepo, ITrainerService trainerRepo)
        {
            _scheduleRepo = scheduleRepo ?? throw new ArgumentNullException(nameof(scheduleRepo));
            _trainerRepo = trainerRepo ?? throw new ArgumentNullException(nameof(trainerRepo));
            InitializeCommands();
            LoadData();
        }

        private void InitializeDependencies()
        {
            var dbService = new DatabaseService();
            _scheduleRepo = new SqlScheduleRepository(dbService);
            _trainerRepo = new TrainerService(dbService);
        }

        private void InitializeCommands()
        {
            AddScheduleCommand = new RelayCommand(OpenAddScheduleWindow);
            DeleteSelectedCommand = new RelayCommand(DeleteSelectedItems);
            AddTrainerCommand = new RelayCommand(OpenAddTrainerWindow);
            DeleteTrainerCommand = new RelayCommand(DeleteSelectedTrainers);
        }

        private void LoadData()
        {
            Trainers = new ObservableCollection<Trainer>(_trainerRepo.GetAllTrainers());
            Schedule = new ObservableCollection<ScheduleItem>(_scheduleRepo.GetAllSchedule());
        }

        public void RefreshAllSchedule()
        {
            Schedule.Clear();
            foreach (var item in _scheduleRepo.GetAllSchedule())
            {
                Schedule.Add(item);
            }
        }

        public void RefreshTrainers()
        {
            Trainers.Clear();
            foreach (var trainer in _trainerRepo.GetAllTrainers())
            {
                Trainers.Add(trainer);
            }
        }

        private void OpenAddScheduleWindow(object parameter)
        {
            var window = new AddScheduleWindow(_scheduleRepo, _trainerRepo);
            if (window.ShowDialog() == true)
            {
                RefreshAllSchedule();
            }
        }

        private void DeleteSelectedItems(object parameter)
        {
            var selectedItems = Schedule.Where(item => item.IsSelected).ToList();

            if (!selectedItems.Any())
            {
                MessageBox.Show("Выберите хотя бы одну запись для удаления", "Внимание",
                                MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var result = MessageBox.Show(
                $"Вы уверены, что хотите удалить {selectedItems.Count} записей?",
                "Подтверждение удаления",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                foreach (var item in selectedItems)
                {
                    _scheduleRepo.DeleteScheduleItem(item.Id);
                }

                RefreshAllSchedule();
                MessageBox.Show($"Удалено {selectedItems.Count} записей", "Успех",
                                MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
        private void DeleteSelectedTrainers(object parameter)
        {
            var selectedTrainers = Trainers.Where(t => t.IsSelected).ToList();

            if (!selectedTrainers.Any())
            {
                MessageBox.Show("Выберите тренера для удаления", "Ошибка",
                                MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var result = MessageBox.Show(
                $"Вы уверены, что хотите уволить {selectedTrainers.Count} тренеров?",
                "Подтверждение удаления",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    foreach (var trainer in selectedTrainers)
                    {
                        _trainerRepo.DeleteTrainer(trainer.Id);
                    }

                    RefreshTrainers();
                    RefreshAllSchedule();
                    MessageBox.Show($"Уволено {selectedTrainers.Count} тренеров", "Успех",
                                    MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при удалении: {ex.Message}", "Ошибка",
                                    MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void OpenAddTrainerWindow(object parameter)
        {
            var window = new AddTrainerWindow(_trainerRepo);
            if (window.ShowDialog() == true)
            {
                RefreshTrainers();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}