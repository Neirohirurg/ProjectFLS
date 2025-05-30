using ProjectFLS.Models;
using ProjectFLS.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;
using System.Data.SqlClient;
using ProjectFLS.flsdbDataSetTableAdapters;

namespace ProjectFLS.Manager.DataPages
{
    public partial class PartnersPage : Page, ISearchable
    {
        private Frame _managerMainFrame;
        private Border _mainBorder;
        private List<Partner> _partnersList;
        private citiesTableAdapter _citiesAdapter;
        private usersTableAdapter _usersAdapter;

        public PartnersPage(Frame managerMainFrame)
        {
            InitializeComponent();
            _managerMainFrame = managerMainFrame;
            _mainBorder = App.mainStackPanelBorder;
            _partnersList = new List<Partner>();

            _citiesAdapter = new citiesTableAdapter();
            _usersAdapter = new usersTableAdapter();
        }

        public void PerformSearch(string query)
        {
            query = query?.ToLowerInvariant() ?? "";
            var filteredPartners = _partnersList
                .Where(p => p.DealerName.ToLower().Contains(query) ||
                            p.CityName.ToLower().Contains(query) ||
                            p.ManagerName.Contains(query))
                .ToList();

            PartnersListView.ItemsSource = filteredPartners;
        }

        public void EnableSearch() { }

        public void RefreshPartners()
        {
            _partnersList.Clear();

                // Используем адаптеры для получения данных
                try
                {
                    // Получаем данные через адаптеры
                    var dealersData = new dealersTableAdapter().GetData(); // Замените на ваш адаптер для партнеров

                    if (dealersData == null || dealersData.Count == 0)
                    {
                        MessageBox.Show("Нет данных о партнерах.");
                        return;
                    }

                    foreach (var dealer in dealersData)
                    {
                        var partner = new Partner
                        {
                            DealerID = dealer.dealerID,
                            DealerName = dealer.dealerName,
                            CityName = GetCityName(dealer.cityID),
                            ManagerName = GetManagerName(dealer.managerID)
                        };

                        _partnersList.Add(partner);
                    }

                    PartnersListView.ItemsSource = _partnersList;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при получении данных: {ex.Message}");
                }
        }

        private string GetCityName(int cityID)
        {
            try
            {
                // Получаем список городов через адаптер
                var cityData = _citiesAdapter.GetData();

                // Проверка на наличие данных
                var city = cityData.FirstOrDefault(c => c.cityID == cityID);
                return city?.cityName ?? "Неизвестный город";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при получении имени города: {ex.Message}");
                return "Ошибка при загрузке города";
            }
        }

        private string GetManagerName(int managerID)
        {
            try
            {
                // Получаем список пользователей через адаптер
                var usersData = _usersAdapter.GetData();

                // Проверка на наличие данных
                var manager = usersData.FirstOrDefault(u => u.userID == managerID);
                return manager?.surname ?? "Неизвестный менеджер";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при получении имени менеджера: {ex.Message}");
                return "Ошибка при загрузке менеджера";
            }
        }

        public void DeleteSelectedPartner()
        {
            if (PartnersListView.SelectedItem is Partner selectedPartner)
            {
                _partnersList.Remove(selectedPartner);
                PartnersListView.ItemsSource = null;
                PartnersListView.ItemsSource = _partnersList;
            }
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            App.mainStackPanel.IsEnabled = false;
            _mainBorder.Visibility = Visibility.Visible;
            RefreshPartners();
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            _mainBorder.Visibility = Visibility.Collapsed;
        }

        private void Sort_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button == null) return;

            string sortBy = button.Tag.ToString();

            switch (sortBy)
            {
                case "DealerID":
                    _partnersList = _partnersList.OrderBy(p => p.DealerID).ToList();
                    break;
                case "DealerName":
                    _partnersList = _partnersList.OrderBy(p => p.DealerName).ToList();
                    break;
                case "CityName":
                    _partnersList = _partnersList.OrderBy(p => p.CityName).ToList();
                    break;
                case "ManagerName":
                    _partnersList = _partnersList.OrderBy(p => p.ManagerName).ToList();
                    break;
            }

            PartnersListView.ItemsSource = null;
            PartnersListView.ItemsSource = _partnersList;
        }


        private void PartnersListView_Selected(object sender, RoutedEventArgs e)
        {
            if (PartnersListView.SelectedItem is Partner selectedPartner)
            {
                // Если элемент выбран, выполняем необходимые действия
                App._selectedPartner = selectedPartner;
                App.mainStackPanel.IsEnabled = true; // Включаем панель
                                                     // Можешь выполнить дополнительные действия с selectedPartner, например, отображение данных.
            }
            else
            {
                // Если ничего не выбрано
                App.mainStackPanel.IsEnabled = false;
            }
        }

        private void PartnersListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (PartnersListView.SelectedItem is Partner selectedPartner)
            {
                // Если элемент выбран, выполняем необходимые действия
                App._selectedPartner = selectedPartner;

                _managerMainFrame.Navigate(new AddEditDealerPage(_managerMainFrame));
            }
            else
            {
                // Если ничего не выбрано
                App.mainStackPanel.IsEnabled = false;
            }
        }
    }
}
