using ProjectFLS.flsdbDataSetTableAdapters;
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
    public partial class UsersPage : Page
    {
        private usersTableAdapter _users;
        private rolesTableAdapter _roles;
        private userStatusTableAdapter _userStatuses;
        private StackPanel _stackpanel;

        public UsersPage(StackPanel stackpanel)
        {
            InitializeComponent();
            _users = new usersTableAdapter();
            _roles = new rolesTableAdapter();
            _userStatuses = new userStatusTableAdapter();
            _stackpanel = stackpanel;
        }

        public dynamic GetSelectedUser()
        {
            return UsersListView.SelectedItem;
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
                NavigationService?.Navigate(new AddEditUserPage(_stackpanel, userId));
            }
        }


        private void Page_Loaded(object sender, RoutedEventArgs e)
        {

            RefreshUsers();
        }
    }
}
