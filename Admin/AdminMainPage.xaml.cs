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
using ProjectFLS.Admin.DataPages;
using System.Windows.Controls.Primitives;
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
        private UsersPage _usersPage = new UsersPage();

        public AdminMainPage(UserModel user, StackPanel mainNavBar, Border navBarBorder)
        {
            InitializeComponent();
            _user = user;
            _stackPanel = mainNavBar;
            _border = navBarBorder;
        }
        private void UpdateWidthMainNavBarBorder()
        {
            this._border.Width = this._stackPanel.ActualWidth;

        }
        private void users_Click(object sender, RoutedEventArgs e)
        {
            AdminMainFrame.Navigate(_usersPage);
            ShowUsersNavBar();
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

            _stackPanel.Children.Add(CreateNavLabel("Добавить", Add_Click));
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


        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            AdminMainFrame.Navigate(_usersPage); 
            ShowUsersNavBar();
        }

        private void AdminMainFrame_Navigated(object sender, NavigationEventArgs e)
        {
            if (e.Content is UsersPage)
            {
                _usersPage.RefreshUsers();
            }
        }
    }
}
