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
using System.Collections.ObjectModel;

namespace ProjectFLS.Logist.Applications
{
    public partial class ApplicationsPage : Page, ISearchable
    {
        private ObservableCollection<ActiveDeliveryInfo> _activeDeliveries;
        private string _currentSortField = null;
        private bool _sortAscending = true;
        private Border _mainBorder;
        private Frame _logistFrame;

        private deliveries1TableAdapter _deliveriesAdapter;
        private deliveryTypesTableAdapter _deliveryTypesAdapter;
        private warehousesTableAdapter _warehousesAdapter;
        private dealersTableAdapter _dealersAdapter;
        private transportTableAdapter _transportAdapter;
        private deliveryStatusesTableAdapter _deliveryStatusesAdapter;

        public ApplicationsPage(Frame logistFrame)
        {
            InitializeComponent();

            _mainBorder = App.mainStackPanelBorder;
            _logistFrame = logistFrame;
            _deliveriesAdapter = new deliveries1TableAdapter();
            _deliveryTypesAdapter = new deliveryTypesTableAdapter();
            _warehousesAdapter = new warehousesTableAdapter();
            _dealersAdapter = new dealersTableAdapter();
            _transportAdapter = new transportTableAdapter();
            _deliveryStatusesAdapter = new deliveryStatusesTableAdapter();

            _activeDeliveries = new ObservableCollection<ActiveDeliveryInfo>();
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadDeliveriesAsync();


        }

        private async Task LoadDeliveriesAsync()
        {
            try
            {
                var deliveries = await Task.Run(() => _deliveriesAdapter.GetData());
                var deliveryTypes = await Task.Run(() => _deliveryTypesAdapter.GetData());
                var warehouses = await Task.Run(() => _warehousesAdapter.GetData());
                var dealers = await Task.Run(() => _dealersAdapter.GetData());
                var transport = await Task.Run(() => _transportAdapter.GetData());
                var deliveryStatuses = await Task.Run(() => _deliveryStatusesAdapter.GetData());

                _activeDeliveries.Clear();

                foreach (var delivery in deliveries)
                {
                    var deliveryType = deliveryTypes.FirstOrDefault(dt => dt.deliveryTypeID == delivery.deliveryTypeID);
                    var fromWarehouse = warehouses.FirstOrDefault(w => w.warehouseID == delivery.fromWarehouseID);
                    var toWarehouse = warehouses.FirstOrDefault(w => w.warehouseID == delivery.toWarehouseID);
                    var toDealer = dealers.FirstOrDefault(d => d.dealerID == delivery.toDealerID);
                    var transportDetails = transport.FirstOrDefault(t => t.transportID == delivery.transportID);
                    var deliveryStatus = deliveryStatuses.FirstOrDefault(ds => ds.statusID == delivery.statusID);

                    if (deliveryStatus?.statusName == "Ожидает оформления")
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

            // Фильтрация данных на основе введенного запроса
            var filteredDeliveries = _activeDeliveries.Where(d =>
                (d.DeliveryTypeName?.ToLower().Contains(query) ?? false) ||
                (d.FromWarehouseName?.ToLower().Contains(query) ?? false) ||
                (d.ToWarehouseName?.ToLower().Contains(query) ?? false) ||
                (d.ToDealerName?.ToLower().Contains(query) ?? false) ||
                (d.TransportName?.ToLower().Contains(query) ?? false) ||
                (d.StatusName?.ToLower().Contains(query) ?? false)
            ).ToList();

            // Обновляем привязку, если фильтр изменился
            ActiveDeliveryListView.ItemsSource = new ObservableCollection<ActiveDeliveryInfo>(filteredDeliveries);
        }


        private async Task RefreshDataAsync()
{
    try
    {
        // Получаем обновленные данные из базы данных
        var deliveries = await Task.Run(() => _deliveriesAdapter.GetData());
        var deliveryTypes = await Task.Run(() => _deliveryTypesAdapter.GetData());
        var warehouses = await Task.Run(() => _warehousesAdapter.GetData());
        var dealers = await Task.Run(() => _dealersAdapter.GetData());
        var transport = await Task.Run(() => _transportAdapter.GetData());
        var deliveryStatuses = await Task.Run(() => _deliveryStatusesAdapter.GetData());

        // Очищаем текущие данные
        _activeDeliveries.Clear();

        // Заполняем обновленными данными
        foreach (var delivery in deliveries)
        {
            var deliveryType = deliveryTypes.FirstOrDefault(dt => dt.deliveryTypeID == delivery.deliveryTypeID);
            var fromWarehouse = warehouses.FirstOrDefault(w => w.warehouseID == delivery.fromWarehouseID);
            var toWarehouse = warehouses.FirstOrDefault(w => w.warehouseID == delivery.toWarehouseID);
            var toDealer = dealers.FirstOrDefault(d => d.dealerID == delivery.toDealerID);
            var transportDetails = transport.FirstOrDefault(t => t.transportID == delivery.transportID);
            var deliveryStatus = deliveryStatuses.FirstOrDefault(ds => ds.statusID == delivery.statusID);

            // Добавляем только те доставки, которые в статусе "Ожидает оформления"
            if (deliveryStatus?.statusName == "Ожидает оформления")
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

        // Обновляем привязку данных
        ActiveDeliveryListView.ItemsSource = new ObservableCollection<ActiveDeliveryInfo>(_activeDeliveries);

        // Применяем текущий запрос поиска, если он был
        if (!string.IsNullOrEmpty(SearchQuery))
        {
            PerformSearch(SearchQuery);
        }
    }
    catch (Exception ex)
    {
        MessageBox.Show($"Ошибка при обновлении данных: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
    }
}
        private string SearchQuery { get; set; }
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

            ActiveDeliveryListView.ItemsSource = new ObservableCollection<ActiveDeliveryInfo>(sortedData);
        }

        private object GetFieldValue(ActiveDeliveryInfo delivery, string field)
        {
            switch (field)
            {
                case "DeliveryType":
                    return delivery.DeliveryTypeName;
                case "FromWarehouse":
                    return delivery.FromWarehouseName;
                case "ToWarehouse":
                    return delivery.ToWarehouseName;
                case "ToDealer":
                    return delivery.ToDealerName;
                case "Transport":
                    return delivery.TransportName;
                case "Status":
                    return delivery.StatusName;
                default:
                    return null;
            }
        }

        private async void ApproveButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedDelivery = ActiveDeliveryListView.SelectedItem as ActiveDeliveryInfo;
            if (selectedDelivery != null)
            {
                selectedDelivery.StatusName = "В пути";

                var status = (await Task.Run(() => _deliveryStatusesAdapter.GetData()))
                    .FirstOrDefault(s => s.statusName == "В пути");

                if (status != null)
                {
                    int result = _deliveriesAdapter.UpdateStatus(status.statusID, selectedDelivery.DeliveryID);
                    if (result > 0)
                    {
                        CustomMessageBox.Show("Доставка подтверждена", "Подтверждение", showCancel: false);
                        await RefreshDataAsync();
                    }
                    else
                    {
                        MessageBox.Show("Ошибка при обновлении статуса", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else
                {
                    MessageBox.Show("Статус 'На складе' не найден!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }

            }
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            _mainBorder.Visibility = Visibility.Collapsed;
        }
    }
}
