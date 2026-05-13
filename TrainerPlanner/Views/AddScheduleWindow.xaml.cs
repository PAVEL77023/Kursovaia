using System.Windows.Media;
using System.Windows;
using TrainerPlanner.Models;
using TrainerPlanner.Services;
using System;
using System.Windows.Controls;

namespace TrainerPlanner.Views
{
    public partial class AddScheduleWindow : Window
    {
        private readonly IScheduleRepository _repository;
        private readonly ITrainerService _trainerService;
        private const string PlaceholderText = "Введите ФИО клиента";

        public AddScheduleWindow(IScheduleRepository repository, ITrainerService trainerService)
        {
            InitializeComponent();
            _repository = repository;
            _trainerService = trainerService;
            CbTrainer.ItemsSource = _trainerService.GetAllTrainers();
            if (CbTrainer.Items.Count > 0)
                CbTrainer.SelectedIndex = 0;

            LoadTimeCombos();

            TbClientName.Text = PlaceholderText;
            TbClientName.Foreground = Brushes.Gray;
        }

        private void LoadTimeCombos()
        {
            for (int i = 0; i < 24; i++)
            {
                CbStartHour.Items.Add(i.ToString("D2"));
                CbEndHour.Items.Add(i.ToString("D2"));
            }
            foreach (int minute in new[] { 0, 15, 30, 45 })
            {
                CbStartMinute.Items.Add(minute.ToString("D2"));
                CbEndMinute.Items.Add(minute.ToString("D2"));
            }
            CbStartHour.SelectedIndex = 9;
            CbStartMinute.SelectedIndex = 0;
            CbEndHour.SelectedIndex = 10;
            CbEndMinute.SelectedIndex = 0;
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (CbTrainer.SelectedItem == null)
            {
                MessageBox.Show("Выберите тренера", "Ошибка",
                                MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var clientName = TbClientName.Text.Trim();
            if (string.IsNullOrWhiteSpace(clientName) || clientName == PlaceholderText)
            {
                MessageBox.Show("Укажите ФИО клиента", "Ошибка",
                                MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!DpDate.SelectedDate.HasValue)
            {
                MessageBox.Show("Выберите дату", "Ошибка",
                                MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (CbStartHour.SelectedItem == null || CbStartMinute.SelectedItem == null ||
                CbEndHour.SelectedItem == null || CbEndMinute.SelectedItem == null)
            {
                MessageBox.Show("Выберите время начала и окончания", "Ошибка",
                                MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var trainer = (Trainer)CbTrainer.SelectedItem;
            var date = DpDate.SelectedDate.Value;

            if (!int.TryParse(CbStartHour.SelectedItem.ToString(), out int startHour) ||
                !int.TryParse(CbStartMinute.SelectedItem.ToString(), out int startMinute) ||
                !int.TryParse(CbEndHour.SelectedItem.ToString(), out int endHour) ||
                !int.TryParse(CbEndMinute.SelectedItem.ToString(), out int endMinute))
            {
                MessageBox.Show("Некорректное время", "Ошибка",
                                MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var start = date.AddHours(startHour).AddMinutes(startMinute);
            var end = date.AddHours(endHour).AddMinutes(endMinute);

            if (end <= start)
            {
                MessageBox.Show("Время окончания должно быть позже начала", "Ошибка",
                                MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var item = new ScheduleItem
            {
                TrainerId = trainer.Id,
                TrainerName = trainer.FullName,
                StartTime = start,
                EndTime = end,
                Type = CbType.Text,
                ClientName = clientName
            };

            _repository.AddScheduleItem(item);
            DialogResult = true;
            Close();
        }
        private void TbClientName_GotFocus(object sender, RoutedEventArgs e)
        {
            if (TbClientName.Text == PlaceholderText)
            {
                TbClientName.Text = "";
                TbClientName.Foreground = Brushes.Black;
            }
        }

        private void TbClientName_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TbClientName.Text))
            {
                TbClientName.Text = PlaceholderText;
                TbClientName.Foreground = Brushes.Gray;
            }
        }
    }
}