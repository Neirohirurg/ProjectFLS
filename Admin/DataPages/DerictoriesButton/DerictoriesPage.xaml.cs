using ProjectFLS.Interfaces;
using ProjectFLS.UI;
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
using ProjectFLS.Admin.DataPages.DerictoriesButton.Derictories;

namespace ProjectFLS.Admin.DataPages.DerictoriesButton
{
    /// <summary>
    /// Логика взаимодействия для DerictoriesPage.xaml
    /// </summary>
    public partial class DerictoriesPage : Page, ISearchable
    {
        private StackPanel _stackpanel;
        private Border _stackpanelBorder;
        private Frame _AdminMainFrame;
        public DerictoriesPage(Frame adminMainFrame)
        {
            InitializeComponent();

            _stackpanel = App.mainStackPanel;
            _stackpanelBorder = App.mainStackPanelBorder;
            _AdminMainFrame = adminMainFrame;
        }



        public void EnableSearch()
        {
            // Включить поисковую строку — ничего не нужно, если не требуется поведение при активации
        }

        public void PerformSearch(string query)
        {
            query = query.ToLower().Trim();

            var buttons = new List<Button>
            {
                RolesButton,
                UserStatusesButton,
                TransportButton,
                TransportTypesButton,
                FuelTypesButton,
                DeliveryStatusesButton,
                PartsButton,
                RoutesButton,
                DeliveriesButton,
                TractorTypesButton,
                CitiesButton,
                WarehousesButton,
                PartnersButton,
                DeliveryCostsButton,
                FuelCostsButton
            };

            // Сбросить видимость всех кнопок
            foreach (var btn in buttons)
            {
                btn.Visibility = Visibility.Visible;
                btn.ClearValue(Button.BorderBrushProperty);
                btn.ClearValue(Button.BorderThicknessProperty);
            }

            if (string.IsNullOrWhiteSpace(query))
                return;

            var matched = buttons
                .Where(b => b.Content.ToString().ToLower().Contains(query))
                .ToList();

            // Скрыть неподходящие
            foreach (var btn in buttons.Except(matched))
            {
                btn.Visibility = Visibility.Collapsed;
            }

            // Выделить и фокус на первой
            if (matched.Any())
            {
                matched[0].BorderBrush = Brushes.DodgerBlue;
                matched[0].BorderThickness = new Thickness(2);
            }
        }

        public void SearchSubmit()
        {
            var buttons = new List<Button>
            {
                RolesButton,
                UserStatusesButton,
                TransportButton,
                TransportTypesButton,
                FuelTypesButton,
                DeliveryStatusesButton,
                PartsButton,
                RoutesButton,
                DeliveriesButton,
                TractorTypesButton,
                CitiesButton,
                WarehousesButton,
                PartnersButton,
                DeliveryCostsButton,
                FuelCostsButton
            };

            // Найти первую видимую кнопку (она уже прошла фильтрацию в PerformSearch)
            var firstVisible = buttons.FirstOrDefault(b => b.Visibility == Visibility.Visible);

            // Сымитировать клик
            if (firstVisible != null)
            {
                firstVisible.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
                
            }
        }

        private void RolesButton_Click(object sender, RoutedEventArgs e)
        {
            _AdminMainFrame.Navigate(new RolesPage());
        }

        private void UserStatusesButton_Click(object sender, RoutedEventArgs e)
        {
            _AdminMainFrame.Navigate(new UserStatusesPage());
        }

        private void TransportButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void TransportTypesButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void FuelTypesButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void DeliveryStatusesButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void PartsButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void RoutesButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void DeliveriesButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void TractorTypesButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void CitiesButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void WarehousesButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void PartnersButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void DeliveryCostsButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void FuelCostsButton_Click(object sender, RoutedEventArgs e)
        {

        }

    }
}
