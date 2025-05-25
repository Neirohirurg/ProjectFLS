using ProjectFLS.Admin;
using ProjectFLS.Logist;
using ProjectFLS.Manager;
using ProjectFLS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
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
using System.Windows.Threading;

namespace ProjectFLS
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private UserModel _user;

        public MainWindow(UserModel user)
        {
            InitializeComponent();
            _user = user;
      
        }
        private void mainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateDateTimeLabel();
            TimerInit();
            WelcomeLabel.Content = $"Добро пожаловать, {_user.Firstname} {_user.Patronymic} ({_user.RoleName})";

            LoadPageForRole(_user.RoleID);
        }

        private void LoadPageForRole(int roleID)
        {
            switch (roleID)
            {
                case 1: // Администратор
                    mainFrame.Navigate(new AdminMainPage(_user, this.mainNavBar, this.navBarBorder));
                    break;
                case 2: // Менеджер
                    mainFrame.Navigate(new ManagerMainPage(_user));
                    break;
                case 3: // Логист
                    mainFrame.Navigate(new LogistMainPage(_user));
                    break;
                default:
                    MessageBox.Show("Неизвестная роль. Невозможно загрузить интерфейс.");
                    break;
            }
        }
        private void TimerInit()
        {

            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(1);
            _timer.Tick += Timer_Tick;
            _timer.Start();
        }

        private void UpdateDateTimeLabel()
        {
            DateTime currentDateTime = DateTime.Now;
            string formattedDateTime = currentDateTime.ToString("dd.MM.yyyy | HH:mm:ss");
            dateLabel.Content = formattedDateTime;
        }

        private DispatcherTimer _timer;


        private void Timer_Tick(object sender, EventArgs e)
        {
            UpdateDateTimeLabel();
        }

        private void ExitToLoginButton_Click(object sender, RoutedEventArgs e)
        {
            var loginWindow = new LoginWindow();
            loginWindow.Show();
            this.Close();
        }

    }
}
