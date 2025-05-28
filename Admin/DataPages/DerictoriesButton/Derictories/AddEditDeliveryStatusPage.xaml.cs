using ProjectFLS.flsdbDataSetTableAdapters;
using ProjectFLS.UI;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ProjectFLS.Admin.DataPages.DerictoriesButton.Derictories
{
    public partial class AddEditDeliveryStatusPage : Page
    {
        private readonly int? _statusID;
        private readonly deliveryStatusesTableAdapter _adapter;

        public AddEditDeliveryStatusPage(int? statusID = null)
        {
            InitializeComponent();
            _statusID = statusID;
            _adapter = new deliveryStatusesTableAdapter();

            if (_statusID.HasValue)
                LoadDeliveryStatusData(_statusID.Value);
        }

        private void LoadDeliveryStatusData(int id)
        {
            var row = _adapter.GetData().FirstOrDefault(t => t.statusID == id);
            if (row != null)
            {
                StatusNameTextBox.Text = row.statusName;
            }
        }

        private void UpdateDeliveryStatusRecord(int statusID, string name)
        {
            var data = _adapter.GetData();
            var row = data.FirstOrDefault(t => t.statusID == statusID);

            if (row != null)
            {
                row.statusName = name;
                _adapter.Update(row); // Save the changes back to the database
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            // Clear the background of the field
            StatusNameTextBox.ClearValue(TextBox.BackgroundProperty);

            bool hasError = false;
            var redBrush = new SolidColorBrush(Color.FromRgb(255, 200, 200));

            // Check if the status name is empty
            if (string.IsNullOrWhiteSpace(StatusNameTextBox.Text))
            {
                StatusNameTextBox.Background = redBrush;
                hasError = true;
            }

            if (hasError)
            {
                CustomMessageBox.Show("Пожалуйста, заполните все поля корректно.", "Ошибка", showCancel: false);
                return;
            }

            try
            {
                // If the status ID is present, update the existing record
                if (_statusID.HasValue)
                {
                    UpdateDeliveryStatusRecord(_statusID.Value, StatusNameTextBox.Text);
                    CustomMessageBox.Show("Статус доставки обновлён.", "Успешно", showCancel: false);
                }
                else
                {
                    // Add a new delivery status
                    _adapter.Insert(StatusNameTextBox.Text);
                    CustomMessageBox.Show("Статус доставки добавлен.", "Успешно", showCancel: false);
                }

                // Update the list of delivery statuses
                NavigationService.GoBack();
            }
            catch (Exception ex)
            {
                CustomMessageBox.Show($"Ошибка при сохранении статуса доставки: {ex.Message}", "Ошибка", showCancel: false);
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
