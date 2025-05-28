using ProjectFLS.flsdbDataSetTableAdapters;
using ProjectFLS.Interfaces;
using ProjectFLS.UI;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ProjectFLS.Admin.DataPages.DerictoriesButton.Derictories
{
    public partial class CitiesPage : Page, ISearchable, INavigationPanelHost
    {
        private citiesTableAdapter _citiesAdapter;
        private StackPanel _stackpanel;
        private Border _stackpanelBorder;
        private string _currentSortField = null;
        private bool _sortAscending = true;

        public CitiesPage()
        {
            InitializeComponent();
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
                NavigationService?.Navigate(new AddEditCityPage());
            };

            var editLabel = new Label { Content = "Редактировать", Style = (Style)Application.Current.FindResource("menuLabel"), Margin = new Thickness(5), Cursor = Cursors.Hand };
            editLabel.MouseLeftButtonUp += (s, e) =>
            {
                if (CitiesListView.SelectedItem is flsdbDataSet.citiesRow selectedCity)
                {
                    _stackpanel.IsEnabled = false;
                    NavigationService?.Navigate(new AddEditCityPage(selectedCity.cityID));
                }
            };

            var deleteLabel = new Label { Content = "Удалить", Style = (Style)Application.Current.FindResource("menuLabel"), Margin = new Thickness(5), Cursor = Cursors.Hand };
            deleteLabel.MouseLeftButtonUp += (s, e) => DeleteSelectedAsync();

            panel.Children.Add(addLabel);
            panel.Children.Add(editLabel);
            panel.Children.Add(deleteLabel);
        }

        public void PerformSearch(string query)
        {
            var all = _citiesAdapter.GetData();
            query = query?.ToLowerInvariant() ?? "";

            var filtered = all.Where(t => t.cityName != null && t.cityName.ToLower().Contains(query)).ToList();

            CitiesListView.ItemsSource = filtered;
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
            var data = _citiesAdapter.GetData().AsEnumerable();
            var sortedData = _sortAscending
                ? data.OrderBy(row => GetFieldValue(row, _currentSortField))
                : data.OrderByDescending(row => GetFieldValue(row, _currentSortField));

            CitiesListView.ItemsSource = sortedData.ToList();
        }

        private object GetFieldValue(dynamic item, string field)
        {
            return item.GetType().GetProperty(field)?.GetValue(item, null);
        }

        public async Task DeleteSelectedAsync()
        {
            if (CitiesListView.SelectedItem is flsdbDataSet.citiesRow selectedCity)
            {
                int cityID = selectedCity.cityID;
                bool? result = CustomMessageBox.Show($"Удалить город с ID = {cityID}?", "Подтверждение удаления", showCancel: true);
                if (result == true)
                {
                    try
                    {
                        await Task.Run(() => _citiesAdapter.DeleteQuery(cityID));
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
            var list = await Task.Run(() => _citiesAdapter.GetData());
            CitiesListView.ItemsSource = list.ToList();
        }

        private void CitiesListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (CitiesListView.SelectedItem is flsdbDataSet.citiesRow selectedCity)
            {
                _stackpanelBorder.Visibility = Visibility.Visible;
                _stackpanel.IsEnabled = false;
                NavigationService?.Navigate(new AddEditCityPage(selectedCity.cityID));
            }
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            _stackpanelBorder.Visibility = Visibility.Visible;
            await RefreshAsync();
        }
    }
}
