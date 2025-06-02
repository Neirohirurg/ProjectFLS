using ProjectFLS.Models;
using ProjectFLS.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using ProjectFLS.UI;
using ProjectFLS.flsdbDataSetTableAdapters;

namespace ProjectFLS.Manager.DataPages
{
    public partial class AddDeliveryPage : Page
    {
        /* ----------  адаптеры  ---------- */
        private readonly deliveries1TableAdapter _deliveriesTa = new deliveries1TableAdapter();
        private readonly deliveryTypesTableAdapter _typesTa = new deliveryTypesTableAdapter();
        private readonly warehousesTableAdapter _whTa = new warehousesTableAdapter();
        private readonly dealersTableAdapter _dealersTa = new dealersTableAdapter();
        private readonly transportTableAdapter _transportTa = new transportTableAdapter();
        private readonly deliveryStatusesTableAdapter _statusesTa = new deliveryStatusesTableAdapter();
        private readonly partsTableAdapter _partsTa = new partsTableAdapter();
        private readonly tractorUnitsTableAdapter _trTa = new tractorUnitsTableAdapter();
        private readonly deliveryItemsTableAdapter _deliveryItemsTa = new deliveryItemsTableAdapter();

        /* ----------  состояние  ---------- */
        private readonly ObservableCollection<DeliveryItemVm> _items = new ObservableCollection<DeliveryItemVm>();
        private readonly Frame _hostFrame;

        private bool IsMainWarehouse { get { return App.CurrentWareHouseID == 1; } }

        /* ---------- ctor ---------- */
        public AddDeliveryPage(Frame hostFrame)
        {
            InitializeComponent();
            _hostFrame = hostFrame;

            DeliveryItemsListView.ItemsSource = _items;
            DeliveryItemsListView.MouseDoubleClick += OnItemDoubleClick;
        }

        /* ===================================================================
           1.  Работа со списком позиций
           ===================================================================*/
        private void AddItemButton_Click(object sender, RoutedEventArgs e)
        {
            _hostFrame.Navigate(new AddItemPage(AddItemCallback));
        }

        private void AddItemCallback(DeliveryItemVm vm)
        {
            /*  глав-склад  – запрещено добавлять тракторы  */
            if (IsMainWarehouse && vm.IsTractor)
            {
                CustomMessageBox.Show("На главный склад разрешены только запчасти.",
                                      "Запрет", showCancel: false);
                return;
            }

            _items.Add(vm);
            RecalcDeliveryType();
        }

        private void OnItemDoubleClick(object s, MouseButtonEventArgs e)
        {
            var vm = DeliveryItemsListView.SelectedItem as DeliveryItemVm;
            if (vm == null) return;

            int idx = _items.IndexOf(vm);
            _hostFrame.Navigate(new AddItemPage(
                onSave: (i, v) => { _items[i] = v; RecalcDeliveryType(); },
                onDelete: i => { _items.RemoveAt(i); RecalcDeliveryType(); },
                existing: vm,
                index: idx));
        }

        private void RecalcDeliveryType()
        {
            if (IsMainWarehouse) { DeliveryTypeTextBox.Text = "Запчасть"; return; }

            bool hasTr = _items.Any(x => x.IsTractor);
            bool hasPar = _items.Any(x => !x.IsTractor);

            DeliveryTypeTextBox.Text = hasTr && hasPar ? "Смешанная"
                                   : hasTr ? "Трактор"
                                   : hasPar ? "Запчасть"
                                                         : "";
        }

        /* ===================================================================
           2.  Загрузка справочников
           ===================================================================*/
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            App.mainStackPanelBorder.Visibility = Visibility.Collapsed;

            LoadWarehouses();
            LoadCombos();

            if (IsMainWarehouse)
            {
                FromWarehouseBorder.Visibility = Visibility.Collapsed;
                TransportBorder.Visibility = Visibility.Collapsed;
                ToDealerBorder.Visibility = Visibility.Collapsed;


                
            }

            RecalcDeliveryType();
        }

        private void LoadWarehouses()
        {
            /* текущий склад – получатель (Disabled) */
            var curWh = _whTa.GetData().First(w => w.warehouseID == App.CurrentWareHouseID);
            ToWarehouseComboBox.ItemsSource = new[] { new { curWh.warehouseID, curWh.warehouseName } };
            ToWarehouseComboBox.DisplayMemberPath = "warehouseName";
            ToWarehouseComboBox.SelectedValuePath = "warehouseID";
            ToWarehouseComboBox.SelectedValue = curWh.warehouseID;
            ToWarehouseComboBox.IsEnabled = false;

            /* завод – отправитель (Disabled) */
            var factory = _whTa.GetData().First(w => w.warehouseID == 1);
            FromWarehouseComboBox.ItemsSource = new[] { new { factory.warehouseID, factory.warehouseName } };
            FromWarehouseComboBox.DisplayMemberPath = "warehouseName";
            FromWarehouseComboBox.SelectedValuePath = "warehouseID";
            FromWarehouseComboBox.SelectedValue = factory.warehouseID;
            FromWarehouseComboBox.IsEnabled = false;
        }

        private void LoadCombos()
        {
            TransportComboBox.ItemsSource = _transportTa.GetData();
            TransportComboBox.DisplayMemberPath = "transportName";
            TransportComboBox.SelectedValuePath = "transportID";

            ToDealerComboBox.ItemsSource = _dealersTa.GetData();
            ToDealerComboBox.DisplayMemberPath = "dealerName";
            ToDealerComboBox.SelectedValuePath = "dealerID";
        }

        /* ===================================================================
           3.  Валидация формы
           ===================================================================*/
        private bool Validate(out int typeId, out string msg)
        {
            msg = "";
            typeId = 0;

            if (!_items.Any()) { msg = "Добавьте позиции."; return false; }
            if (IsMainWarehouse && _items.Any(i => i.IsTractor))
            {
                msg = "Тракторы запрещены для главного склада."; return false;
            }

            var typeRow = _typesTa.GetData()
                         .FirstOrDefault(r => r.deliveryTypeName.Equals(
                                 DeliveryTypeTextBox.Text.Trim(),
                                 StringComparison.InvariantCultureIgnoreCase));
            if (typeRow == null) { msg = "Неизвестный тип доставки."; return false; }

            typeId = typeRow.deliveryTypeID;

            if (TransportComboBox.SelectedValue == null) { msg = "Выберите транспорт."; return false; }
            if (ToDealerComboBox.SelectedValue == null) { msg = "Выберите дилера."; return false; }
            if (DeliveryDatePicker.SelectedDate == null) { msg = "Укажите дату."; return false; }

            return true;
        }

        /* ===================================================================
           4.  Сохранение доставки
           ===================================================================*/
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            int typeId; string err;
            if (!Validate(out typeId, out err))
            {
                CustomMessageBox.Show(err, "Ошибка", showCancel: false);
                return;
            }

            /* статус: «Доставка от поставщика» для главного склада,
                       иначе «Ожидает отправления»                       */
            string statusName = IsMainWarehouse ? "Доставка от поставщика"
                                                : "Ожидает отправления";
            int statusId = _statusesTa.GetData()
                           .First(s => s.statusName == statusName).statusID;

            /* ---------- вставка delivery ---------- */
            int newId = _deliveriesTa.Insert(
                /* deliveryTypeID */ typeId,
                /* fromWh        */ 1,                       // завод
                /* toWh          */ App.CurrentWareHouseID,
                /* toDealer      */ (int)ToDealerComboBox.SelectedValue,
                /* transport     */ (int)TransportComboBox.SelectedValue,
                /* status        */ statusId,
                /* date          */ DeliveryDatePicker.SelectedDate.Value,
                /* createdAt     */ DateTime.Now,
                /* updatedAt     */ null);

            if (newId <= 0)
            {
                CustomMessageBox.Show("Ошибка записи delivery.", "Ошибка", showCancel: false);
                return;
            }

            /* ---------- позиции ---------- */
            foreach (var it in _items)
            {
                if (it.IsTractor)
                {
                    /* PartsTable: insert( string deliveryID, int? tractorID, int? partID, int qty )   */
                    _deliveryItemsTa.Insert(newId, it.TractorID, null, it.Quantity);

                }
                else
                {
                    _deliveryItemsTa.Insert(newId, null, it.PartID, it.Quantity);
                }
            }

            CustomMessageBox.Show("Доставка сохранена.", "OK", showCancel: false);
            NavigationService.GoBack();
        }

        private void CancelButton_Click(object s, RoutedEventArgs e) => NavigationService.GoBack();

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {

        }
    }
}
