using ProjectFLS.flsdbDataSetTableAdapters;
using ProjectFLS.UI;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ProjectFLS.Admin.DataPages.DerictoriesButton.Derictories
{
    public partial class AddEditCityPage : Page
    {
        private readonly int? _cityID;
        private readonly citiesTableAdapter _adapter;

        public AddEditCityPage(int? cityID = null)
        {
            InitializeComponent();
            _cityID = cityID;
            _adapter = new citiesTableAdapter();

            if (_cityID.HasValue)
                LoadCityData(_cityID.Value);
        }

        private void LoadCityData(int id)
        {
            var row = _adapter.GetData().FirstOrDefault(t => t.cityID == id);
            if (row != null)
            {
                CityNameTextBox.Text = row.cityName;
            }
        }

        private void UpdateCityRecord(int cityID, string name)
        {
            var data = _adapter.GetData();
            var row = data.FirstOrDefault(t => t.cityID == cityID);

            if (row != null)
            {
                row.cityName = name;
                _adapter.Update(row); // Сохранение изменений в базу данных
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            // Очистим фон поля
            CityNameTextBox.ClearValue(TextBox.BackgroundProperty);

            bool hasError = false;
            var redBrush = new SolidColorBrush(Color.FromRgb(255, 200, 200));

            // Проверка на пустое название города
            if (string.IsNullOrWhiteSpace(CityNameTextBox.Text))
            {
                CityNameTextBox.Background = redBrush;
                hasError = true;
            }

            if (hasError)
            {
                CustomMessageBox.Show("Пожалуйста, заполните все поля корректно.", "Ошибка", showCancel: false);
                return;
            }

            try
            {
                // Если есть ID города, то обновляем существующий
                if (_cityID.HasValue)
                {
                    UpdateCityRecord(_cityID.Value, CityNameTextBox.Text);
                    CustomMessageBox.Show("Город обновлён.", "Успешно", showCancel: false);
                }
                else
                {
                    // Добавляем новый город
                    _adapter.Insert(CityNameTextBox.Text);
                    CustomMessageBox.Show("Город добавлен.", "Успешно", showCancel: false);
                }

                // Обновляем список городов
                NavigationService.GoBack();
            }
            catch (Exception ex)
            {
                CustomMessageBox.Show($"Ошибка при сохранении города: {ex.Message}", "Ошибка", showCancel: false);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            App.mainStackPanel.IsEnabled = true;
            NavigationService?.GoBack();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            App.mainStackPanel.IsEnabled = false;
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            App.mainStackPanel.IsEnabled = true;
        }
    }
}
