using ProjectFLS.flsdbDataSetTableAdapters;
using ProjectFLS.UI;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ProjectFLS.Admin.DataPages.DerictoriesButton.Derictories
{
    public partial class AddEditPartPage : Page
    {
        private readonly int? _partID;
        private readonly partsTableAdapter _adapter;

        public AddEditPartPage(int? partID = null)
        {
            InitializeComponent();
            _partID = partID;
            _adapter = new partsTableAdapter();

            if (_partID.HasValue)
                LoadPartData(_partID.Value);
        }

        private void LoadPartData(int id)
        {
            var row = _adapter.GetData().FirstOrDefault(t => t.partID == id);
            if (row != null)
            {
                PartNameTextBox.Text = row.partName;
                LengthTextBox.Text = row.lengthM.ToString();
                WidthTextBox.Text = row.widthM.ToString();
                HeightTextBox.Text = row.heightM.ToString();
            }
        }

        private void UpdatePartRecord(int partID, string name, double length, double width, double height)
        {
            var data = _adapter.GetData();
            var row = data.FirstOrDefault(t => t.partID == partID);

            if (row != null)
            {
                row.partName = name;
                row.lengthM = (Decimal)length;
                row.widthM = (Decimal)width;
                row.heightM = (Decimal)height;
                _adapter.Update(row); // Save the changes back to the database
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            // Clear the background of the fields
            PartNameTextBox.ClearValue(TextBox.BackgroundProperty);
            LengthTextBox.ClearValue(TextBox.BackgroundProperty);
            WidthTextBox.ClearValue(TextBox.BackgroundProperty);
            HeightTextBox.ClearValue(TextBox.BackgroundProperty);

            bool hasError = false;
            var redBrush = new SolidColorBrush(Color.FromRgb(255, 200, 200));

            // Validate the fields
            if (string.IsNullOrWhiteSpace(PartNameTextBox.Text))
            {
                PartNameTextBox.Background = redBrush;
                hasError = true;
            }

            if (!double.TryParse(LengthTextBox.Text, out double length) || length <= 0)
            {
                LengthTextBox.Background = redBrush;
                hasError = true;
            }

            if (!double.TryParse(WidthTextBox.Text, out double width) || width <= 0)
            {
                WidthTextBox.Background = redBrush;
                hasError = true;
            }

            if (!double.TryParse(HeightTextBox.Text, out double height) || height <= 0)
            {
                HeightTextBox.Background = redBrush;
                hasError = true;
            }

            if (hasError)
            {
                CustomMessageBox.Show("Пожалуйста, заполните все поля корректно.", "Ошибка", showCancel: false);
                return;
            }

            try
            {
                // If part ID exists, update the existing record
                if (_partID.HasValue)
                {
                    UpdatePartRecord(_partID.Value, PartNameTextBox.Text, length, width, height);
                    CustomMessageBox.Show("Запчасть обновлена.", "Успешно", showCancel: false);
                }
                else
                {
                    // Add a new part
                    _adapter.Insert(PartNameTextBox.Text, (Decimal)length, (Decimal)width, (Decimal)height);
                    CustomMessageBox.Show("Запчасть добавлена.", "Успешно", showCancel: false);
                }

                // Navigate back after saving
                NavigationService.GoBack();
            }
            catch (Exception ex)
            {
                CustomMessageBox.Show($"Ошибка при сохранении запчасти: {ex.Message}", "Ошибка", showCancel: false);
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
