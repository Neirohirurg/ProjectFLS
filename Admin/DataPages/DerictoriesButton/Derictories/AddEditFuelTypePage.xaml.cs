using ProjectFLS.flsdbDataSetTableAdapters;
using ProjectFLS.UI;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ProjectFLS.Admin.DataPages.DerictoriesButton.Derictories
{
    public partial class AddEditFuelTypePage : Page
    {
        private readonly int? _fuelTypeID;
        private readonly fuelTypesTableAdapter _adapter;

        public AddEditFuelTypePage(int? fuelTypeID = null)
        {
            InitializeComponent();
            _fuelTypeID = fuelTypeID;
            _adapter = new fuelTypesTableAdapter();

            if (_fuelTypeID.HasValue)
                LoadFuelTypeData(_fuelTypeID.Value);
        }

        private void LoadFuelTypeData(int id)
        {
            var row = _adapter.GetData().FirstOrDefault(t => t.fuelTypeID == id);
            if (row != null)
            {
                FuelTypeNameTextBox.Text = row.fuelName;
            }
        }

        private void UpdateFuelTypeRecord(int fuelTypeID, string name)
        {
            var data = _adapter.GetData();
            var row = data.FirstOrDefault(t => t.fuelTypeID == fuelTypeID);

            if (row != null)
            {
                row.fuelName = name;
                _adapter.Update(row); // Сохранение изменений в базу данных
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            // Очистим фон поля
            FuelTypeNameTextBox.ClearValue(TextBox.BackgroundProperty);

            bool hasError = false;
            var redBrush = new SolidColorBrush(Color.FromRgb(255, 200, 200));

            // Проверка на пустое название типа топлива
            if (string.IsNullOrWhiteSpace(FuelTypeNameTextBox.Text))
            {
                FuelTypeNameTextBox.Background = redBrush;
                hasError = true;
            }

            if (hasError)
            {
                CustomMessageBox.Show("Пожалуйста, заполните все поля корректно.", "Ошибка", showCancel: false);
                return;
            }

            try
            {
                // Если есть ID типа топлива, то обновляем существующий
                if (_fuelTypeID.HasValue)
                {
                    UpdateFuelTypeRecord(_fuelTypeID.Value, FuelTypeNameTextBox.Text);
                    CustomMessageBox.Show("Тип топлива обновлён.", "Успешно", showCancel: false);
                }
                else
                {
                    // Добавляем новый тип топлива
                    _adapter.Insert(FuelTypeNameTextBox.Text);
                    CustomMessageBox.Show("Тип топлива добавлен.", "Успешно", showCancel: false);
                }

                // Обновляем список типов топлива
                NavigationService.GoBack();
            }
            catch (Exception ex)
            {
                CustomMessageBox.Show($"Ошибка при сохранении типа топлива: {ex.Message}", "Ошибка", showCancel: false);
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
