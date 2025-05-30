using ProjectFLS.Models;
using ProjectFLS.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using ProjectFLS.flsdbDataSetTableAdapters;
using System.Threading.Tasks;
using System.Windows.Media;
using ProjectFLS.UI;

namespace ProjectFLS.Manager.DataPages
{
    public partial class ActiveDeliveryPage : Page, ISearchable
    {
        private List<ActiveDeliveryInfo> _activeDeliveries;
        private string _currentSortField = null;
        private bool _sortAscending = true;
        private Frame _managerMainFrame;
        private Border _mainBorder;

        private deliveries1TableAdapter _deliveriesAdapter;
        private deliveryTypesTableAdapter _deliveryTypesAdapter;
        private warehousesTableAdapter _warehousesAdapter;
        private dealersTableAdapter _dealersAdapter;
        private transportTableAdapter _transportAdapter;
        private deliveryStatusesTableAdapter _deliveryStatusesAdapter;

        public ActiveDeliveryPage(Frame managerMainFrame)
        {
            InitializeComponent();
            _managerMainFrame = managerMainFrame;
            _mainBorder = App.mainStackPanelBorder;

            _deliveriesAdapter = new deliveries1TableAdapter();
            _deliveryTypesAdapter = new deliveryTypesTableAdapter();
            _warehousesAdapter = new warehousesTableAdapter();
            _dealersAdapter = new dealersTableAdapter();
            _transportAdapter = new transportTableAdapter();
            _deliveryStatusesAdapter = new deliveryStatusesTableAdapter();

            _activeDeliveries = new List<ActiveDeliveryInfo>();
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            _mainBorder.Visibility = Visibility.Visible;
            await LoadDeliveriesAsync();
            UpdateButtonVisibility();
        }

        private async Task LoadDeliveriesAsync()
        {
            try
            {
                var deliveries = _deliveriesAdapter.GetData();
                var deliveryTypes = _deliveryTypesAdapter.GetData();
                var warehouses = _warehousesAdapter.GetData();
                var dealers = _dealersAdapter.GetData();
                var transport = _transportAdapter.GetData();
                var deliveryStatuses = _deliveryStatusesAdapter.GetData();

                _activeDeliveries.Clear();

                foreach (var delivery in deliveries)
                {
                    var deliveryType = deliveryTypes.FirstOrDefault(dt => dt.deliveryTypeID == delivery.deliveryTypeID);
                    var fromWarehouse = warehouses.FirstOrDefault(w => w.warehouseID == delivery.fromWarehouseID);
                    var toWarehouse = warehouses.FirstOrDefault(w => w.warehouseID == delivery.toWarehouseID);
                    var toDealer = dealers.FirstOrDefault(d => d.dealerID == delivery.toDealerID);
                    var transportDetails = transport.FirstOrDefault(t => t.transportID == delivery.transportID);
                    var deliveryStatus = deliveryStatuses.FirstOrDefault(ds => ds.statusID == delivery.statusID);

                    // Фильтрация по текущему складу
                    if (fromWarehouse != null && (fromWarehouse.warehouseID == App.CurrentWareHouseID || toWarehouse?.warehouseID == App.CurrentWareHouseID))
                    {
                        var activeDelivery = new ActiveDeliveryInfo
                        {
                            DeliveryTypeName = deliveryType?.deliveryTypeName,
                            FromWarehouseName = fromWarehouse?.warehouseName,
                            ToWarehouseName = toWarehouse?.warehouseName,
                            ToDealerName = toDealer?.dealerName,
                            TransportName = transportDetails?.transportName,
                            StatusName = deliveryStatus?.statusName,
                            DeliveryDate = delivery.deliveryDate,
                            DeliveryID = delivery.deliveryID,
                            FromWarehouseID = fromWarehouse?.warehouseID ?? 0,
                            ToWarehouseID = toWarehouse?.warehouseID ?? 0
                        };

                        _activeDeliveries.Add(activeDelivery);
                    }
                }

                ActiveDeliveryListView.ItemsSource = _activeDeliveries;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке данных: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void EnableSearch() { }

        public void PerformSearch(string query)
        {
            query = query?.ToLowerInvariant() ?? "";

            // Поиск по всем полям
            var filteredDeliveries = _activeDeliveries.Where(d =>
                (d.DeliveryTypeName != null && d.DeliveryTypeName.ToLower().Contains(query)) ||
                (d.FromWarehouseName != null && d.FromWarehouseName.ToLower().Contains(query)) ||
                (d.ToWarehouseName != null && d.ToWarehouseName.ToLower().Contains(query)) ||
                (d.ToDealerName != null && d.ToDealerName.ToLower().Contains(query)) ||
                (d.TransportName != null && d.TransportName.ToLower().Contains(query)) ||
                (d.StatusName != null && d.StatusName.ToLower().Contains(query))
            ).ToList();

            ActiveDeliveryListView.ItemsSource = filteredDeliveries;
        }

        private void UpdateButtonVisibility()
        {
            var selectedDelivery = ActiveDeliveryListView.SelectedItem as ActiveDeliveryInfo;

            if (selectedDelivery == null)
            {
                // Если элемент не выбран, отключаем все кнопки и меняем их внешний вид
                ApproovButton.IsEnabled = false;
                SentButton.IsEnabled = false;
                ApproovButton.Foreground = new SolidColorBrush(Color.FromArgb(255, 221, 221, 221)); // #DDD
                SentButton.Foreground = new SolidColorBrush(Color.FromArgb(255, 221, 221, 221)); // #DDD
                ApproovButton.Opacity = 0.5; // Делаем кнопку полупрозрачной
                SentButton.Opacity = 0.5; // Делаем кнопку полупрозрачной
                return;
            }

            // Проверка, если склад назначения = текущий склад, а статус "В пути"
            if (selectedDelivery.StatusName == "В пути" && selectedDelivery.ToWarehouseID == App.CurrentWareHouseID)
            {
                ApproovButton.IsEnabled = true; // Разрешаем кнопку подтверждения
                SentButton.IsEnabled = false;  // Отключаем кнопку отправки
                ApproovButton.Foreground = new SolidColorBrush(Colors.Black); // Черный цвет текста
                SentButton.Foreground = new SolidColorBrush(Color.FromArgb(255, 221, 221, 221)); // #DDD
                ApproovButton.Opacity = 1; // Кнопка полностью видимая
                SentButton.Opacity = 0.5; // Полупрозрачная кнопка
            }

            else if (selectedDelivery.StatusName == "На складе" && selectedDelivery.ToWarehouseID == App.CurrentWareHouseID)
            {
                ApproovButton.IsEnabled = false; // Разрешаем кнопку подтверждения
                SentButton.IsEnabled = true;  // Отключаем кнопку отправки
                SentButton.Foreground = new SolidColorBrush(Colors.Black); // Черный цвет текста
                ApproovButton.Foreground = new SolidColorBrush(Color.FromArgb(255, 221, 221, 221)); // #DDD
                ApproovButton.Opacity = 1; // Кнопка полностью видимая
                SentButton.Opacity = 0.5; // Полупрозрачная кнопка
            }
            // Если склад назначения = текущий склад, а статус "Ожидание"
            else if (selectedDelivery.StatusName == "Ожидает отправления" && selectedDelivery.FromWarehouseID == App.CurrentWareHouseID)
            {
                ApproovButton.IsEnabled = false;  // Отключаем кнопку подтверждения
                SentButton.IsEnabled = true;     // Разрешаем кнопку отправки
                ApproovButton.Foreground = new SolidColorBrush(Color.FromArgb(255, 221, 221, 221)); // #DDD
                SentButton.Foreground = new SolidColorBrush(Colors.Black); // Черный цвет текста
                ApproovButton.Opacity = 0.5; // Полупрозрачная кнопка
                SentButton.Opacity = 1; // Кнопка полностью видимая
            }
            else
            {
                ApproovButton.IsEnabled = false;
                SentButton.IsEnabled = false;
                ApproovButton.Foreground = new SolidColorBrush(Color.FromArgb(255, 221, 221, 221)); // #DDD
                SentButton.Foreground = new SolidColorBrush(Color.FromArgb(255, 221, 221, 221)); // #DDD
                ApproovButton.Opacity = 0.5; // Полупрозрачная кнопка
                SentButton.Opacity = 0.5; // Полупрозрачная кнопка
            }
        }

       


        // Обработчик события выбора элемента в ListView
        private void ActiveDeliveryListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateButtonVisibility(); // Обновляем видимость кнопок при изменении выбора
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

        private void ApplySorting()
        {
            var sortedData = _sortAscending
                ? _activeDeliveries.OrderBy(d => GetFieldValue(d, _currentSortField)).ToList()
                : _activeDeliveries.OrderByDescending(d => GetFieldValue(d, _currentSortField)).ToList();

            ActiveDeliveryListView.ItemsSource = sortedData;
        }

        private object GetFieldValue(ActiveDeliveryInfo delivery, string field)
        {
            switch (field)
            {
                case "DeliveryTypeName":
                    return delivery.DeliveryTypeName;
                case "FromWarehouseName":
                    return delivery.FromWarehouseName;
                case "ToWarehouseName":
                    return delivery.ToWarehouseName;
                case "ToDealerName":
                    return delivery.ToDealerName;
                case "TransportName":
                    return delivery.TransportName;
                case "StatusName":
                    return delivery.StatusName;
                default:
                    return null;
            }
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            _mainBorder.Visibility = Visibility.Collapsed;
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }

        private void ApproovButton_Click(object sender, RoutedEventArgs e)
        {
            // Обновление статуса на "На складе"
            var selectedDelivery = ActiveDeliveryListView.SelectedItem as ActiveDeliveryInfo;
            if (selectedDelivery != null)
            {
                // Проверяем, что это нужная доставка
                selectedDelivery.StatusName = "Ожидает отправления";  // Обновление состояния в списке

                // Получаем ID статуса "На складе" из базы данных
                var status = _deliveryStatusesAdapter.GetData()
                    .FirstOrDefault(s => s.statusName == "На складе");

                if (status != null)
                {
                    // Обновление статуса в базе данных
                    _deliveriesAdapter.UpdateStatus(status.statusID, selectedDelivery.DeliveryID);

                    CustomMessageBox.Show("Доставка подтверждена", "Подтверждение", showCancel: false);
                }
                else
                {
                    MessageBox.Show("Статус не найден!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }

                UpdateButtonVisibility();
            }
        }

        private void SentButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedDelivery = ActiveDeliveryListView.SelectedItem as ActiveDeliveryInfo;
            if (selectedDelivery != null)
            {
                selectedDelivery.StatusName = "Отправлен к дилеру";  // Обновление состояния в списке

                var status = _deliveryStatusesAdapter.GetData()
                    .FirstOrDefault(s => s.statusName == "Отправлен к дилеру");

                if (status != null)
                {
                    // Обновление статуса в базе данных
                    int result = _deliveriesAdapter.UpdateStatus(status.statusID, selectedDelivery.DeliveryID);

                    if (result > 0)
                    {
                        ActiveDeliveryListView.Items.Refresh();  // Обновление данных в ListView
                        CustomMessageBox.Show("Доставка отправлена к дилеру", "Подтверждение", showCancel: false);
                    }
                    else
                    {
                        MessageBox.Show("Ошибка при обновлении статуса!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else
                {
                    MessageBox.Show("Статус 'Отправлен к дилеру' не найден!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Выберите доставку для отправки.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            UpdateButtonVisibility();
        }



    }
}
