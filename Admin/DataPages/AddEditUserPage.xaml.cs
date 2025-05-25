using System.Windows;
using System.Windows.Controls;
using ProjectFLS.flsdbDataSetTableAdapters;
using System;
using System.Windows.Media;

namespace ProjectFLS.Admin.DataPages
{
    public partial class AddEditUserPage : Page
    {
        private readonly usersTableAdapter _users = new usersTableAdapter();
        private readonly rolesTableAdapter _roles = new rolesTableAdapter();

        private int? _editUserId = null;

        public AddEditUserPage(int? userId = null)
        {
            InitializeComponent();
            RoleComboBox.ItemsSource = _roles.GetData();

            if (userId.HasValue)
            {
                LoadUser(userId.Value);
                _editUserId = userId;
            }
        }

        private void LoadUser(int userId)
        {
            var user = _users.GetData().FindByuserID(userId);
            if (user != null)
            {
                SurnameTextBox.Text = user.surname;
                FirstnameTextBox.Text = user.firstname;
                PatronymicTextBox.Text = user.patronymic;
                UsernameTextBox.Text = user.username;
                PasswordBox.Password = user.password;
                RoleComboBox.SelectedValue = user.roleID;
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            if (NavigationService.CanGoBack)
            {
                NavigationService.GoBack();
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

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            // Сбросим фон полей
            SurnameTextBox.ClearValue(TextBox.BackgroundProperty);
            FirstnameTextBox.ClearValue(TextBox.BackgroundProperty);
            PatronymicTextBox.ClearValue(TextBox.BackgroundProperty);
            UsernameTextBox.ClearValue(TextBox.BackgroundProperty);
            PasswordBox.ClearValue(PasswordBox.BackgroundProperty);
            RoleComboBox.ClearValue(ComboBox.BackgroundProperty);

            bool hasError = false;
            var redBrush = new SolidColorBrush(Color.FromRgb(255, 200, 200));
            var regex = new System.Text.RegularExpressions.Regex(@"^[А-Яа-яЁёA-Za-z\-]+$");

            // Проверка ФИО
            if (string.IsNullOrWhiteSpace(SurnameTextBox.Text) || !regex.IsMatch(SurnameTextBox.Text))
            {
                SurnameTextBox.Background = redBrush;
                hasError = true;
            }
            if (string.IsNullOrWhiteSpace(FirstnameTextBox.Text) || !regex.IsMatch(FirstnameTextBox.Text))
            {
                FirstnameTextBox.Background = redBrush;
                hasError = true;
            }
            if (string.IsNullOrWhiteSpace(PatronymicTextBox.Text) || !regex.IsMatch(PatronymicTextBox.Text))
            {
                PatronymicTextBox.Background = redBrush;
                hasError = true;
            }

            // Проверка логина и пароля
            if (string.IsNullOrWhiteSpace(UsernameTextBox.Text))
            {
                UsernameTextBox.Background = redBrush;
                hasError = true;
            }
            if (string.IsNullOrWhiteSpace(PasswordBox.Password))
            {
                PasswordBox.Background = redBrush;
                hasError = true;
            }
            if (RoleComboBox.SelectedItem == null)
            {
                RoleComboBox.Background = redBrush;
                hasError = true;
            }

            if (hasError)
            {
                MessageBox.Show("Пожалуйста, заполните все поля корректно (только буквы в ФИО).", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (_editUserId.HasValue)
            {
                _users.UpdateQuery(
                    SurnameTextBox.Text,
                    FirstnameTextBox.Text,
                    PatronymicTextBox.Text,
                    UsernameTextBox.Text,
                    PasswordBox.Password,
                    (int)RoleComboBox.SelectedValue,
                    _editUserId.Value
                );
                MessageBox.Show("Пользователь обновлен.", "Успешно", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                try
                {
                    _users.Insert(
                        SurnameTextBox.Text,
                        FirstnameTextBox.Text,
                        PatronymicTextBox.Text,
                        UsernameTextBox.Text,
                        PasswordBox.Password,
                        (int)RoleComboBox.SelectedValue
                    );
                    MessageBox.Show("Пользователь добавлен.", "Успешно", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при добавлении пользователя: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

            NavigationService.GoBack();
        }



    }
}
