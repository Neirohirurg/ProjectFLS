using ProjectFLS.flsdbDataSetTableAdapters;
using ProjectFLS.UI;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ProjectFLS.Admin.DataPages.DerictoriesButton.Derictories
{
    public partial class AddEditPartnerPage : Page
    {
        private readonly int? _dealerID;
        private readonly dealersTableAdapter _dealersAdapter;
        private readonly citiesTableAdapter _citiesAdapter;
        private readonly usersTableAdapter _usersAdapter;

        public AddEditPartnerPage(int? dealerID = null)
        {
            InitializeComponent();
            _dealerID = dealerID;
            _dealersAdapter = new dealersTableAdapter();
            _citiesAdapter = new citiesTableAdapter();
            _usersAdapter = new usersTableAdapter();

            if (_dealerID.HasValue)
                LoadPartnerData(_dealerID.Value);
            else
                LoadComboBoxData();
        }

        private void LoadPartnerData(int id)
        {
            var row = _dealersAdapter.GetData().FirstOrDefault(t => t.dealerID == id);
            if (row != null)
            {
                PartnerNameTextBox.Text = row.dealerName;
                CityComboBox.SelectedValue = row.cityID;

                // Загрузим менеджера
                ManagerComboBox.SelectedValue = row.managerID;
            }
        }

        private void LoadComboBoxData()
        {
            // Загружаем города в ComboBox
            CityComboBox.ItemsSource = _citiesAdapter.GetData();

            // Загружаем только менеджеров (пользователей с ролью 2)
            var managers = _usersAdapter.GetData().Where(u => u.roleID == 2).ToList();

            // Настроим отображение имен менеджеров в ComboBox
            ManagerComboBox.ItemsSource = managers;
            ManagerComboBox.DisplayMemberPath = "surname";
        }

        private void UpdatePartnerRecord(int dealerID, string dealerName, int cityID, int? managerID)
        {
            var data = _dealersAdapter.GetData();
            var row = data.FirstOrDefault(t => t.dealerID == dealerID);

            if (row != null)
            {
                row.dealerName = dealerName;
                row.cityID = cityID;
                row.managerID = (int)managerID; // managerID теперь может быть null
                _dealersAdapter.Update(row); // Сохранение изменений в базу данных
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            // Очистим фон полей
            PartnerNameTextBox.ClearValue(TextBox.BackgroundProperty);
            CityComboBox.ClearValue(ComboBox.BackgroundProperty);

            bool hasError = false;
            var redBrush = new SolidColorBrush(Color.FromRgb(255, 200, 200));

            // Проверка на пустое название партнера
            if (string.IsNullOrWhiteSpace(PartnerNameTextBox.Text))
            {
                PartnerNameTextBox.Background = redBrush;
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

                // Если dealerID существует, обновляем партнера
                if (_dealerID.HasValue)
                {
                    UpdatePartnerRecord(_dealerID.Value, PartnerNameTextBox.Text, (int)CityComboBox.SelectedValue, managerID);
                    CustomMessageBox.Show("Партнер обновлён.", "Успешно", showCancel: false);
                }
                else
                {
                    // Добавляем нового партнера
                    _dealersAdapter.Insert(PartnerNameTextBox.Text, (int)CityComboBox.SelectedValue, managerID);
                    CustomMessageBox.Show("Партнер добавлен.", "Успешно", showCancel: false);
                }

                // Обновляем список партнеров и возвращаемся
                NavigationService.GoBack();
            }
            catch (Exception ex)
            {
                CustomMessageBox.Show($"Ошибка при сохранении партнера: {ex.Message}", "Ошибка", showCancel: false);
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
