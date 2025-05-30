using ProjectFLS.Admin;
using ProjectFLS.Interfaces;
using ProjectFLS.Logist;
using ProjectFLS.Logist.Applications;
using ProjectFLS.Manager;
using ProjectFLS.Models;
using ProjectFLS.UI;
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
using System.Windows.Media.Animation;
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

            App.mainStackPanelBorder = this.navBarBorder;
            App.mainStackPanel = this.mainNavBar;
            App.mainSearchTextBox = this.searchTextBox;
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
                    mainFrame.Navigate(new AdminMainPage(_user));
                    break;
                case 2: // Менеджер
                    mainFrame.Navigate(new ManagerMainPage(_user));
                    break;
                case 3: // Логист
                    mainFrame.Navigate(new LogistMainPage(_user));
                    break;
                default:
                    CustomMessageBox.Show("Неизвестная роль. Невозможно загрузить интерфейс.", "Ошибка", showCancel: false);
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

        // Поиск

        private bool isSearchAvailable = false;

        private void searchIcon_MouseEnter(object sender, MouseEventArgs e)
        {
            if (!isSearchAvailable) return;

            searchIcon.Visibility = Visibility.Collapsed;
            var storyboard = (Storyboard)this.Resources["ShowSearchBox"];
            storyboard.Begin();
            searchTextBox.Focus();
        }

        private void searchTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (!isSearchAvailable) return;

            var storyboard = (Storyboard)this.Resources["HideSearchBox"];

            // После завершения — вернуть иконку
            storyboard.Completed += (s, a) =>
            {
                searchIcon.Visibility = Visibility.Visible;
            };

            storyboard.Begin();
        }


        private void searchTextBox_MouseLeave(object sender, MouseEventArgs e)
        {
            searchTextBox_LostFocus(sender, e);
        }
        private void searchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!isSearchAvailable) return;

            string query = searchTextBox.Text;

            if (mainFrame.Content is AdminMainPage adminPage)
            {
                adminPage.PerformSearch(query);
            }
            else if (mainFrame.Content is ISearchable searchablePage)
            {
                searchablePage.PerformSearch(query);
            }
        }

        private void mainFrame_Navigated(object sender, NavigationEventArgs e)
        {
            if (e.Content is AdminMainPage adminPage)
            {
                this.EnableSearch();
            }
            else if (e.Content is ManagerMainPage managerPage)
            {
                this.EnableSearch();
            }
            else if (e.Content is ISearchable searchablePage)
            {
                searchablePage.EnableSearch();
                this.EnableSearch();
            }
            else if (e.Content is LogistMainPage logistPage)
            {

                this.EnableSearch();
            }
        }

        public void EnableSearch()
        {
            isSearchAvailable = true;
        }

    }
}
