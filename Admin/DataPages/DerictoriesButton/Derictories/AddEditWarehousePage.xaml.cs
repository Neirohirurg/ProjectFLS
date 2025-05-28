using ProjectFLS.flsdbDataSetTableAdapters;
using ProjectFLS.UI;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ProjectFLS.Admin.DataPages.DerictoriesButton.Derictories
{
    public partial class AddEditWarehousePage : Page
    {
        private readonly int? _warehouseID;
        private readonly warehousesTableAdapter _adapter;
        private readonly citiesTableAdapter _citiesAdapter;
        private readonly usersTableAdapter _usersAdapter;

        public AddEditWarehousePage(int? warehouseID = null)
        {
            InitializeComponent();
            _warehouseID = warehouseID;
            _adapter = new warehousesTableAdapter();
            _citiesAdapter = new citiesTableAdapter();
            _usersAdapter = new usersTableAdapter();

            if (_warehouseID.HasValue)
                LoadWarehouseData(_warehouseID.Value);
            else
                LoadComboBoxData();
        }

        private void LoadWarehouseData(int id)
        {
            var row = _adapter.GetData().FirstOrDefault(t => t.warehouseID == id);
            if (row != null)
            {
                WarehouseNameTextBox.Text = row.warehouseName;
                CityComboBox.SelectedValue = row.cityID;

                // Здесь используем `managerID.HasValue` и передаем `null`, если значения нет
                ManagerComboBox.SelectedValue = row.managerID;
            }
        }

        private void LoadComboBoxData()
        {
            // Очистим ComboBox перед добавлением новых элементов

            // Загружаем города
            CityComboBox.ItemsSource = _citiesAdapter.GetData();

            var managers = _usersAdapter.GetData().Where(u => u.roleID == 2).ToList();

            // Настроим отображение имен менеджеров в ComboBox
            ManagerComboBox.ItemsSource = managers;
            ManagerComboBox.DisplayMemberPath = "surname";
        }

        private void UpdateWarehouseRecord(int warehouseID, string warehouseName, int cityID, int? managerID)
        {
            var data = _adapter.GetData();
            var row = data.FirstOrDefault(t => t.warehouseID == warehouseID);

            if (row != null)
            {
                row.warehouseName = warehouseName;
                row.cityID = cityID;
                row.managerID = (int)managerID; // managerID теперь может быть null
                _adapter.Update(row); // Сохранение изменений в базу данных
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            // Очистим фон полей
            WarehouseNameTextBox.ClearValue(TextBox.BackgroundProperty);
            CityComboBox.ClearValue(ComboBox.BackgroundProperty);

            bool hasError = false;
            var redBrush = new SolidColorBrush(Color.FromRgb(255, 200, 200));

            // Проверка на пустое название склада
            if (string.IsNullOrWhiteSpace(WarehouseNameTextBox.Text))
            {
                WarehouseNameTextBox.Background = redBrush;
                hasError = true;
            }

            // Проверка на выбор города
            if (CityComboBox.SelectedValue == null)
            {
                CityComboBox.Background = redBrush;
                hasError = true;
            }

            if (hasError)
            {
                CustomMessageBox.Show("Пожалуйста, заполните все поля корректно.", "Ошибка", showCancel: false);
                return;
            }

            try
            {
                // Получаем managerID из ComboBox, проверяя, выбран ли менеджер
                int? managerID = ManagerComboBox.SelectedValue as int?;

                // Если warehouseID существует, обновляем склад
                if (_warehouseID.HasValue)
                {
                    UpdateWarehouseRecord(_warehouseID.Value, WarehouseNameTextBox.Text, (int)CityComboBox.SelectedValue, managerID);
                    CustomMessageBox.Show("Склад обновлён.", "Успешно", showCancel: false);
                }
                else
                {
                    // Добавляем новый склад
                    _adapter.Insert(WarehouseNameTextBox.Text, (int)CityComboBox.SelectedValue, managerID);
                    CustomMessageBox.Show("Склад добавлен.", "Успешно", showCancel: false);
                }

                // Обновляем список складов
                NavigationService.GoBack();
            }
            catch (Exception ex)
            {
                CustomMessageBox.Show($"Ошибка при сохранении склада: {ex.Message}", "Ошибка", showCancel: false);
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
            LoadComboBoxData();
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            App.mainStackPanel.IsEnabled = true;
        }
    }
}
