using ProjectFLS.Models;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using ProjectFLS.Interfaces;
using System.Threading.Tasks;
using ProjectFLS.Admin.DataPages.DerictoriesButton;
using ProjectFLS.Admin.DataPages.History;
using ProjectFLS.Admin.DataPages.UsersButton;
using System.Windows.Input;

namespace ProjectFLS.Logist
{
    public partial class LogistMainPage : Page
    {
        private UserModel _user;
        private StackPanel _stackPanel;
        private Border _border;


        public LogistMainPage()
        {
            InitializeComponent();
        }

        public LogistMainPage(UserModel user)
        {
            InitializeComponent();
            _user = user;
            _stackPanel = App.mainStackPanel;
            _border = App.mainStackPanelBorder;

        }

        private void UpdateWidthMainNavBarBorder()
        {
            this._border.Width = this._stackPanel.ActualWidth;
        }

        // Обработчик загрузки страницы
        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {

            ShowUsersNavBar();

            var panelStoryboard = (Storyboard)FindResource("SlideInSidebar");
            panelStoryboard.Begin();

            // Ждём завершения анимации панели
            await Task.Delay(500);

            // Затем анимация полоски
            var lineStoryboard = (Storyboard)FindResource("GrowVerticalLine");
            lineStoryboard.Begin();
        }

        private void LogistMainFrame_Navigated(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
            
            if (e.Content is INavigationPanelHost navHost && e.Content is ISearchable)
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

        // Кнопка "Пользователи"
        

        // Показывает панель навигации для пользователей
        private void ShowUsersNavBar()
        {
            _stackPanel.Children.Clear();

            _stackPanel.Children.Add(CreateNavLabel("Зарегистрировать", Add_Click));
            _stackPanel.Children.Add(CreateNavLabel("Удалить", Delete_Click));
            _stackPanel.Children.Add(CreateNavLabel("Редактировать", Change_Click));
        }

        // Создаёт Label с обработчиком клика
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

        // Обработчик добавления нового пользователя
        private void Add_Click(object sender, MouseButtonEventArgs e)
        {
           
        }

        // Обработчик удаления пользователя
        private void Delete_Click(object sender, MouseButtonEventArgs e)
        {

        }

        // Обработчик редактирования пользователя
        private void Change_Click(object sender, MouseButtonEventArgs e)
        {
            
        }
    }
}
