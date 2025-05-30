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
    public partial class AddDeliveryPage : Page
    {
        private deliveries1TableAdapter _deliveriesAdapter;
        private deliveryTypesTableAdapter _deliveryTypesAdapter;
        private warehousesTableAdapter _warehousesAdapter;
        private dealersTableAdapter _dealersAdapter;
        private transportTableAdapter _transportAdapter;
        private deliveryStatusesTableAdapter _deliveryAdapter;
        private tractorUnitsTableAdapter _tractorsAdapter;
        private partsTableAdapter _partsAdapter;

        private List<ActiveDeliveryInfo> _activeDeliveries;

        public AddDeliveryPage()
        {
            InitializeComponent();

            _deliveriesAdapter = new deliveries1TableAdapter();
            _deliveryTypesAdapter = new deliveryTypesTableAdapter();
            _warehousesAdapter = new warehousesTableAdapter();
            _dealersAdapter = new dealersTableAdapter();
            _transportAdapter = new transportTableAdapter();
            _tractorsAdapter = new tractorUnitsTableAdapter();
            _partsAdapter = new partsTableAdapter();

            _activeDeliveries = new List<ActiveDeliveryInfo>();
        }

        private void SetWarehouseData()
        {
            // Получаем текущий склад из App.CurrentWarehouseId
            var currentWarehouse = _warehousesAdapter.GetData()
                                    .FirstOrDefault(w => w.warehouseID == App.CurrentWareHouseID);

            // Если склад найден, заполняем ComboBox
            if (currentWarehouse != null)
            {
                // Создаем список для добавления текущего склада в ComboBox
                var warehouses = new List<object>
        {
            new { warehouseID = currentWarehouse.warehouseID, warehouseName = currentWarehouse.warehouseName }
        };

                // Устанавливаем данные в ComboBox
                FromWarehouseComboBox.ItemsSource = warehouses;
                FromWarehouseComboBox.DisplayMemberPath = "warehouseName";
                FromWarehouseComboBox.SelectedValuePath = "warehouseID";

                // Устанавливаем выбранное значение в ComboBox на текущий склад
                FromWarehouseComboBox.SelectedValue = currentWarehouse.warehouseID;
            }
            else
            {
                MessageBox.Show("Не удалось найти склад с указанным ID.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Загрузка данных для выпадающих списков
        private async Task LoadPageDataAsync()
        {
            try
            {
                // Загрузка данных о доставках
                var deliveryTypes = _deliveryTypesAdapter.GetData();
                var warehouses = _warehousesAdapter.GetData();
                var dealers = _dealersAdapter.GetData();
                var transport = _transportAdapter.GetData();

                // Устанавливаем элементы в ComboBox'ах
                DeliveryTypeComboBox.ItemsSource = deliveryTypes;
                DeliveryTypeComboBox.DisplayMemberPath = "deliveryTypeName";
                DeliveryTypeComboBox.SelectedValuePath = "deliveryTypeID";


                FromWarehouseComboBox.DisplayMemberPath = "warehouseName";
                FromWarehouseComboBox.SelectedValuePath = "warehouseID";

                ToWarehouseComboBox.ItemsSource = warehouses;
                ToWarehouseComboBox.DisplayMemberPath = "warehouseName";
                ToWarehouseComboBox.SelectedValuePath = "warehouseID";

                ToDealerComboBox.ItemsSource = dealers;
                ToDealerComboBox.DisplayMemberPath = "dealerName";
                ToDealerComboBox.SelectedValuePath = "dealerID";

                TransportComboBox.ItemsSource = transport;
                TransportComboBox.DisplayMemberPath = "transportName";
                TransportComboBox.SelectedValuePath = "transportID";

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке данных: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Сохранение новой доставки
        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Сбор данных с UI
                var newDelivery = new ActiveDeliveryInfo
                {
                    DeliveryTypeID = (int)DeliveryTypeComboBox.SelectedValue,
                    FromWarehouseID = App.CurrentWareHouseID,
                    ToWarehouseID = (int)ToWarehouseComboBox.SelectedValue,
                    ToDealerID = (int)ToDealerComboBox.SelectedValue,
                    TransportID = (int)TransportComboBox.SelectedValue,
                    DeliveryDate = DeliveryDatePicker.SelectedDate ?? DateTime.Now,
                    StatusID = 6, // Статус "Не доставлено"
                    CreatedAt = DateTime.Now
                };

                // Пример добавления элементов и тракторов
                newDelivery.DeliveryItems.Add(new DeliveryItem
                {
                    DeliveryID = newDelivery.DeliveryID,
                    TractorID = 1, // Примерный ID трактора
                    PartID = 1,    // Примерный ID детали
                    Quantity = 10
                });

                newDelivery.TractorUnits.Add(new TractorUnit
                {
                    TractorID = 1,
                    Model = "Трактор X1",
                    LengthM = 5.2,
                    WidthM = 2.3,
                    HeightM = 2.5,
                    WeightKg = 3500,
                    EnginePowerHP = 200
                });

                // Добавление в базу данных
                await AddNewDeliveryAsync(newDelivery);
                CustomMessageBox.Show($"Успешное сохранение данных", "Успех", showCancel:false);
                NavigationService.GoBack();
            }
            catch (Exception ex)
            {
                CustomMessageBox.Show($"Ошибка при сохранении данных: {ex.Message}", "Ошибка", showCancel: false);
            }
        }


        private async Task AddNewDeliveryAsync(ActiveDeliveryInfo deliveryInfo)
        {
            try
            {
                // Добавляем информацию о доставке в базу данных (должен быть соответствующий адаптер для deliveries1)
                int newDeliveryID = _deliveriesAdapter.Insert(
                    deliveryInfo.DeliveryTypeID,
                    deliveryInfo.FromWarehouseID,
                    deliveryInfo.ToWarehouseID,
                    deliveryInfo.ToDealerID,
                    deliveryInfo.TransportID,
                    deliveryInfo.StatusID,
                    deliveryInfo.DeliveryDate,
                    DateTime.Now, null); // CreatedAt

                // Если доставка добавлена, теперь добавляем связанные элементы
                if (newDeliveryID > 0)
                {
                    deliveryInfo.DeliveryID = newDeliveryID;

                    // Добавление товаров в доставку
                    foreach (var item in deliveryInfo.DeliveryItems)
                    {
                        _partsAdapter.Insert(Convert.ToString(item.DeliveryID), item.TractorID, item.PartID, item.Quantity);
                    }

                    // Добавление тракторов в доставку
                    foreach (var tractor in deliveryInfo.TractorUnits)
                    {
                        _tractorsAdapter.Insert(tractor.Model, Convert.ToDecimal(tractor.LengthM),
                            Convert.ToDecimal(tractor.WidthM), Convert.ToDecimal(tractor.HeightM),
                            Convert.ToDecimal(tractor.WeightKg), (int)tractor.EnginePowerHP);
                    }

                    MessageBox.Show("Доставка добавлена успешно!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("Ошибка при добавлении доставки.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при добавлении доставки: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
   /*     // Очистка формы
        private void ClearForm()
        {
            DeliveryTypeComboBox.SelectedIndex = -1;
            FromWarehouseComboBox.SelectedIndex = -1;
            ToWarehouseComboBox.SelectedIndex = -1;
            ToDealerComboBox.SelectedIndex = -1;
            TransportComboBox.SelectedIndex = -1;
            DeliveryDatePicker.SelectedDate = null;
        }

        // Обработчик для кнопки "Отмена"
*/

        // Обработчик для загрузки страницы
        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            App.mainStackPanelBorder.Visibility = Visibility.Collapsed;

            // Загружаем данные складов и устанавливаем текущий склад
            SetWarehouseData();

            // Дополнительные действия, если нужно
            await LoadPageDataAsync();
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}
