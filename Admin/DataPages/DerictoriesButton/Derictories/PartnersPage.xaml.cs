using ProjectFLS.flsdbDataSetTableAdapters;
using ProjectFLS.Interfaces;
using ProjectFLS.Models;
using ProjectFLS.UI;
using System;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ProjectFLS.Admin.DataPages.DerictoriesButton.Derictories
{
    public partial class PartnersPage : Page, ISearchable, INavigationPanelHost
    {
        private dealersTableAdapter _dealersAdapter;
        private usersTableAdapter _usersAdapter;
        private citiesTableAdapter _citiesAdapter;
        private StackPanel _stackpanel;
        private Border _stackpanelBorder;
        private string _currentSortField = null;
        private bool _sortAscending = true;

        public PartnersPage()
        {
            InitializeComponent();
            _dealersAdapter = new dealersTableAdapter();
            _usersAdapter = new usersTableAdapter();
            _citiesAdapter = new citiesTableAdapter();
            _stackpanel = App.mainStackPanel;
            _stackpanelBorder = App.mainStackPanelBorder;
        }

        public void SetupNavigationPanel(StackPanel panel, Border border)
        {
            panel.Children.Clear();

            var addLabel = new Label { Content = "Добавить", Style = (Style)Application.Current.FindResource("menuLabel"), Margin = new Thickness(5), Cursor = Cursors.Hand };
            addLabel.MouseLeftButtonUp += (s, e) =>
            {
                _stackpanel.IsEnabled = false;
                NavigationService?.Navigate(new AddEditPartnerPage());
            };

            var editLabel = new Label { Content = "Редактировать", Style = (Style)Application.Current.FindResource("menuLabel"), Margin = new Thickness(5), Cursor = Cursors.Hand };
            editLabel.MouseLeftButtonUp += (s, e) =>
            {
                try
                {
                    if (PartnersListView.SelectedItem is Partner selectedDealer)
                    {
                        
                        _stackpanel.IsEnabled = false;
                        NavigationService?.Navigate(new AddEditPartnerPage(selectedDealer.DealerID));
                        
                    }
                }
                catch (Exception ex)
                {
                    // Логирование ошибки или отображение сообщения
                    CustomMessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", showCancel: false);
                }

            };

            var deleteLabel = new Label { Content = "Удалить", Style = (Style)Application.Current.FindResource("menuLabel"), Margin = new Thickness(5), Cursor = Cursors.Hand };
#pragma warning disable CS4014 // Так как этот вызов не ожидается, выполнение существующего метода продолжается до тех пор, пока вызов не будет завершен
            deleteLabel.MouseLeftButtonUp += (s, e) => DeleteSelectedAsync();
#pragma warning restore CS4014 // Так как этот вызов не ожидается, выполнение существующего метода продолжается до тех пор, пока вызов не будет завершен

            panel.Children.Add(addLabel);
            panel.Children.Add(editLabel);
            panel.Children.Add(deleteLabel);
        }

        public void PerformSearch(string query)
        {
            var all = _dealersAdapter.GetData();
            query = query?.ToLowerInvariant() ?? "";

            // Получаем пользователей и города только один раз
            var users = _usersAdapter.GetData();
            var cities = _citiesAdapter.GetData();

            // Фильтруем по всем полям
            var filtered = all.Where(d =>
                (d.dealerName != null && d.dealerName.ToLower().Contains(query)) ||  // Поиск по имени
                (cities.FirstOrDefault(c => c.cityID == d.cityID)?.cityName.ToLower().Contains(query) ?? false) ||  // Поиск по городу
                (users.FirstOrDefault(u => u.userID == d.managerID)?.surname.ToLower().Contains(query) ?? false) // Поиск по фамилии менеджера
            ).ToList();

            // Преобразуем в нужный формат
            var partnersList = filtered.Select(dealer =>
            {
                // Ищем город и менеджера для каждого партнера
                var city = cities.FirstOrDefault(c => c.cityID == dealer.cityID);
                var manager = users.FirstOrDefault(u => u.userID == dealer.managerID);

                return new Partner
                {
                    DealerID = dealer.dealerID,
                    DealerName = dealer.dealerName,
                    CityName = city?.cityName ?? "Неизвестный город", // Если город не найден, выводим "Неизвестный город"
                    ManagerName = manager != null ? $"{manager.surname}" : "Неизвестный менеджер", // Если менеджер не найден, выводим "Неизвестный менеджер"
                };
            }).ToList();

            PartnersListView.ItemsSource = partnersList; // Устанавливаем источник данных
        }


        public void EnableSearch() { }

        private void Sort_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is string field)
            {
                if (_currentSortField == field)
                    _sortAscending = !_sortAscending;
                else
                {
                    _currentSortField = field;
                    _sortAscending = true;
                }

                ApplySorting();
            }
        }

        private void ApplySorting()
        {
            var data = PartnersListView.ItemsSource.Cast<Partner>().ToList();  // Преобразуем ItemsSource в список warehousesRow

            var sortedData = _sortAscending
                ? data.OrderBy(row => GetFieldValue(row, _currentSortField)).ToList()
                : data.OrderByDescending(row => GetFieldValue(row, _currentSortField)).ToList();

            PartnersListView.ItemsSource = sortedData;  // Обновляем ItemsSource с отсортированными данными
        }
        private object GetPropertyValue(Partner partner, string field)
        {
            switch (field)
            {
                case "DealerID":
                    return partner.DealerID;
                case "DealerName":
                    return partner.DealerName;
                case "CityName":
                    return partner.CityName;
                case "ManagerName":
                    return partner.ManagerName;
                default:
                    return null;
            }
        }

        private object GetFieldValue(dynamic item, string field)
        {
            return item.GetType().GetProperty(field)?.GetValue(item, null);
        }

        public async Task DeleteSelectedAsync()
        {
            if (PartnersListView.SelectedItem is Partner selectedDealer)
            {
                int dealerID = selectedDealer.DealerID;
                bool? result = CustomMessageBox.Show($"Удалить партнера с ID = {dealerID}?", "Подтверждение удаления", showCancel: true);
                if (result == true)
                {
                    try
                    {
                        await Task.Run(() => _dealersAdapter.DeleteQuery(dealerID));
                        await RefreshAsync();
                        CustomMessageBox.Show("Успешно удалено.", "Удаление", showCancel: false);
                    }
                    catch (Exception ex)
                    {
                        CustomMessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", showCancel: false);
                    }
                }
            }
        }

        public async Task RefreshAsync()
        {
            var list = await Task.Run(() => _dealersAdapter.GetData());
            var users = _usersAdapter.GetData();
            var cities = _citiesAdapter.GetData();

            var partnersList = list.Select(dealer =>
            {
                var city = cities.First(c => c.cityID == dealer.cityID);
                var manager = users.First(u => u.userID == dealer.managerID);

                return new Partner
                {
                    DealerID = dealer.dealerID,
                    DealerName = dealer.dealerName,
                    CityName = city.cityName,
                    ManagerName = $"{manager.surname}",
                };
            }).ToList();

            PartnersListView.ItemsSource = partnersList;  // Обновляем ItemsSource с новым списком
        }



        private void PartnersListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (PartnersListView.SelectedItem is Partner selectedPartner)
            {
                _stackpanelBorder.Visibility = Visibility.Visible;
                _stackpanel.IsEnabled = false;
                NavigationService?.Navigate(new AddEditPartnerPage(selectedPartner.DealerID));
            }
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            _stackpanelBorder.Visibility = Visibility.Visible;
            await RefreshAsync();
        }
    }
}
