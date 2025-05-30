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
    public partial class AddEditDealerPage : Page, ISearchable
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

        private ActiveDeliveryInfo selectedDelivery;
        public AddEditDealerPage(Frame managerMainFrame)
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

                // Фильтрация доставок по DealerID
                foreach (var delivery in deliveries)
                {
                    // Пропускаем доставки, которые не принадлежат выбранному дилеру
                    if (delivery.toDealerID != App._selectedPartner.DealerID)
                        continue;

                    var deliveryType = deliveryTypes.FirstOrDefault(dt => dt.deliveryTypeID == delivery.deliveryTypeID);
                    var fromWarehouse = warehouses.FirstOrDefault(w => w.warehouseID == delivery.fromWarehouseID);
                    var toWarehouse = warehouses.FirstOrDefault(w => w.warehouseID == delivery.toWarehouseID);
                    var toDealer = dealers.FirstOrDefault(d => d.dealerID == delivery.toDealerID);
                    var transportDetails = transport.FirstOrDefault(t => t.transportID == delivery.transportID);
                    var deliveryStatus = deliveryStatuses.FirstOrDefault(ds => ds.statusID == delivery.statusID);

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

                // Обновляем источник данных для ListView
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
            try
            {
                DisableApproovButton();
                switch (selectedDelivery.StatusName)
                {
                    case "Отправлен к дилеру":
                        EnableApproovButton();
                        break;
                    case "Доставлено":
                        DisableApproovButton();  // Если статус уже "Доставлено" или "Отправлен к дилеру", отключаем кнопку
                        break;
                    default:
                        DisableApproovButton();  // В остальных случаях, включаем кнопку
                        break;
                }
            }
            catch (Exception ex)
            {
                // Если произошла ошибка при обновлении видимости кнопок
                Console.WriteLine($"Ошибка: {ex.Message}");
                DisableApproovButton();  // Отключаем кнопку в случае ошибки
            }
        }


        // Метод для отключения кнопки ApproovButton
        // Метод для отключения кнопки ApproovButton
        private void DisableApproovButton()
        {
            ApproovButton.IsEnabled = false;
            ApproovButton.Foreground = new SolidColorBrush(Color.FromArgb(255, 221, 221, 221)); // #DDD
            ApproovButton.Opacity = 0.5; // Полупрозрачная кнопка
        }

        // Метод для активации кнопки ApproovButton
        private void EnableApproovButton()
        {
            ApproovButton.IsEnabled = true;
            ApproovButton.Foreground = new SolidColorBrush(Colors.Black); // Черный цвет текста
            ApproovButton.Opacity = 0.5; // Полностью видимая кнопка
        }


        // Обработчик события выбора элемента в ListView
        private void ActiveDeliveryListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Получаем выбранный элемент
            selectedDelivery = (ActiveDeliveryInfo)ActiveDeliveryListView.SelectedItem;
            Console.WriteLine($"Выбранный элемент: {selectedDelivery?.ToDealerName}");  // Для отладки
            UpdateButtonVisibility();  // Обновляем видимость кнопок при изменении выбора
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


        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }

        // Методы для кнопок действия
        private void ApproovButton_Click(object sender, RoutedEventArgs e)
        {
            // Проверяем, есть ли выбранная доставка
            if (selectedDelivery != null)
            {
                Console.WriteLine($"Обновление статуса для доставки ID: {selectedDelivery.DeliveryID}");

                // Обновление статуса в модели
                selectedDelivery.StatusName = "Доставлено";

                // Получаем статус "Доставлено" из базы данных
                var status = _deliveryStatusesAdapter.GetData()
                    .FirstOrDefault(s => s.statusName == "Доставлено");

                if (status != null)
                {
                    // Обновляем статус в базе данных
                    int result = _deliveriesAdapter.UpdateStatus(status.statusID, selectedDelivery.DeliveryID);

                    if (result > 0)
                    {
                        ActiveDeliveryListView.Items.Refresh();  // Обновляем данные в ListView
                        CustomMessageBox.Show("Доставка доставлена", "Подтверждение", showCancel: false);
                    }
                    else
                    {
                        MessageBox.Show("Ошибка при обновлении статуса!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else
                {
                    MessageBox.Show("Статус 'Доставлено' не найден!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Выберите доставку для отправки.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            UpdateButtonVisibility();  // Обновляем видимость кнопки после выполнения действия
        }


        private async void Page_Loaded_1(object sender, RoutedEventArgs e)
        {
            _mainBorder.Visibility = Visibility.Collapsed;
            await LoadDeliveriesAsync();

        }
    }
}
