using ProjectFLS.flsdbDataSetTableAdapters;
using ProjectFLS.Interfaces;
using ProjectFLS.UI;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ProjectFLS.Admin.DataPages.UsersButton
{
    /// <summary>
    /// Логика взаимодействия для UsersPage.xaml
    /// </summary>
    public partial class UsersPage : Page, ISearchable
    {
        private usersTableAdapter _users;
        private rolesTableAdapter _roles;
        private userStatusTableAdapter _userStatuses;
        private StackPanel _stackpanel;
        private Border _stackpanelBorder;

        private string _currentSortField = null;
        private bool _sortAscending = true;

        public string SortArrow_userID => GetSortArrow("userID");
        public string SortArrow_surname => GetSortArrow("surname");
        public string SortArrow_firstname => GetSortArrow("firstname");
        public string SortArrow_patronymic => GetSortArrow("patronymic");
        public string SortArrow_roleName => GetSortArrow("roleName");
        public string SortArrow_statusName => GetSortArrow("statusName");

        private string GetSortArrow(string field)
        {
            if (_currentSortField != field) return "";
            return _sortAscending ? "▲" : "▼";
        }

        public UsersPage()
        {
            InitializeComponent();
            _users = new usersTableAdapter();
            _roles = new rolesTableAdapter();
            _userStatuses = new userStatusTableAdapter();
            _stackpanel = App.mainStackPanel;
            _stackpanelBorder = App.mainStackPanelBorder;
        }

        public dynamic GetSelectedUser()
        {
            return UsersListView.SelectedItem;
        }

        public void PerformSearch(string query)
        {
            var users = _users.GetData();
            var roles = _roles.GetData();
            var statuses = _userStatuses.GetData();

            int currentUserId = App.CurrentUserId;

            query = query?.ToLowerInvariant() ?? "";

            var joined = from user in users
                         where user.userID != currentUserId
                         join role in roles on user.roleID equals role.roleID
                         join status in statuses on user.statusID equals status.statusID
                         let fullName = $"{user.firstname} {user.surname} {user.patronymic}".ToLowerInvariant()
                         let roleName = role.roleName.ToLowerInvariant()
                         let statusName = status.statusName.ToLowerInvariant()
                         where fullName.Contains(query)
                            || roleName.Contains(query)
                            || statusName.Contains(query)
                         select new
                         {
                             user.userID,
                             user.surname,
                             user.firstname,
                             user.patronymic,
                             role.roleName,
                             status.statusName
                         };

            UsersListView.ItemsSource = joined.ToList();
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
            var users = _users.GetData();
            var roles = _roles.GetData();
            var statuses = _userStatuses.GetData();

            int currentUserId = App.CurrentUserId;

            var joined = from user in users
                         where user.userID != currentUserId
                         join role in roles on user.roleID equals role.roleID
                         join status in statuses on user.statusID equals status.statusID
                         select new
                         {
                             user.userID,
                             user.surname,
                             user.firstname,
                             user.patronymic,
                             roleName = role.roleName,
                             statusName = status.statusName
                         };

            var sorted = _sortAscending
                ? joined.OrderBy(x => GetFieldValue(x, _currentSortField))
                : joined.OrderByDescending(x => GetFieldValue(x, _currentSortField));

            UsersListView.ItemsSource = sorted.ToList();

            // Обновить стрелки
            DataContext = null;
            DataContext = this;
        }

        private object GetFieldValue(dynamic item, string fieldName)
        {
            return item.GetType().GetProperty(fieldName)?.GetValue(item, null);
        }
        public void DeleteSelectedUser()
        {
            var selectedItem = UsersListView.SelectedItem;
            if (selectedItem != null)
            {
                dynamic user = selectedItem;
                int userID = user.userID;

                bool? result = CustomMessageBox.Show("Вы действительно хотите удалить пользователя с ID = " + userID + "?", "Подтверждение удаления", showCancel: true);
                if (result == true)
                {
                    try
                    {
                        _users.DeleteUserByID(userID);
                        RefreshUsers();
                        CustomMessageBox.Show("Пользователь успешно удалён.", "Удаление", showCancel: false);
                    }
                    catch (Exception ex)
                    {
                        CustomMessageBox.Show($"Ошибка при удалении: {ex.Message}", "Ошибка", showCancel: false);
                    }
                }


            }
            else
            {
                CustomMessageBox.Show("Выберите пользователя для удаления", "Удаление", showCancel: false);
            }
            
        }

        public void RefreshUsers()
        {
            var users = _users.GetData();
            var roles = _roles.GetData();
            var statuses = _userStatuses.GetData();

            int currentUserId = App.CurrentUserId; // ID текущего пользователя

            var joined = from user in users
                         where user.userID != currentUserId // Исключить себя
                         join role in roles on user.roleID equals role.roleID
                         join status in statuses on user.statusID equals status.statusID
                         select new
                         {
                             user.userID,
                             user.surname,
                             user.firstname,
                             user.patronymic,
                             role.roleName,
                             status.statusName // новое поле
                         };

            UsersListView.ItemsSource = joined.ToList();
        }


        private void UsersListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (UsersListView.SelectedItem != null)
            {
                dynamic user = UsersListView.SelectedItem;
                int userId = user.userID;

                // Отключаем навбар
                _stackpanel.IsEnabled = false;

                // Навигация на страницу редактирования
                NavigationService?.Navigate(new AddEditUserPage(userId));
            }
        }


        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            _stackpanelBorder.Visibility = Visibility.Visible;
            RefreshUsers();
        }
    }
}
