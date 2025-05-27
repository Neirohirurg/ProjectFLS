using System.Windows;
using System.Windows.Controls;
using ProjectFLS.flsdbDataSetTableAdapters;
using System;
using System.Windows.Media;
using System.Windows.Navigation;
using ProjectFLS.UI;

namespace ProjectFLS.Admin.DataPages.UsersButton
{
    public partial class AddEditUserPage : Page
    {
        private readonly usersTableAdapter _users = new usersTableAdapter();
        private readonly rolesTableAdapter _roles = new rolesTableAdapter();
        private readonly userStatusTableAdapter _statuses = new userStatusTableAdapter(); // новый адаптер
        private StackPanel _stackPanel;

        private int? _editUserId = null;

        public AddEditUserPage(int? userId = null)
        {
            InitializeComponent();
            RoleComboBox.ItemsSource = _roles.GetData();
            RoleComboBox.DisplayMemberPath = "roleName";
            RoleComboBox.SelectedValuePath = "roleID";

            StatusComboBox.ItemsSource = _statuses.GetData(); // инициализация ComboBox статуса
            StatusComboBox.DisplayMemberPath = "statusName";
            StatusComboBox.SelectedValuePath = "statusID";

            if (userId.HasValue)
            {
                LoadUser(userId.Value);
                _editUserId = userId;
                _stackPanel = App.mainStackPanel;
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
                StatusComboBox.SelectedValue = user.statusID; // загрузка статуса
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
            StatusComboBox.ClearValue(ComboBox.BackgroundProperty);

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

            // Проверка логина, пароля, роли и статуса
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
            if (StatusComboBox.SelectedItem == null)
            {
                StatusComboBox.Background = redBrush;
                hasError = true;
            }

            if (hasError)
            {
                CustomMessageBox.Show("Пожалуйста, заполните все поля корректно (только буквы в ФИО).", "Ошибка", showCancel: false);
                return;
            }

            try
            {
                if (_editUserId.HasValue)
                {
                    _users.UpdateQuery(
                        SurnameTextBox.Text,
                        FirstnameTextBox.Text,
                        PatronymicTextBox.Text,
                        UsernameTextBox.Text,
                        PasswordBox.Password,
                        (int)RoleComboBox.SelectedValue,
                        (int)StatusComboBox.SelectedValue, // статус
                        _editUserId.Value
                    );
                    CustomMessageBox.Show("Пользователь обновлен.", "Успешно", showCancel: false);
                }
                else
                {
                    _users.Insert(
                        SurnameTextBox.Text,
                        FirstnameTextBox.Text,
                        PatronymicTextBox.Text,
                        UsernameTextBox.Text,
                        PasswordBox.Password,
                        (int)RoleComboBox.SelectedValue,
                        (int)StatusComboBox.SelectedValue // статус
                    );
                    CustomMessageBox.Show("Пользователь добавлен.", "Успешно", showCancel: false);
                }

                NavigationService.GoBack();
            }
            catch (Exception ex)
            {
                CustomMessageBox.Show("Ошибка при сохранении пользователя: " + ex.Message, "Ошибка", showCancel: false);
            }
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            if (_stackPanel != null)
                _stackPanel.IsEnabled = true;
        }
    }
}
