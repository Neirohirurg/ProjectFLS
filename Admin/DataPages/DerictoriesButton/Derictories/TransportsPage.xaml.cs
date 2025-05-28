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
    public partial class TransportsPage : Page, ISearchable, INavigationPanelHost
    {
        private transportTableAdapter _transports;
        private StackPanel _stackpanel;
        private Border _stackpanelBorder;
        private string _currentSortField = null;
        private bool _sortAscending = true;

        public TransportsPage()
        {
            InitializeComponent();
            _transports = new transportTableAdapter();
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
                NavigationService?.Navigate(new AddEditTransportPage());
            };

            var editLabel = new Label { Content = "Редактировать", Style = (Style)Application.Current.FindResource("menuLabel"), Margin = new Thickness(5), Cursor = Cursors.Hand };
            editLabel.MouseLeftButtonUp += (s, e) =>
            {
                if (TransportsListView.SelectedItem is flsdbDataSet.transportRow selectedTransport)
                {
                    _stackpanel.IsEnabled = false;
                    NavigationService?.Navigate(new AddEditTransportPage(selectedTransport.transportID));
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
            var all = _transports.GetData();
            query = query?.ToLowerInvariant() ?? "";

            var filtered = all.Where(t => t.transportName != null && t.transportName.ToLower().Contains(query)).ToList();

            TransportsListView.ItemsSource = filtered;
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
            var data = _transports.GetData().AsEnumerable();
            var sortedData = _sortAscending
                ? data.OrderBy(row => GetFieldValue(row, _currentSortField))
                : data.OrderByDescending(row => GetFieldValue(row, _currentSortField));

            TransportsListView.ItemsSource = sortedData.ToList();
        }

        private object GetFieldValue(dynamic item, string field)
        {
            return item.GetType().GetProperty(field)?.GetValue(item, null);
        }

        public async Task DeleteSelectedAsync()
        {
            if (TransportsListView.SelectedItem is flsdbDataSet.transportRow selectedTransport)
            {
                int transportID = selectedTransport.transportID;
                bool? result = CustomMessageBox.Show($"Удалить транспорт с ID = {transportID}?", "Подтверждение удаления", showCancel: true);
                if (result == true)
                {
                    try
                    {
                        await Task.Run(() => _transports.DeleteQuery(transportID));
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
            var list = await Task.Run(() => _transports.GetData());
            TransportsListView.ItemsSource = list.ToList();
        }

        private void TransportsListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (TransportsListView.SelectedItem is flsdbDataSet.transportRow selectedTransport)
            {
                _stackpanelBorder.Visibility = Visibility.Visible;
                _stackpanel.IsEnabled = false;
                NavigationService?.Navigate(new AddEditTransportPage(selectedTransport.transportID));
            }
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            _stackpanelBorder.Visibility = Visibility.Visible;
            await RefreshAsync();
        }
    }
}
