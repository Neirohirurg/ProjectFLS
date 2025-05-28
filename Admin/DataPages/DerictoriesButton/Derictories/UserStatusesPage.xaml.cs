using ProjectFLS.flsdbDataSetTableAdapters;
using ProjectFLS.Interfaces;
using ProjectFLS.UI;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ProjectFLS.Admin.DataPages.DerictoriesButton.Derictories
{
    public partial class UserStatusesPage : Page, ISearchable, INavigationPanelHost
    {
        private userStatusTableAdapter _statuses;
        private StackPanel _stackpanel;
        private Border _stackpanelBorder;

        private string _currentSortField = null;
        private bool _sortAscending = true;

        public string SortArrow_statusID => GetSortArrow("statusID");
        public string SortArrow_statusName => GetSortArrow("statusName");

        private string GetSortArrow(string field)
        {
            if (_currentSortField != field) return "";
            return _sortAscending ? "▲" : "▼";
        }

        public UserStatusesPage()
        {
            InitializeComponent();
            _statuses = new userStatusTableAdapter();
            _stackpanel = App.mainStackPanel;
            _stackpanelBorder = App.mainStackPanelBorder;
        }

        public void SetupNavigationPanel(StackPanel panel, Border border)
        {
            panel.Children.Clear();

            var addLabel = new Label
            {
                Content = "Добавить",
                Style = (Style)Application.Current.FindResource("menuLabel"),
                Margin = new Thickness(5),
                Cursor = Cursors.Hand
            };
            addLabel.MouseLeftButtonUp += (s, e) =>
            {
                App.mainStackPanel.IsEnabled = false;
                NavigationService?.Navigate(new AddEditUserStatusPage());
            };

            var editLabel = new Label
            {
                Content = "Редактировать",
                Style = (Style)Application.Current.FindResource("menuLabel"),
                Margin = new Thickness(5),
                Cursor = Cursors.Hand
            };
            editLabel.MouseLeftButtonUp += (s, e) =>
            {
                if (UserStatusesListView.SelectedItem != null)
                {
                    dynamic item = UserStatusesListView.SelectedItem;
                    App.mainStackPanel.IsEnabled = false;
                    NavigationService?.Navigate(new AddEditUserStatusPage(item.statusID));
                }
            };

            var deleteLabel = new Label
            {
                Content = "Удалить",
                Style = (Style)Application.Current.FindResource("menuLabel"),
                Margin = new Thickness(5),
                Cursor = Cursors.Hand
            };
            deleteLabel.MouseLeftButtonUp += (s, e) => DeleteSelectedStatus();

            panel.Children.Add(addLabel);
            panel.Children.Add(editLabel);
            panel.Children.Add(deleteLabel);
        }

        public void PerformSearch(string query)
        {
            var statuses = _statuses.GetData();

            query = query?.ToLowerInvariant() ?? "";

            var filtered = from status in statuses
                           let statusName = status.statusName.ToLowerInvariant()
                           where statusName.Contains(query)
                           select new
                           {
                               status.statusID,
                               status.statusName
                           };

            UserStatusesListView.ItemsSource = filtered.ToList();
        }

        public void EnableSearch() { }

        private void Sort_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is string sortField)
            {
                if (_currentSortField == sortField)
                    _sortAscending = !_sortAscending;
                else
                {
                    _currentSortField = sortField;
                    _sortAscending = true;
                }

                ApplySorting();
            }
        }

        private void ApplySorting()
        {
            var statuses = _statuses.GetData();

            var sorted = _sortAscending
                ? statuses.OrderBy(x => GetFieldValue(x, _currentSortField))
                : statuses.OrderByDescending(x => GetFieldValue(x, _currentSortField));

            UserStatusesListView.ItemsSource = sorted.ToList();

            DataContext = null;
            DataContext = this;
        }

        private object GetFieldValue(dynamic item, string fieldName)
        {
            return item.GetType().GetProperty(fieldName)?.GetValue(item, null);
        }

        public void DeleteSelectedStatus()
        {
            if (UserStatusesListView.SelectedItem != null)
            {
                dynamic status = UserStatusesListView.SelectedItem;
                int id = status.statusID;

                bool? result = CustomMessageBox.Show($"Удалить статус с ID = {id}?", "Удаление", showCancel: true);
                if (result == true)
                {
                    try
                    {
                        _statuses.DeleteQuery(id);
                        RefreshStatuses();
                        CustomMessageBox.Show("Статус успешно удалён.", "Успешно", showCancel: false);
                    }
                    catch (Exception ex)
                    {
                        CustomMessageBox.Show($"Ошибка при удалении: {ex.Message}", "Ошибка", showCancel: false);
                    }
                }
            }
            else
            {
                CustomMessageBox.Show("Выберите статус для удаления.", "Удаление", showCancel: false);
            }
        }

        public void RefreshStatuses()
        {
            var statuses = _statuses.GetData();

            var list = from status in statuses
                       select new
                       {
                           status.statusID,
                           status.statusName
                       };

            UserStatusesListView.ItemsSource = list.ToList();
        }

        private void UserStatusesListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (UserStatusesListView.SelectedItem != null)
            {
                dynamic status = UserStatusesListView.SelectedItem;
                int id = status.statusID;

                _stackpanelBorder.Visibility = Visibility.Visible;
                _stackpanel.IsEnabled = false;

                NavigationService?.Navigate(new AddEditUserStatusPage(id));
            }
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            _stackpanelBorder.Visibility = Visibility.Visible;
            RefreshStatuses();
        }
    }
}
