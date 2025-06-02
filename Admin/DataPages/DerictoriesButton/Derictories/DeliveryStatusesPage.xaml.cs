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
    public partial class DeliveryStatusesPage : Page, ISearchable, INavigationPanelHost
    {
        private deliveryStatusesTableAdapter _deliveryStatusesAdapter;
        private StackPanel _stackpanel;
        private Border _stackpanelBorder;
        private string _currentSortField = null;
        private bool _sortAscending = true;

        public DeliveryStatusesPage()
        {
            InitializeComponent();
            _deliveryStatusesAdapter = new deliveryStatusesTableAdapter();
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
                NavigationService?.Navigate(new AddEditDeliveryStatusPage());
            };

            var editLabel = new Label { Content = "Редактировать", Style = (Style)Application.Current.FindResource("menuLabel"), Margin = new Thickness(5), Cursor = Cursors.Hand };
            editLabel.MouseLeftButtonUp += (s, e) =>
            {
                if (DeliveryStatusesListView.SelectedItem is flsdbDataSet.deliveryStatusesRow selectedStatus)
                {
                    _stackpanel.IsEnabled = false;
                    NavigationService?.Navigate(new AddEditDeliveryStatusPage(selectedStatus.statusID));
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
            var all = _deliveryStatusesAdapter.GetData();
            query = query?.ToLowerInvariant() ?? "";

            var filtered = all.Where(t => t.statusName != null && t.statusName.ToLower().Contains(query)).ToList();

            DeliveryStatusesListView.ItemsSource = filtered;
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
            var data = _deliveryStatusesAdapter.GetData().AsEnumerable();
            var sortedData = _sortAscending
                ? data.OrderBy(row => GetFieldValue(row, _currentSortField))
                : data.OrderByDescending(row => GetFieldValue(row, _currentSortField));

            DeliveryStatusesListView.ItemsSource = sortedData.ToList();
        }

        private object GetFieldValue(dynamic item, string field)
        {
            return item.GetType().GetProperty(field)?.GetValue(item, null);
        }

        public async Task DeleteSelectedAsync()
        {
            if (DeliveryStatusesListView.SelectedItem is flsdbDataSet.deliveryStatusesRow selectedStatus)
            {
                int statusID = selectedStatus.statusID;
                bool? result = CustomMessageBox.Show($"Удалить статус доставки с ID = {statusID}?", "Подтверждение удаления", showCancel: true);
                if (result == true)
                {
                    try
                    {
                        await Task.Run(() => _deliveryStatusesAdapter.DeleteQuery(statusID));
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
            var list = await Task.Run(() => _deliveryStatusesAdapter.GetData());
            DeliveryStatusesListView.ItemsSource = list.ToList();
        }

        private void DeliveryStatusesListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (DeliveryStatusesListView.SelectedItem is flsdbDataSet.deliveryStatusesRow selectedStatus)
            {
                _stackpanelBorder.Visibility = Visibility.Visible;
                _stackpanel.IsEnabled = false;
                NavigationService?.Navigate(new AddEditDeliveryStatusPage(selectedStatus.statusID));
            }
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            _stackpanelBorder.Visibility = Visibility.Visible;
            await RefreshAsync();
        }
    }
}
