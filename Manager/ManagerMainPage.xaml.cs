using ProjectFLS.Models;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using ProjectFLS.Manager.DataPages.WarehousesButton;
using ProjectFLS.Manager.DataPages.PartnersButton;
using System.Threading.Tasks;
using ProjectFLS.Admin.DataPages.DerictoriesButton.Derictories;
using ProjectFLS.Interfaces;
using System.Windows.Input;

namespace ProjectFLS.Manager
{
    public partial class ManagerMainPage : Page
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
            ShowWarehousesNavBar();

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
            if (e.Content is WarehousesPage && e.Content is ISearchable)
            {
                _warehousesPage.RefreshWarehouses();
                App.mainStackPanel.IsEnabled = true;
            }
            else if (e.Content is PartnersPage && e.Content is ISearchable)
            {
                _partnersPage.RefreshPartners();
                App.mainStackPanel.IsEnabled = true;
            }
            else if (!(e.Content is AddEditWarehousePage))
            {
                App.mainStackPanel.Children.Clear();
                App.mainStackPanelBorder.Visibility = Visibility.Collapsed;
            }
        }

        // Кнопка "Склады"
        private void warehouses_Click(object sender, RoutedEventArgs e)
        {
            ManagerMainFrame.Navigate(_warehousesPage);
            ShowWarehousesNavBar();
        }

        // Кнопка "Партнеры"
        private void partners_Click(object sender, RoutedEventArgs e)
        {
            ManagerMainFrame.Navigate(_partnersPage);
            ShowPartnersNavBar();
        }

        // Показывает панель навигации для складов
        private void ShowWarehousesNavBar()
        {
            _stackPanel.Children.Clear();

            _stackPanel.Children.Add(CreateNavLabel("Добавить склад", AddWarehouse_Click));
            _stackPanel.Children.Add(CreateNavLabel("Удалить склад", DeleteWarehouse_Click));
            _stackPanel.Children.Add(CreateNavLabel("Редактировать склад", EditWarehouse_Click));
        }

        // Показывает панель навигации для партнеров
        private void ShowPartnersNavBar()
        {
            _stackPanel.Children.Clear();

            _stackPanel.Children.Add(CreateNavLabel("Добавить партнера", AddPartner_Click));
            _stackPanel.Children.Add(CreateNavLabel("Удалить партнера", DeletePartner_Click));
            _stackPanel.Children.Add(CreateNavLabel("Редактировать партнера", EditPartner_Click));
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

        // Обработчик добавления склада
        private void AddWarehouse_Click(object sender, MouseButtonEventArgs e)
        {
            App.mainStackPanel.IsEnabled = false;
            ManagerMainFrame.Navigate(new AddEditWarehousePage());
        }

        // Обработчик удаления склада
        private void DeleteWarehouse_Click(object sender, MouseButtonEventArgs e)
        {
            _warehousesPage.DeleteSelectedWarehouse();
        }

        // Обработчик редактирования склада
        private void EditWarehouse_Click(object sender, MouseButtonEventArgs e)
        {
            var selectedWarehouse = _warehousesPage.GetSelectedWarehouse();
            if (selectedWarehouse == null)
            {
                MessageBox.Show("Выберите склад для редактирования", "Редактирование", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }

            int warehouseId = selectedWarehouse.WarehouseID;
            ManagerMainFrame.Navigate(new AddEditWarehousePage(warehouseId));
        }

        // Обработчик добавления партнера
        private void AddPartner_Click(object sender, MouseButtonEventArgs e)
        {
            App.mainStackPanel.IsEnabled = false;
            ManagerMainFrame.Navigate(new AddEditPartnerPage());
        }

        // Обработчик удаления партнера
        private void DeletePartner_Click(object sender, MouseButtonEventArgs e)
        {
            _partnersPage.DeleteSelectedPartner();
        }

        // Обработчик редактирования партнера
        private void EditPartner_Click(object sender, MouseButtonEventArgs e)
        {
            var selectedPartner = _partnersPage.GetSelectedPartner();
            if (selectedPartner == null)
            {
                MessageBox.Show("Выберите партнера для редактирования", "Редактирование", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }

            int partnerId = selectedPartner.PartnerID;
            ManagerMainFrame.Navigate(new AddEditPartnerPage(partnerId));
        }
    }
}
