using ProjectFLS.Models;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Threading.Tasks;
using ProjectFLS.Manager.DataPages;
using ProjectFLS.Interfaces;
using System.Windows.Input;
using ProjectFLS.UI;

namespace ProjectFLS.Manager
{
    public partial class ManagerMainPage : Page, ISearchable, INavigationPanelHost
    {
        private UserModel _user;
        private StackPanel _stackPanel;
        private Border _border;
        private WarehousesPage _warehousesPage;
        private PartnersPage _partnersPage;

        public ManagerMainPage()
        {
            InitializeComponent();
        }

        public ManagerMainPage(UserModel user)
        {
            InitializeComponent();
            _user = user;
            _stackPanel = App.mainStackPanel;
            _border = App.mainStackPanelBorder;
            _warehousesPage = new WarehousesPage();
            _partnersPage = new PartnersPage(this.ManagerMainFrame);
        }

        private void UpdateWidthMainNavBarBorder()
        {
            this._border.Width = this._stackPanel.ActualWidth;
        }

        // Обработчик загрузки страницы
        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            ManagerMainFrame.Navigate(_warehousesPage);

            var panelStoryboard = (Storyboard)FindResource("SlideInSidebar");
            panelStoryboard.Begin();

            // Ждём завершения анимации панели
            await Task.Delay(500);

            // Затем анимация полоски
            var lineStoryboard = (Storyboard)FindResource("GrowVerticalLine");
            lineStoryboard.Begin();

        }

        private void ManagerMainFrame_Navigated(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
            //UpdateWidthMainNavBarBorder();

            if (e.Content is WarehousesPage && e.Content is ISearchable)
            {
                _warehousesPage.RefreshWarehouses();
            }
            else if (e.Content is PartnersPage && e.Content is ISearchable)
            {
                _partnersPage.RefreshPartners();
                App.mainStackPanel.IsEnabled = true;

                SetupNavigationPanel(_stackPanel, _border);
            }
            else if (!(e.Content is AddEditWarehousePage))
            {
                App.mainStackPanel.Children.Clear();
                App.mainStackPanelBorder.Visibility = Visibility.Collapsed;
            }
        }

        public void EnableSearch() { }

        public void PerformSearch(string query)
        {
            if (ManagerMainFrame?.Content is ISearchable searchablePage)
            {
                searchablePage.PerformSearch(query);
            }
            else
            {
                CustomMessageBox.Show("Поиск недоступен на этой странице.", "Информация", showCancel: false);
            }
        }

        // Кнопка "Склады"
        private void warehouses_Click(object sender, RoutedEventArgs e)
        {
            ManagerMainFrame.Navigate(_warehousesPage);
        }

        // Кнопка "Партнеры"
        private void partners_Click(object sender, RoutedEventArgs e)
        {
            ManagerMainFrame.Navigate(_partnersPage);
            //ShowPartnersNavBar();
        }

        /*private void ShowPartnersNavBar()
        {

            _stackPanel.Children.Add(CreateNavLabel("Добавить партнера", AddPartner_Click));
            _stackPanel.Children.Add(CreateNavLabel("Удалить партнера", DeletePartner_Click));
        }*/
        // Реализация метода интерфейса INavigationPanelHost для настройки панели навигации для партнёров
        public void SetupNavigationPanel(StackPanel panel, Border border)
        {
            panel.Children.Clear();

            // Добавление метки для добавления партнёра
            var addLabel = new Label { Content = "Добавить", Style = (Style)Application.Current.FindResource("menuLabel"), Margin = new Thickness(5), Cursor = Cursors.Hand };
            addLabel.MouseLeftButtonUp += (s, e) =>
            {
                _stackPanel.IsEnabled = false;
                ManagerMainFrame.Navigate(new AddEditPartnerPage());
            };

            // Добавление метки для удаления партнёра
            var deleteLabel = new Label { Content = "Удалить", Style = (Style)Application.Current.FindResource("menuLabel"), Margin = new Thickness(5), Cursor = Cursors.Hand };
            deleteLabel.MouseLeftButtonUp += (s, e) => DeleteSelectedPartner();

            panel.Children.Add(addLabel);
            panel.Children.Add(deleteLabel);
        }

        // Обработчик удаления партнёра
        private void DeleteSelectedPartner()
        {
            _partnersPage.DeleteSelectedPartner();
        }

        // Обработчик добавления партнёра
        private void AddPartner_Click(object sender, MouseButtonEventArgs e)
        {
            App.mainStackPanel.IsEnabled = false;
            ManagerMainFrame.Navigate(new AddEditPartnerPage());
        }

        // Обработчик удаления партнёра
        private void DeletePartner_Click(object sender, MouseButtonEventArgs e)
        {
            _partnersPage.DeleteSelectedPartner();
        }
    }

}
