using ProjectFLS.flsdbDataSetTableAdapters;
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

namespace ProjectFLS.Admin.DataPages
{
    /// <summary>
    /// Логика взаимодействия для UsersPage.xaml
    /// </summary>
    public partial class UsersPage : Page
    {
        private usersTableAdapter _users;
        private rolesTableAdapter _roles;

        public UsersPage()
        {
            InitializeComponent();
            _users = new usersTableAdapter();
            _roles = new rolesTableAdapter();  

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

                var result = MessageBox.Show(
                    $"Вы действительно хотите удалить пользователя с ID = {userID}?",
                    "Подтверждение удаления",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning
                );

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        _users.DeleteUserByID(userID);
                        RefreshUsers();
                        MessageBox.Show("Пользователь успешно удалён.", "Удаление", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка при удалении: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Выберите пользователя для удаления", "Удаление", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
            
        }

        public void RefreshUsers()
        {
            var users = _users.GetData();
            var roles = _roles.GetData();

            int currentUserId = App.CurrentUserId; // ID текущего пользователя

            var joined = from user in users
                         where user.userID != currentUserId // Исключить себя
                         join role in roles on user.roleID equals role.roleID
                         select new
                         {
                             user.userID,
                             user.surname,
                             user.firstname,
                             user.patronymic,
                             role.roleName
                         };

            UsersListView.ItemsSource = joined.ToList();
        }

        private void UsersListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (UsersListView.SelectedItem != null)
            {
                // Приведение к анонимному типу не работает напрямую, используем dynamic
                dynamic user = UsersListView.SelectedItem;

                int userId = user.userID;

                // Навигация на страницу редактирования
                NavigationService?.Navigate(new AddEditUserPage(userId));
            }
        }


        private void Page_Loaded(object sender, RoutedEventArgs e)
        {

            RefreshUsers();
        }
    }
}
