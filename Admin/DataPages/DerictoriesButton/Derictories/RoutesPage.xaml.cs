using ProjectFLS.flsdbDataSetTableAdapters;
using ProjectFLS.UI;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Collections.Generic;
using ProjectFLS.Models;
using ProjectFLS.Interfaces;

namespace ProjectFLS.Admin.DataPages.DerictoriesButton.Derictories
{
    public partial class RoutesPage : Page, ISearchable, INavigationPanelHost
    {
        private routesTableAdapter _routesAdapter;
        private citiesTableAdapter _citiesAdapter;
        private StackPanel _stackpanel;
        private Border _stackpanelBorder;
        private string _currentSortField = null;
        private bool _sortAscending = true;

        public RoutesPage()
        {
            InitializeComponent();
            _routesAdapter = new routesTableAdapter();
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
                NavigationService?.Navigate(new AddEditRoutePage());
            };

            var editLabel = new Label { Content = "Редактировать", Style = (Style)Application.Current.FindResource("menuLabel"), Margin = new Thickness(5), Cursor = Cursors.Hand };
            editLabel.MouseLeftButtonUp += (s, e) =>
            {
                if (RoutesListView.SelectedItem is Route selectedPart)
                {
                    _stackpanel.IsEnabled = false;
                    NavigationService?.Navigate(new AddEditRoutePage(selectedPart.RouteID));
                }
            };

            var deleteLabel = new Label { Content = "Удалить", Style = (Style)Application.Current.FindResource("menuLabel"), Margin = new Thickness(5), Cursor = Cursors.Hand };
            deleteLabel.MouseLeftButtonUp += (s, e) => DeleteSelectedAsync();

            panel.Children.Add(addLabel);
            panel.Children.Add(editLabel);
            panel.Children.Add(deleteLabel);
        }

        public void EnableSearch() { }
        // Поиск маршрутов
        public void PerformSearch(string query)
        {
            var routes = _routesAdapter.GetData();
            var cities = _citiesAdapter.GetData();
            query = query?.ToLowerInvariant() ?? "";

            // Используем LINQ для объединения данных маршрутов с городами
            var joinedRoutes = from route in routes
                               join fromCity in cities on route.fromCityID equals fromCity.cityID
                               join toCity in cities on route.toCityID equals toCity.cityID
                               where route.routeID.ToString().Contains(query)
                                  || fromCity.cityName.ToLowerInvariant().Contains(query)
                                  || toCity.cityName.ToLowerInvariant().Contains(query)
                                  || route.distanceKm.ToString().Contains(query)
                               select new Route
                               {
                                   RouteID = route.routeID,
                                   FromCityID = route.fromCityID,
                                   ToCityID = route.toCityID,
                                   FromCityName = fromCity.cityName,
                                   ToCityName = toCity.cityName,
                                   DistanceKm = (Double)route.distanceKm
                               };

            RoutesListView.ItemsSource = joinedRoutes.ToList();
        }

        // Сортировка маршрутов по выбранному полю
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
            var routes = _routesAdapter.GetData();
            var cities = _citiesAdapter.GetData();

            var joinedRoutes = from route in routes
                               join fromCity in cities on route.fromCityID equals fromCity.cityID
                               join toCity in cities on route.toCityID equals toCity.cityID
                               select new Route
                               {
                                   RouteID = route.routeID,
                                   FromCityID = route.fromCityID,
                                   ToCityID = route.toCityID,
                                   FromCityName = fromCity.cityName,
                                   ToCityName = toCity.cityName,
                                   DistanceKm = (Double)route.distanceKm
                               };

            // Применяем сортировку в зависимости от выбранного поля
            var sortedRoutes = _sortAscending
                ? SortByField(joinedRoutes, _currentSortField)
                : SortByFieldDescending(joinedRoutes, _currentSortField);

            RoutesListView.ItemsSource = sortedRoutes.ToList();
        }

        private IEnumerable<Route> SortByField(IEnumerable<Route> routes, string field)
        {
            switch (field)
            {
                case "RouteID":
                    return routes.OrderBy(r => r.RouteID);
                case "FromCityName":
                    return routes.OrderBy(r => r.FromCityName);
                case "ToCityName":
                    return routes.OrderBy(r => r.ToCityName);
                case "DistanceKm":
                    return routes.OrderBy(r => r.DistanceKm);
                default:
                    return routes;
            }
        }

        private IEnumerable<Route> SortByFieldDescending(IEnumerable<Route> routes, string field)
        {
            switch (field)
            {
                case "RouteID":
                    return routes.OrderByDescending(r => r.RouteID);
                case "FromCityName":
                    return routes.OrderByDescending(r => r.FromCityName);
                case "ToCityName":
                    return routes.OrderByDescending(r => r.ToCityName);
                case "DistanceKm":
                    return routes.OrderByDescending(r => r.DistanceKm);
                default:
                    return routes;
            }
        }

        private object GetFieldValue(dynamic item, string field)
        {
            return item.GetType().GetProperty(field)?.GetValue(item, null);
        }

        // Удаление выбранного маршрута
        public async Task DeleteSelectedAsync()
        {
            if (RoutesListView.SelectedItem is Route selectedRoute)
            {
                int routeID = selectedRoute.RouteID;
                bool? result = CustomMessageBox.Show($"Удалить маршрут с ID = {routeID}?", "Подтверждение удаления", showCancel: true);
                if (result == true)
                {
                    try
                    {
                        // Удаление маршрута
                        await Task.Run(() => _routesAdapter.DeleteQuery(routeID));
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

        // Обновление списка маршрутов
        public async Task RefreshAsync()
        {
            var routes = _routesAdapter.GetData();
            var cities = _citiesAdapter.GetData();

            if (!routes.Any() || !cities.Any())
            {
                MessageBox.Show("Нет данных для отображения");
                return;
            }

            var joinedRoutes = from route in routes
                               join fromCity in cities on route.fromCityID equals fromCity.cityID
                               join toCity in cities on route.toCityID equals toCity.cityID
                               select new Route
                               {
                                   RouteID = route.routeID,
                                   FromCityID = route.fromCityID,
                                   ToCityID = route.toCityID,
                                   FromCityName = fromCity.cityName,
                                   ToCityName = toCity.cityName,
                                   DistanceKm = (Double)route.distanceKm
                               };

            RoutesListView.ItemsSource = joinedRoutes.ToList();
        }

        // Двойной клик по маршруту для редактирования
        private void RoutesListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (RoutesListView.SelectedItem is Route selectedRoute)
            {
                int routeID = selectedRoute.RouteID;

                // Отключаем навигацию
                _stackpanel.IsEnabled = false;

                // Навигация на страницу редактирования маршрута
                if (NavigationService != null)
                {
                    NavigationService.Navigate(new AddEditRoutePage(routeID));
                }
                else
                {
                    MessageBox.Show("Ошибка навигации");
                }
            }
        }

        // Страница загружена, обновляем данные
        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            _stackpanelBorder.Visibility = Visibility.Visible;

            try
            {
                await RefreshAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке данных: {ex.Message}");
            }
        }
    }
}
