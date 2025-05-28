using ProjectFLS.flsdbDataSetTableAdapters;
using ProjectFLS.UI;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ProjectFLS.Admin.DataPages.DerictoriesButton.Derictories
{
    public partial class AddEditTransportTypePage : Page
    {
        private readonly int? _transportTypeID;
        private readonly transportTypesTableAdapter _adapter;

        public AddEditTransportTypePage(int? transportTypeID = null)
        {
            InitializeComponent();
            _transportTypeID = transportTypeID;
            _adapter = new transportTypesTableAdapter();

            if (_transportTypeID.HasValue)
                LoadTransportTypeData(_transportTypeID.Value);
        }

        private void LoadTransportTypeData(int id)
        {
            var row = _adapter.GetData().FirstOrDefault(t => t.transportTypeID == id);
            if (row != null)
            {
                TransportTypeNameTextBox.Text = row.transportTypeName;
            }
        }

        private void UpdateTransportTypeRecord(int transportTypeID, string name)
        {
            var data = _adapter.GetData();
            var row = data.FirstOrDefault(t => t.transportTypeID == transportTypeID);

            if (row != null)
            {
                row.transportTypeName = name;
                _adapter.Update(row); // Это сохранит изменения обратно в базу данных
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            // Очистим фон поля
            TransportTypeNameTextBox.ClearValue(TextBox.BackgroundProperty);

            bool hasError = false;
            var redBrush = new SolidColorBrush(Color.FromRgb(255, 200, 200));

            // Проверка на пустое название типа транспорта
            if (string.IsNullOrWhiteSpace(TransportTypeNameTextBox.Text))
            {
                TransportTypeNameTextBox.Background = redBrush;
                hasError = true;
            }

            if (hasError)
            {
                CustomMessageBox.Show("Пожалуйста, заполните все поля корректно.", "Ошибка", showCancel: false);
                return;
            }

            try
            {
                // Если есть ID типа транспорта, то обновляем существующий
                if (_transportTypeID.HasValue)
                {
                    UpdateTransportTypeRecord(_transportTypeID.Value, TransportTypeNameTextBox.Text);
                    CustomMessageBox.Show("Тип транспорта обновлён.", "Успешно", showCancel: false);
                }
                else
                {
                    // Добавляем новый тип транспорта
                    _adapter.Insert(TransportTypeNameTextBox.Text);
                    CustomMessageBox.Show("Тип транспорта добавлен.", "Успешно", showCancel: false);
                }

                // Обновляем список типов транспорта
                NavigationService.GoBack();
            }
            catch (Exception ex)
            {
                CustomMessageBox.Show($"Ошибка при сохранении типа транспорта: {ex.Message}", "Ошибка", showCancel: false);
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
