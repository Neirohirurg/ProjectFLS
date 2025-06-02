using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ProjectFLS.flsdbDataSetTableAdapters;
using ProjectFLS.Interfaces;
using ProjectFLS.Models;

namespace ProjectFLS.Manager.DataPages
{
    partial class WarehousesPage : Page, ISearchable//, INavigationPanelHost
    {
        private warehousesTableAdapter _warehousesAdapter;
        private int CurrentUserID = App.CurrentUserId;
        private Frame _managerMainFrame;

        public WarehousesPage(Frame managerFrame)
        {
            InitializeComponent();
            _warehousesAdapter = new warehousesTableAdapter();
            _managerMainFrame = managerFrame;
            LoadWarehouses();

        }

        /*public void SetupNavigationPanel(StackPanel panel, Border border)
        {
            panel.Children.Clear();

            // Добавление метки для добавления партнёра
            var addLabel = new Label { Content = "Добавить поставку", Style = (Style)Application.Current.FindResource("menuLabel"), Margin = new Thickness(5), Cursor = Cursors.Hand };
            addLabel.MouseLeftButtonUp += (s, e) =>
            {
                App.mainStackPanel.IsEnabled = false;
                _managerMainFrame.Navigate(new AddDeliveryPage(this._managerMainFrame));
            };

            *//* // Добавление метки для удаления партнёра
             var deleteLabel = new Label { Content = "Удалить", Style = (Style)Aplication.Current.FindResource("menuLabel"), Margin = new Thickness(5), Cursor = Cursors.Hand };
             deleteLabel.MouseLeftButtonUp += (s, e) => DeleteSelectedPartner();*//*

            panel.Children.Add(addLabel);
            //panel.Children.Add(deleteLabel);
        }*/

        public void RefreshWarehouses() { }

        public void EnableSearch() { }

        public void PerformSearch(string query)
        {
            query = query?.ToLowerInvariant() ?? "";

            try
            {
                // Получаем список складов для текущего пользователя
                var warehousesData = _warehousesAdapter.GetWarehousesByUserId(CurrentUserID);

                // Фильтруем склады по имени склада (поиск по части имени)
                var filteredWarehouses = warehousesData
                    .Where(warehouse => warehouse.warehouseName.ToLowerInvariant().Contains(query))
                    .ToList();

                // Очищаем панель от старых кнопок
                WarehousesPanel.Children.Clear();

                // Динамически создаём кнопки для отфильтрованных складов
                foreach (var warehouse in filteredWarehouses)
                {
                    var warehouseButton = new Button
                    {
                        Content = warehouse.warehouseName, // Название склада
                        Tag = warehouse.warehouseID, // Для получения ID склада
                        Style = (Style)FindResource("RoundButtonStyleBig"),
                        Margin = new Thickness(10)
                    };

                    // Подключаем обработчик события нажатия
                    warehouseButton.Click += WarehouseButton_Click;

                    // Добавляем кнопку в панель
                    WarehousesPanel.Children.Add(warehouseButton);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при поиске складов: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Загрузка складов из базы данных и создание кнопок
        private async void LoadWarehouses()
        {
            try
            {
                App.mainStackPanelBorder.Visibility = Visibility.Collapsed;

                // Загрузим данные о складах для текущего пользователя
                var warehousesData = await Task.Run(() => _warehousesAdapter.GetWarehousesByUserId(CurrentUserID));

                // Очистим панель от старых кнопок
                WarehousesPanel.Children.Clear();

                // Динамически создаём кнопки для каждого склада
                foreach (var warehouse in warehousesData)
                {
                    var warehouseButton = new Button
                    {
                        Content = warehouse.warehouseName, // Название склада
                        Tag = warehouse.warehouseID, // Для получения ID склада
                        Style = (Style)FindResource("RoundButtonStyleBig"),
                        Margin = new Thickness(10)
                    };

                    // Подключаем обработчик события нажатия
                    warehouseButton.Click += WarehouseButton_Click;

                    // Добавляем кнопку в панель
                    WarehousesPanel.Children.Add(warehouseButton);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке складов: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Обработчик нажатия на кнопку склада
        private async void WarehouseButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button warehouseButton)
            {
                int warehouseID = (int)warehouseButton.Tag;

                // Открытие информации по складу (включая информацию о тракторах)
                await OpenWarehouseInfoAsync(warehouseID);
            }
        }

        // Асинхронная загрузка информации по складу и тракторным единицам
        private async Task OpenWarehouseInfoAsync(int warehouseID)
        {
            try
            {
                var warehouseTractorData = new WarehouseTractorData();
                var tractorsInfo = await warehouseTractorData.GetWarehouseTractorsInfoAsync(warehouseID);

                // Открываем окно с информацией о тракторных единицах на складе
                var inWarehousePage = new InWarehouse(warehouseID, tractorsInfo);
                _managerMainFrame.Navigate(inWarehousePage);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке информации о складе: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {

        }
    }

}
