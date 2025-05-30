using ProjectFLS.Models;
using ProjectFLS.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ProjectFLS.UI;

namespace ProjectFLS.Manager.DataPages
{
    public partial class InWarehouse : Page, ISearchable, INavigationPanelHost
    {
        private List<WarehouseTractorInfo> _tractorsInfo;
        private string _currentSortField = null;
        private bool _sortAscending = true;
        private StackPanel _stackpanel;
        private Frame _managerMainFrame;
        private Border _mainBorder;


        public InWarehouse(int warehouseID, List<WarehouseTractorInfo> tractorsInfo)
        {
            InitializeComponent();
            _tractorsInfo = tractorsInfo;
            App.CurrentWareHouseID = warehouseID;

            _stackpanel = App.mainStackPanel;
            
            _mainBorder = App.mainStackPanelBorder;
            DisplayWarehouseInfo(warehouseID, tractorsInfo);
        }

        public InWarehouse(Frame managerMainFrame)
        {
            InitializeComponent();
            _stackpanel = App.mainStackPanel;

            _mainBorder = App.mainStackPanelBorder;
            _managerMainFrame = managerMainFrame;
        }

        // Метод для отображения информации о складе
        private void DisplayWarehouseInfo(int warehouseID, List<WarehouseTractorInfo> tractorsInfo)
        {
            WarehouseTractorsListView.ItemsSource = tractorsInfo;
        }

        // Реализация метода интерфейса ISearchable
        public void PerformSearch(string query)
        {
            query = query?.ToLowerInvariant() ?? "";

            // Поиск по всем полям
            var filteredTractors = _tractorsInfo.Where(t =>
                (t.Model != null && t.Model.ToLower().Contains(query)) ||
                (t.Quantity.ToString().ToLower().Contains(query)) ||
                (t.LengthM.ToString().ToLower().Contains(query)) ||
                (t.WidthM.ToString().ToLower().Contains(query)) ||
                (t.EnginePowerHP.ToString().ToLower().Contains(query))
            ).ToList();

            WarehouseTractorsListView.ItemsSource = filteredTractors;
        }

        public void EnableSearch()
        {
            // Может быть реализовано, если требуется включение поиска
        }

        // Метод сортировки
        private void Sort_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is string field)
            {
                if (_currentSortField == field)
                    _sortAscending = !_sortAscending;
                else
                {
                    _currentSortField = field;
                    _sortAscending = true;
                }

                ApplySorting();
            }
        }

        // Применение сортировки
        // Применение сортировки
        private void ApplySorting()
        {
            // Проверяем, какой тип данных используется для сортировки
            var sortedData = _sortAscending
                ? _tractorsInfo.OrderBy(t =>
                {
                    var value = GetFieldValue(t, _currentSortField);
                    if (value is IComparable comparable)
                    {
                        return comparable;
                    }
                    return value?.ToString(); // Для строковых полей
                }).ToList()
                : _tractorsInfo.OrderByDescending(t =>
                {
                    var value = GetFieldValue(t, _currentSortField);
                    if (value is IComparable comparable)
                    {
                        return comparable;
                    }
                    return value?.ToString(); // Для строковых полей
                }).ToList();

            // Обновляем привязку данных
            WarehouseTractorsListView.ItemsSource = sortedData;
        }


        // Получение значения по полю
        private object GetFieldValue(WarehouseTractorInfo item, string field)
        {
            switch (field)
            {
                case "Model":
                    return item.Model;
                case "Quantity":
                    return item.Quantity;
                case "LengthM":
                    return item.LengthM;
                case "WidthM":
                    return item.WidthM;
                case "EnginePowerHP":
                    return item.EnginePowerHP;
                default:
                    return null;
            }
        }


        // Обработка кнопки "Назад"
        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            if (NavigationService.CanGoBack)
            {
                NavigationService.GoBack();
            }
            else
            {
                MessageBox.Show("Не удается вернуться назад.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Метод, выполняемый при загрузке страницы
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            _mainBorder.Visibility = Visibility.Visible;

        }

        // Реализация интерфейса INavigationPanelHost
        public void SetupNavigationPanel(StackPanel panel, Border border)
        {
            panel.Children.Clear();



            var addLabel = new Label { Content = "Оформить доставку", Style = (Style)Application.Current.FindResource("menuLabel"), Margin = new Thickness(5), Cursor = Cursors.Hand };
            addLabel.MouseLeftButtonUp += (s, e) =>
            {
                _stackpanel.IsEnabled = false;
                _managerMainFrame.Navigate(new AddDeliveryPage());
            };

            var activeLabel = new Label { Content = "Активные доставки", Style = (Style)Application.Current.FindResource("menuLabel"), Margin = new Thickness(5), Cursor = Cursors.Hand };
            activeLabel.MouseLeftButtonUp += (s, e) =>
            {


                _stackpanel.IsEnabled = false;
                _managerMainFrame.Navigate(new ActiveDeliveryPage(_managerMainFrame));
            };

            /*var deleteLabel = new Label { Content = "Удалить", Style = (Style)Application.Current.FindResource("menuLabel"), Margin = new Thickness(5), Cursor = Cursors.Hand };
            deleteLabel.MouseLeftButtonUp += (s, e) => DeleteTractor();*/

            panel.Children.Add(addLabel);
            panel.Children.Add(activeLabel);
        }

        /*public async Task DeleteSelectedAsync()
        {
            if (WarehouseTractorsListView.SelectedItem is WarehouseTractorInfo selectedTractor)
            {
                int tractorID = selectedTractor.TractorID;
                bool? result = CustomMessageBox.Show($"Удалить трактор с ID = {tractorID}?", "Подтверждение удаления", showCancel: true);
                if (result == true)
                {
                    try
                    {
                        await Task.Run(() => DeleteTractor(tractorID));
                        await RefreshAsync();
                        CustomMessageBox.Show("Успешно удалено.", "Удаление", showCancel: false);
                    }
                    catch (Exception ex)
                    {
                        CustomMessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", showCancel: false);
                    }
                }
            }
        }*/

        public async Task RefreshAsync()
        {
            var list = await Task.Run(() => _tractorsInfo);
            WarehouseTractorsListView.ItemsSource = list.ToList();
        }

        private void DeleteTractor(int tractorID)
        {
            // Логика удаления трактора
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            _mainBorder.Visibility = Visibility.Collapsed;
        }
    }
}
