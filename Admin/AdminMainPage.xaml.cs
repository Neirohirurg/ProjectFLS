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
using System.Windows.Navigation;
using System.Windows.Shapes;
using ProjectFLS.Admin.DataPages.UsersButton;
using System.Windows.Controls.Primitives;
using System.Windows.Media.Animation;
using ProjectFLS.Admin.DataPages.DerictoriesButton;
using ProjectFLS.Interfaces;
using ProjectFLS.UI;
using ProjectFLS.Admin.DataPages.History;
namespace ProjectFLS.Admin
{
    /// <summary>
    /// Логика взаимодействия для AdminMainPage.xaml
    /// </summary>
    public partial class AdminMainPage : Page
    {
        public AdminMainPage()
        {
            InitializeComponent();
        }

        private UserModel _user;
        private StackPanel _stackPanel;
        private Border _border;
        private UsersPage _usersPage;
        private DerictoriesPage _derictoriesPage;



        public AdminMainPage(UserModel user)
        {
            InitializeComponent();

            _user = user;
            _stackPanel = App.mainStackPanel;
            _border = App.mainStackPanelBorder;
            _usersPage = new UsersPage();
            _derictoriesPage = new DerictoriesPage(this.AdminMainFrame);
        }
        private void UpdateWidthMainNavBarBorder()
        {
            this._border.Width = this._stackPanel.ActualWidth;

        }

        // Пользователи 
        private void users_Click(object sender, RoutedEventArgs e)
        {
            AdminMainFrame.Navigate(_usersPage);
            ShowUsersNavBar();
        }

        public void PerformSearch(string query)
        {
            if (AdminMainFrame?.Content is ISearchable searchablePage)
            {
                searchablePage.PerformSearch(query);
            }
            else
            {
                CustomMessageBox.Show("Поиск недоступен на этой странице.", "Информация", showCancel: false);
            }
        }
        private Label CreateNavLabel(string content, MouseButtonEventHandler handler)
        {
            var label = new Label
            {
                Content = content,
                Style = (Style)FindResource("menuLabel")
            };
            label.MouseLeftButtonUp += handler;
            return label;
        }

        private bool _eventsAttached = false;

        private void ShowUsersNavBar()
        {
            _stackPanel.Children.Clear();

            if (!_eventsAttached)
            {
                _stackPanel.Loaded += UsersNavBar_Loaded;
                _stackPanel.SizeChanged += (s, e) => UpdateWidthMainNavBarBorder();
                _eventsAttached = true;
            }

            _stackPanel.Children.Add(CreateNavLabel("Зарегистрировать", Add_Click));
            _stackPanel.Children.Add(CreateNavLabel("Удалить", Delete_Click));
            _stackPanel.Children.Add(CreateNavLabel("Редактировать", Change_Click));

        }


        private void UsersNavBar_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateWidthMainNavBarBorder();
        }

        private void Add_Click(object sender, MouseButtonEventArgs e)
        {
            // Навигация на страницу добавления — без параметров (userId == null)
            App.mainStackPanel.IsEnabled = false;
            AdminMainFrame.Navigate(new AddEditUserPage());
        }

        private void Delete_Click(object sender, MouseButtonEventArgs e)
        {
            _usersPage.DeleteSelectedUser();
        }



        private void Change_Click(object sender, MouseButtonEventArgs e)
        {
            var selectedUser = _usersPage.GetSelectedUser();
            if (selectedUser == null)
            {
                MessageBox.Show("Выберите пользователя для редактирования", "Редактирование", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }

            int userId = selectedUser.userID;

            // Навигация на страницу AddEditUserPage с параметром userId для редактирования
            AdminMainFrame.Navigate(new AddEditUserPage(userId));
        }


        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            AdminMainFrame.Navigate(_usersPage); 
            ShowUsersNavBar();

            var panelStoryboard = (Storyboard)FindResource("SlideInSidebar");
            panelStoryboard.Begin();

            // Ждём завершения анимации панели
            await Task.Delay(500);

            // Затем анимация полоски
            var lineStoryboard = (Storyboard)FindResource("GrowVerticalLine");
            lineStoryboard.Begin();
        }

        private void AdminMainFrame_Navigated(object sender, NavigationEventArgs e)
        {
          
            if (e.Content is UsersPage && e.Content is ISearchable)
            {
                _usersPage.RefreshUsers();
                App.mainStackPanel.IsEnabled = true;
            }
          
            // Только страницы справочников получат меню
            else if (e.Content is INavigationPanelHost navHost && e.Content is ISearchable)
            {
                navHost.SetupNavigationPanel(App.mainStackPanel, App.mainStackPanelBorder);
                App.mainStackPanel.IsEnabled = true;
            }
            else if (!(e.Content is AddEditUserPage))
            {
                App.mainStackPanel.Children.Clear();
                App.mainStackPanelBorder.Visibility = Visibility.Collapsed;
            }
        }

        // Справочники


        private void derictories_Click(object sender, RoutedEventArgs e)
        {
            AdminMainFrame.Navigate(_derictoriesPage);
        }

        private void logs_Click(object sender, RoutedEventArgs e)
        {
            AdminMainFrame.Navigate(new HistoryPage());
        }
    }
}
