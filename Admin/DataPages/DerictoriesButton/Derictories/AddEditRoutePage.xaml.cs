using ProjectFLS.flsdbDataSetTableAdapters;
using ProjectFLS.UI;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace ProjectFLS.Admin.DataPages.DerictoriesButton.Derictories
{
    public partial class AddEditRoutePage : Page
    {
        private readonly int? _routeID;
        private readonly routesTableAdapter _adapter;
        private readonly citiesTableAdapter _citiesAdapter;

        public AddEditRoutePage(int? routeID = null)
        {
            InitializeComponent();
            _routeID = routeID;
            _adapter = new routesTableAdapter();
            _citiesAdapter = new citiesTableAdapter();


            LoadComboBoxData();

            if (_routeID.HasValue)
                LoadRouteData(_routeID.Value);
        }

        private void LoadComboBoxData()
        {
            // Загружаем данные из таблицы городов
            var cities = _citiesAdapter.GetData();

            // Устанавливаем источники данных для ComboBox
            FromCityComboBox.ItemsSource = cities;
            ToCityComboBox.ItemsSource = cities;

            // Устанавливаем поля для отображения в ComboBox
            FromCityComboBox.DisplayMemberPath = "cityName";  // Для города отправления
            ToCityComboBox.DisplayMemberPath = "cityName";    // Для города назначения

            // Устанавливаем ID для SelectedValue
            FromCityComboBox.SelectedValuePath = "cityID"; // Устанавливаем привязку для ID
            ToCityComboBox.SelectedValuePath = "cityID";   // Устанавливаем привязку для ID

            // Опционально: устанавливаем значение по умолчанию
            FromCityComboBox.SelectedIndex = 0;  // Для города отправления (если нужно)
            ToCityComboBox.SelectedIndex = 0;    // Для города назначения (если нужно)
        }
        private void LoadRouteData(int id)
        {
            var route = _adapter.GetData().FirstOrDefault(r => r.routeID == id);
            if (route != null)
            {
                // Загружаем данные в ComboBox, используя ID
                FromCityComboBox.SelectedValue = route.fromCityID;
                ToCityComboBox.SelectedValue = route.toCityID;
                DistanceTextBox.Text = route.distanceKm.ToString();
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            // Проверяем, выбраны ли значения в ComboBox
            if (FromCityComboBox.SelectedValue == null || ToCityComboBox.SelectedValue == null)
            {
                CustomMessageBox.Show("Пожалуйста, выберите города для маршрута.", "Уведомление");
                return;
            }

            int fromCityID = Convert.ToInt32(FromCityComboBox.SelectedValue);
            int toCityID = Convert.ToInt32(ToCityComboBox.SelectedValue);
            decimal distance = 0;

            if (!decimal.TryParse(DistanceTextBox.Text, out distance))
            {
                CustomMessageBox.Show("Пожалуйста, введите корректное расстояние.", "Ошибка");
                return;
            }

            // Сохранение изменений
            try
            {
                if (_routeID.HasValue)
                {
                    // Если маршрут существует, обновляем его
                    _adapter.UpdateQuery(fromCityID, toCityID, distance, _routeID.Value);
                    CustomMessageBox.Show("Маршрут обновлен.", "Успех");
                }
                else
                {
                    // Добавление нового маршрута
                    _adapter.InsertQuery(fromCityID, toCityID, distance);
                    CustomMessageBox.Show("Маршрут добавлен.", "Успех");
                }

                NavigationService.GoBack();
            }
            catch (Exception ex)
            {
                CustomMessageBox.Show($"Ошибка при сохранении маршрута: {ex.Message}", "Ошибка", showCancel:false);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            // Навигация назад без сохранения
            NavigationService.GoBack();
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            // Восстанавливаем доступность панели на основной странице
            App.mainStackPanel.IsEnabled = true;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            // Здесь можно выполнить действия, которые должны быть выполнены при загрузке страницы
            App.mainStackPanel.IsEnabled = false;
        }
    }
}
