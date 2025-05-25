using ProjectFLS.Models;
using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;
using static ProjectFLS.flsdbDataSet;

namespace ProjectFLS
{
    /// <summary>
    /// Логика взаимодействия для LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();

            UsernameTextBox.Focus();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
           /* string username = UsernameTextBox.Text.Trim();
            string password = PasswordBox.Password;

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Введите логин и пароль.");
                return;
            }
*/
            var usersTable = new flsdbDataSetTableAdapters.usersTableAdapter();
            var rolesTable = new flsdbDataSetTableAdapters.rolesTableAdapter();

            //var userRows = usersTable.GetData().Where(u => u.username == username && u.password == password);
            var userRow = usersTable.GetData().FirstOrDefault(u => u.userID == 1);

            //var user = userRows.FirstOrDefault();
            var user = userRow;

            if (user != null)
            {
                var roleRow = rolesTable.GetData().FirstOrDefault(r => r.roleID == user.roleID);
                if (roleRow != null)
                {
                    var userModel = new UserModel
                    {
                        Id = user.userID,
                        Login = user.username,
                        Surname = user.surname,
                        Firstname = user.firstname,
                        Patronymic = user.patronymic,
                        RoleID = user.roleID,
                        RoleName = roleRow.roleName
                    };

                    MainWindow mainWindow = new MainWindow(userModel);
                    mainWindow.Show();
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Роль пользователя не найдена.");
                }
            }
            else
            {
                MessageBox.Show("Неверное имя пользователя или пароль.");
            }
        }



        private void ShowPasswordCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            PasswordVisibleTextBox.Text = PasswordBox.Password;
            PasswordBox.Visibility = Visibility.Collapsed;
            PasswordVisibleTextBox.Visibility = Visibility.Visible;
        }

        private void ShowPasswordCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            PasswordBox.Password = PasswordVisibleTextBox.Text;
            PasswordVisibleTextBox.Visibility = Visibility.Collapsed;
            PasswordBox.Visibility = Visibility.Visible;
        }

        private void UsernameTextBox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                var currentTextBox = sender as TextBox;
                if (currentTextBox == UsernameTextBox)
                    PasswordBox.Focus();
            }
        }

        private void PasswordBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                loginButton.Focus();
                e.Handled = true;  // Чтобы не обрабатывалось дальше
            }
        }

        private void loginButton_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                // Запускаем клик по кнопке программно
                LoginButton_Click(loginButton, new RoutedEventArgs());
                e.Handled = true;
            }
        }
    }
}
