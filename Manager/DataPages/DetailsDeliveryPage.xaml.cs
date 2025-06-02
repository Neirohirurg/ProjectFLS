using ProjectFLS.flsdbDataSetTableAdapters;
using ProjectFLS.Models;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace ProjectFLS.Manager.DataPages
{
    public partial class DetailsDeliveryPage : Page
    {
        // ─── адаптеры, чтобы вытащить содержимое доставки ──────────────────────────
        private readonly deliveryItemsTableAdapter _itemsTab = new deliveryItemsTableAdapter();
        private readonly warehousePartsTableAdapter _partsTabinWh = new warehousePartsTableAdapter();
        private readonly warehouseTractorsTableAdapter _tractorsTabinWh = new warehouseTractorsTableAdapter();
        private readonly partsTableAdapter _partsTab = new partsTableAdapter();
        private readonly tractorUnitsTableAdapter _trTab = new tractorUnitsTableAdapter();
        private readonly deliveryTypesTableAdapter _typesTab = new deliveryTypesTableAdapter();

        public DetailsDeliveryPage(int deliveryId)
        {
            InitializeComponent();
            LoadItems(deliveryId);
        }

        private void LoadItems(int deliveryId)
        {
            var itemsRows = _itemsTab.GetByDelivery(deliveryId);  // ← новый метод
            var partsRows = _partsTab.GetData();
            var tractors = _trTab.GetData();

            var list = new List<DeliveryItemVm>();

            foreach (var row in itemsRows)
            {

                var vm = new DeliveryItemVm { Quantity = row.quantity };

                if (!row.IspartIDNull())          // ► запчасть
                {
                    var p = partsRows.First(r => r.partID == row.partID);
                    vm = new DeliveryItemVm
                    {
                        Quantity = row.quantity,
                        IsTractor = false,
                        PartID = p.partID,
                        ItemName = p.partName,
                        LengthM = p.lengthM,
                        WidthM = p.widthM,
                        HeightM = p.heightM,
                        DeliveryTypeName = "Запчасть"
                    };
                }
                else if (!row.IstractorIDNull())  // ► трактор
                {

                    var t = tractors.First(r => r.tractorID == row.tractorID);
                    vm = new DeliveryItemVm
                    {
                        Quantity = row.quantity,
                        IsTractor = true,
                        TractorID = t.tractorID,
                        ItemName = t.model,
                        LengthM = t.lengthM,
                        WidthM = t.widthM,
                        HeightM = t.heightM,
                        WeightKg = t.weightKg,
                        DeliveryTypeName = "Трактор"
                    };
                }


                list.Add(vm);
            }

            DeliveryItemsListView.ItemsSource = list;
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}
