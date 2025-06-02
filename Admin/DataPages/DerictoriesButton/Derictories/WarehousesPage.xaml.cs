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
    public partial class WarehousesPage : Page, ISearchable, INavigationPanelHost
    {
        private warehousesTableAdapter _warehousesAdapter;
        private StackPanel _stackpanel;
        private Border _stackpanelBorder;
        private string _currentSortField = null;
        private bool _sortAscending = true;

        public WarehousesPage()
        {
            InitializeComponent();
            _warehousesAdapter = new warehousesTableAdapter();
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
                NavigationService?.Navigate(new AddEditWarehousePage());
            };

            var editLabel = new Label { Content = "Редактировать", Style = (Style)Application.Current.FindResource("menuLabel"), Margin = new Thickness(5), Cursor = Cursors.Hand };
            editLabel.MouseLeftButtonUp += (s, e) =>
            {
                if (WarehousesListView.SelectedItem is flsdbDataSet.warehousesRow selectedWarehouse)
                {
                    _stackpanel.IsEnabled = false;
                    NavigationService?.Navigate(new AddEditWarehousePage(selectedWarehouse.warehouseID));
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
            var all = _warehousesAdapter.GetData();
            query = query?.ToLowerInvariant() ?? "";

            var filtered = all.Where(w => w.warehouseName != null && w.warehouseName.ToLower().Contains(query)).ToList();

            WarehousesListView.ItemsSource = filtered;
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
            var data = _warehousesAdapter.GetData().AsEnumerable();
            var sortedData = _sortAscending
                ? data.OrderBy(row => GetFieldValue(row, _currentSortField))
                : data.OrderByDescending(row => GetFieldValue(row, _currentSortField));

            WarehousesListView.ItemsSource = sortedData.ToList();
        }

        private object GetFieldValue(dynamic item, string field)
        {
            return item.GetType().GetProperty(field)?.GetValue(item, null);
        }

        public async Task DeleteSelectedAsync()
        {
            if (WarehousesListView.SelectedItem is flsdbDataSet.warehousesRow selectedWarehouse)
            {
                int warehouseID = selectedWarehouse.warehouseID;
                bool? result = CustomMessageBox.Show($"Удалить склад с ID = {warehouseID}?", "Подтверждение удаления", showCancel: true);
                if (result == true)
                {
                    try
                    {
                        await Task.Run(() => _warehousesAdapter.DeleteQuery(warehouseID));
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
            var list = await Task.Run(() => _warehousesAdapter.GetData());
            WarehousesListView.ItemsSource = list.ToList();
        }

        private void WarehousesListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (WarehousesListView.SelectedItem is flsdbDataSet.warehousesRow selectedWarehouse)
            {
                _stackpanelBorder.Visibility = Visibility.Visible;
                _stackpanel.IsEnabled = false;
                NavigationService?.Navigate(new AddEditWarehousePage(selectedWarehouse.warehouseID));
            }
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            _stackpanelBorder.Visibility = Visibility.Visible;
            await RefreshAsync();
        }
    }
}
