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
    public partial class RolesPage : Page, ISearchable, INavigationPanelHost
    {
        private rolesTableAdapter _roles;
        private StackPanel _stackpanel;
        private Border _stackpanelBorder;

        private string _currentSortField = null;
        private bool _sortAscending = true;

        public string SortArrow_roleID => GetSortArrow("roleID");
        public string SortArrow_roleName => GetSortArrow("roleName");

        private string GetSortArrow(string field)
        {
            if (_currentSortField != field) return "";
            return _sortAscending ? "▲" : "▼";
        }

        public RolesPage()
        {
            InitializeComponent();
            _roles = new rolesTableAdapter();
            _stackpanel = App.mainStackPanel;
            _stackpanelBorder = App.mainStackPanelBorder;
        }

        public dynamic GetSelectedRole()
        {
            return RolesListView.SelectedItem;
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
                _stackpanelBorder.Visibility = Visibility.Visible;
                _stackpanel.IsEnabled = false;

                NavigationService?.Navigate(new AddEditRolesPage());
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
                if (GetSelectedRole() != null)
                {
                    _stackpanelBorder.Visibility = Visibility.Visible;
                    _stackpanel.IsEnabled = false;

                    NavigationService?.Navigate(new AddEditRolesPage(GetSelectedRole().roleID));
                }
            };

            var deleteLabel = new Label
            {
                Content = "Удалить",
                Style = (Style)Application.Current.FindResource("menuLabel"),
                Margin = new Thickness(5),
                Cursor = Cursors.Hand
            };
            deleteLabel.MouseLeftButtonUp += (s, e) => DeleteSelectedRole();

            panel.Children.Add(addLabel);
            panel.Children.Add(editLabel);
            panel.Children.Add(deleteLabel);
        }

        public void PerformSearch(string query)
        {
            var roles = _roles.GetData();

            query = query?.ToLowerInvariant() ?? "";

            var filtered = from role in roles
                           let roleName = role.roleName.ToLowerInvariant()
                           where roleName.Contains(query)
                           select new
                           {
                               role.roleID,
                               role.roleName
                           };

            RolesListView.ItemsSource = filtered.ToList();
        }

        public void EnableSearch()
        {
            // реализация обязательна для активации поиска в MainWindow
        }

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
            var roles = _roles.GetData();

            var sorted = _sortAscending
                ? roles.OrderBy(x => GetFieldValue(x, _currentSortField))
                : roles.OrderByDescending(x => GetFieldValue(x, _currentSortField));

            RolesListView.ItemsSource = sorted.ToList();

            // Обновить стрелки
            DataContext = null;
            DataContext = this;
        }

        private object GetFieldValue(dynamic item, string fieldName)
        {
            return item.GetType().GetProperty(fieldName)?.GetValue(item, null);
        }

        public void DeleteSelectedRole()
        {
            var selectedItem = RolesListView.SelectedItem;
            if (selectedItem != null)
            {
                dynamic role = selectedItem;
                int roleID = role.roleID;

                bool? result = CustomMessageBox.Show("Вы действительно хотите удалить роль с ID = " + roleID + "?", "Подтверждение удаления", showCancel: true);
                if (result == true)
                {
                    try
                    {
                        _roles.DeleteRoleByID(roleID);
                        RefreshRoles();
                        CustomMessageBox.Show("Роль успешно удалена.", "Удаление", showCancel: false);
                    }
                    catch (Exception ex)
                    {
                        CustomMessageBox.Show($"Ошибка при удалении: {ex.Message}", "Ошибка", showCancel: false);
                    }
                }
            }
            else
            {
                CustomMessageBox.Show("Выберите роль для удаления", "Удаление", showCancel: false);
            }
        }

        public void RefreshRoles()
        {
            var roles = _roles.GetData();

            var roleList = from role in roles
                           select new
                           {
                               role.roleID,
                               role.roleName
                           };

            RolesListView.ItemsSource = roleList.ToList();
        }

        private void RolesListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (RolesListView.SelectedItem != null)
            {
                dynamic role = RolesListView.SelectedItem;
                int roleId = role.roleID;

                // Отключаем навбар
                _stackpanelBorder.Visibility = Visibility.Visible;
                _stackpanel.IsEnabled = false;

                // Навигация на страницу редактирования
                NavigationService?.Navigate(new AddEditRolesPage(roleId));
            }
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            _stackpanelBorder.Visibility = Visibility.Visible;
            RefreshRoles();
        }


    }
}
