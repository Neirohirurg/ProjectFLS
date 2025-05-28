using ProjectFLS.flsdbDataSetTableAdapters;
using ProjectFLS.UI;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ProjectFLS.Admin.DataPages.DerictoriesButton.Derictories
{
    public partial class AddEditTransportPage : Page
    {
        private readonly int? _transportID;
        private readonly transportTableAdapter _adapter;
        private readonly transportTypesTableAdapter _transportTypeAdapter;
        private readonly fuelTypesTableAdapter _fuelTypeAdapter;

        private flsdbDataSet.transportTypesDataTable _transportTypes;
        private flsdbDataSet.fuelTypesDataTable _fuelTypes;

        public AddEditTransportPage(int? transportID = null)
        {
            InitializeComponent();
            _transportID = transportID;
            _adapter = new transportTableAdapter();
            _transportTypeAdapter = new transportTypesTableAdapter();
            _fuelTypeAdapter = new fuelTypesTableAdapter();

            LoadComboBoxData();

            if (_transportID.HasValue)
                LoadTransportData(_transportID.Value);
        }

        private void LoadComboBoxData()
        {
            _transportTypes = _transportTypeAdapter.GetData();
            _fuelTypes = _fuelTypeAdapter.GetData();

            // Устанавливаем источники данных для ComboBox
            TransportTypeComboBox.ItemsSource = _transportTypes;
            FuelTypeComboBox.ItemsSource = _fuelTypes;

            // Устанавливаем поля для отображения в ComboBox
            TransportTypeComboBox.DisplayMemberPath = "transportTypeName";  // Для типа транспорта
            FuelTypeComboBox.DisplayMemberPath = "fuelName";                // Для типа топлива
        }

        private void LoadTransportData(int id)
        {
            var row = _adapter.GetData().FirstOrDefault(t => t.transportID == id);
            if (row != null)
            {
                TransportNameTextBox.Text = row.transportName;
                TransportTypeComboBox.SelectedValue = row.transportTypeID;  // Правильно выбираем по ID типа транспорта
                FuelTypeComboBox.SelectedValue = row.fuelTypeID;            // Правильно выбираем по ID типа топлива
                MaxLengthTextBox.Text = row.maxLengthM.ToString();
                MaxWidthTextBox.Text = row.maxWidthM.ToString();
                MaxHeightTextBox.Text = row.maxHeightM.ToString();
                ConsumptionTextBox.Text = row.consumptionLPer100Km.ToString();
            }
        }

        private void UpdateTransportRecord(int transportID, string name, int transportTypeID, int fuelTypeID,
                                   double maxLength, double maxWidth, double maxHeight, double consumption)
        {
            var data = _adapter.GetData();
            var row = data.FirstOrDefault(t => t.transportID == transportID);

            if (row != null)
            {
                row.transportName = name;
                row.transportTypeID = transportTypeID;
                row.fuelTypeID = fuelTypeID;
                row.maxLengthM = (Decimal)maxLength;
                row.maxWidthM = (Decimal)maxWidth;
                row.maxHeightM = (Decimal)maxHeight;
                row.consumptionLPer100Km = (Decimal)consumption;

                _adapter.Update(row); // Это сохранит изменения обратно в базу данных
            }
        }

private void Save_Click(object sender, RoutedEventArgs e)
{
    // Очистим фон полей
    TransportNameTextBox.ClearValue(TextBox.BackgroundProperty);
    TransportTypeComboBox.ClearValue(ComboBox.BackgroundProperty);
    FuelTypeComboBox.ClearValue(ComboBox.BackgroundProperty);
    MaxLengthTextBox.ClearValue(TextBox.BackgroundProperty);
    MaxWidthTextBox.ClearValue(TextBox.BackgroundProperty);
    MaxHeightTextBox.ClearValue(TextBox.BackgroundProperty);
    ConsumptionTextBox.ClearValue(TextBox.BackgroundProperty);

    bool hasError = false;
    var redBrush = new SolidColorBrush(Color.FromRgb(255, 200, 200));

    // Регулярное выражение для проверки, если нужно
    var regex = new System.Text.RegularExpressions.Regex(@"^[А-Яа-яЁёA-Za-z\- ]+$");

    // Проверка названия транспорта
    if (string.IsNullOrWhiteSpace(TransportNameTextBox.Text) || !regex.IsMatch(TransportNameTextBox.Text))
    {
        TransportNameTextBox.Background = redBrush;
        hasError = true;
    }

    // Проверка типа транспорта
    if (TransportTypeComboBox.SelectedItem == null)
    {
        TransportTypeComboBox.Background = redBrush;
        hasError = true;
    }

    // Проверка типа топлива
    if (FuelTypeComboBox.SelectedItem == null)
    {
        FuelTypeComboBox.Background = redBrush;
        hasError = true;
    }

    // Проверка максимальных размеров
    if (string.IsNullOrWhiteSpace(MaxLengthTextBox.Text) || !double.TryParse(MaxLengthTextBox.Text, out _))
    {
        MaxLengthTextBox.Background = redBrush;
        hasError = true;
    }
    if (string.IsNullOrWhiteSpace(MaxWidthTextBox.Text) || !double.TryParse(MaxWidthTextBox.Text, out _))
    {
        MaxWidthTextBox.Background = redBrush;
        hasError = true;
    }
    if (string.IsNullOrWhiteSpace(MaxHeightTextBox.Text) || !double.TryParse(MaxHeightTextBox.Text, out _))
    {
        MaxHeightTextBox.Background = redBrush;
        hasError = true;
    }

    // Проверка расхода топлива
    if (string.IsNullOrWhiteSpace(ConsumptionTextBox.Text) || !double.TryParse(ConsumptionTextBox.Text, out _))
    {
        ConsumptionTextBox.Background = redBrush;
        hasError = true;
    }

    if (hasError)
    {
        CustomMessageBox.Show("Пожалуйста, заполните все поля корректно.", "Ошибка", showCancel: false);
        return;
    }

    try
    {
        // Если есть ID транспорта, значит редактируем существующий
        if (_transportID.HasValue)
        {
            UpdateTransportRecord(
                _transportID.Value,
                TransportNameTextBox.Text,
                (int)TransportTypeComboBox.SelectedValue,
                (int)FuelTypeComboBox.SelectedValue,
                Convert.ToDouble(MaxLengthTextBox.Text),
                Convert.ToDouble(MaxWidthTextBox.Text),
                Convert.ToDouble(MaxHeightTextBox.Text),
                Convert.ToDouble(ConsumptionTextBox.Text)
            );
            CustomMessageBox.Show("Транспорт обновлён.", "Успешно", showCancel: false);
        }
        else
        {
            // Вставляем новый транспорт
            _adapter.Insert(
                TransportNameTextBox.Text,
                (int)TransportTypeComboBox.SelectedValue,
                (int)FuelTypeComboBox.SelectedValue,
                (Decimal)Convert.ToDouble(MaxLengthTextBox.Text),
                (Decimal)Convert.ToDouble(MaxWidthTextBox.Text),
                (Decimal)Convert.ToDouble(MaxHeightTextBox.Text),
                (Decimal)Convert.ToDouble(ConsumptionTextBox.Text)
            );
            CustomMessageBox.Show("Транспорт добавлен.", "Успешно", showCancel: false);
        }

        // Обновление списка транспорта
        NavigationService.GoBack();
    }
    catch (Exception ex)
    {
        CustomMessageBox.Show($"Ошибка при сохранении транспорта: {ex.Message}", "Ошибка", showCancel: false);
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
