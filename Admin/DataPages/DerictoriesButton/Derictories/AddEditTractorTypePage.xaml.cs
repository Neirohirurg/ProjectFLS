using ProjectFLS.flsdbDataSetTableAdapters;
using ProjectFLS.UI;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ProjectFLS.Admin.DataPages.DerictoriesButton.Derictories
{
    public partial class AddEditTractorTypePage : Page
    {
        private readonly int? _tractorID;
        private readonly tractorUnitsTableAdapter _adapter;

        public AddEditTractorTypePage(int? tractorID = null)
        {
            InitializeComponent();
            _tractorID = tractorID;
            _adapter = new tractorUnitsTableAdapter();

            if (_tractorID.HasValue)
                LoadTractorData(_tractorID.Value);
        }

        private void LoadTractorData(int id)
        {
            var row = _adapter.GetData().FirstOrDefault(t => t.tractorID == id);
            if (row != null)
            {
                ModelNameTextBox.Text = row.model;
                TractorLengthTextBox.Text = row.lengthM.ToString();
                TractorWidthTextBox.Text = row.widthM.ToString();
                TractorHeightTextBox.Text = row.heightM.ToString();
                TractorWeightTextBox.Text = row.weightKg.ToString();
                TractorEnginePowerTextBox.Text = row.enginePowerHP.ToString();
            }
        }

        private void UpdateTractorRecord(int tractorID, string model, double length, double width, double height, double weight, double enginePower)
        {
            var data = _adapter.GetData();
            var row = data.FirstOrDefault(t => t.tractorID == tractorID);

            if (row != null)
            {
                row.model = model;
                row.lengthM = (Decimal)length;
                row.widthM = (Decimal)width;
                row.heightM = (Decimal)height;
                row.weightKg = (Decimal)weight;
                row.enginePowerHP = (int)enginePower;
                _adapter.Update(row); // Save the changes back to the database
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            // Clear the background of the fields
            ModelNameTextBox.ClearValue(TextBox.BackgroundProperty);
            TractorLengthTextBox.ClearValue(TextBox.BackgroundProperty);
            TractorWidthTextBox.ClearValue(TextBox.BackgroundProperty);
            TractorHeightTextBox.ClearValue(TextBox.BackgroundProperty);
            TractorWeightTextBox.ClearValue(TextBox.BackgroundProperty);
            TractorEnginePowerTextBox.ClearValue(TextBox.BackgroundProperty);

            bool hasError = false;
            var redBrush = new SolidColorBrush(Color.FromRgb(255, 200, 200));

            // Validate the fields
            if (string.IsNullOrWhiteSpace(ModelNameTextBox.Text))
            {
                ModelNameTextBox.Background = redBrush;
                hasError = true;
            }

            if (!double.TryParse(TractorLengthTextBox.Text, out double length) || length <= 0)
            {
                TractorLengthTextBox.Background = redBrush;
                hasError = true;
            }

            if (!double.TryParse(TractorWidthTextBox.Text, out double width) || width <= 0)
            {
                TractorWidthTextBox.Background = redBrush;
                hasError = true;
            }

            if (!double.TryParse(TractorHeightTextBox.Text, out double height) || height <= 0)
            {
                TractorHeightTextBox.Background = redBrush;
                hasError = true;
            }

            if (!double.TryParse(TractorWeightTextBox.Text, out double weight) || weight <= 0)
            {
                TractorWeightTextBox.Background = redBrush;
                hasError = true;
            }

            if (!double.TryParse(TractorEnginePowerTextBox.Text, out double enginePower) || enginePower <= 0)
            {
                TractorEnginePowerTextBox.Background = redBrush;
                hasError = true;
            }

            if (hasError)
            {
                CustomMessageBox.Show("Пожалуйста, заполните все поля корректно.", "Ошибка", showCancel: false);
                return;
            }

            try
            {
                // If tractor ID exists, update the existing record
                if (_tractorID.HasValue)
                {
                    UpdateTractorRecord(_tractorID.Value, ModelNameTextBox.Text, length, width, height, weight, enginePower);
                    CustomMessageBox.Show("Тип трактора обновлен.", "Успешно", showCancel: false);
                }
                else
                {
                    // Add a new tractor type
                    _adapter.Insert(ModelNameTextBox.Text, (Decimal)length, (Decimal)width, (Decimal)height, (Decimal)weight, (int)enginePower);
                    CustomMessageBox.Show("Тип трактора добавлен.", "Успешно", showCancel: false);
                }

                // Navigate back after saving
                NavigationService.GoBack();
            }
            catch (Exception ex)
            {
                CustomMessageBox.Show($"Ошибка при сохранении типа трактора: {ex.Message}", "Ошибка", showCancel: false);
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
