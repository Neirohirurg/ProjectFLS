using ProjectFLS.flsdbDataSetTableAdapters;
using ProjectFLS.Models;
using ProjectFLS.UI;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace ProjectFLS.Manager.DataPages
{
    public partial class AddItemPage : Page
    {
        private readonly partsTableAdapter _partsTab = new partsTableAdapter();
        private readonly deliveryTypesTableAdapter _typesTab = new deliveryTypesTableAdapter();
        private readonly tractorUnitsTableAdapter _trTab = new tractorUnitsTableAdapter();

        private readonly Action<int, DeliveryItemVm> _onSave;
        private readonly Action<int> _onDelete;
        private readonly int _index;   // -1 = «добавление»
        private readonly bool _isEdit;

        // -------- конструктор «добавление» -------------
        public AddItemPage(Action<DeliveryItemVm> onAdd)
            : this((i, v) => onAdd(v), _ => { }, null, -1) { }

        // -------- конструктор «редактирование/удаление»-
        public AddItemPage(Action<int, DeliveryItemVm> onSave,
                           Action<int> onDelete,
                           DeliveryItemVm existing,
                           int index)
        {
            InitializeComponent();

            _onSave = onSave;
            _onDelete = onDelete;
            _index = index;
            _isEdit = existing != null;

            LoadComboBoxes();

            DeliveryTypeComboBox.SelectionChanged += DeliveryTypeComboBox_SelectionChanged;
            SetBordersVisibility(false, false, false);

            if (_isEdit) FillForm(existing);
            DeleteButton.Visibility = _isEdit ? Visibility.Visible : Visibility.Collapsed;
        }

        // ---------- загрузка списков ---------------
        private void LoadComboBoxes()
        {
            DeliveryTypeComboBox.ItemsSource = _typesTab.GetData()
                  .Where(r => r.deliveryTypeName != "Смешанная")
                  .ToList();

            PartsComboBox.ItemsSource = _partsTab.GetData().ToList();
            TractorComboBox.ItemsSource = _trTab.GetData().ToList();
        }

        // ---------- выбор типа ----------------------
        private void DeliveryTypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DeliveryTypeComboBox.SelectedItem is flsdbDataSet.deliveryTypesRow row)
            {
                string name = row.deliveryTypeName;

                if (name.Contains("Трактор"))
                    SetBordersVisibility(true, false, true);
                else if (name.Contains("Запчасть"))
                    SetBordersVisibility(false, true, true);
                else
                    SetBordersVisibility(false, false, false);
            }
            else
            {
                SetBordersVisibility(false, false, false);
            }
        }

        private void SetBordersVisibility(bool showTractor, bool showParts, bool showQty)
        {
            TractorBorder.Visibility = showTractor ? Visibility.Visible : Visibility.Collapsed;
            PartsBorder.Visibility = showParts ? Visibility.Visible : Visibility.Collapsed;
            QuantityBorder.Visibility = showQty ? Visibility.Visible : Visibility.Collapsed;
        }

        // ---------- заполнение при редактировании ----
        private void FillForm(DeliveryItemVm vm)
        {
            var types = _typesTab.GetData();
            var row = types.First(r => r.deliveryTypeName == (vm.IsTractor ? "Трактор" : "Запчасть"));
            DeliveryTypeComboBox.SelectedValue = row.deliveryTypeID;

            if (vm.IsTractor)
                TractorComboBox.SelectedValue = vm.TractorID;
            else
                PartsComboBox.SelectedValue = vm.PartID;

            Quantity.Text = vm.Quantity.ToString();
        }

        // ---------- Сохранить -----------------------
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            bool useTractor = TractorBorder.Visibility == Visibility.Visible;
            bool useParts = PartsBorder.Visibility == Visibility.Visible;

            if (useTractor && TractorComboBox.SelectedItem == null ||
                useParts && PartsComboBox.SelectedItem == null)
            {
                CustomMessageBox.Show("Выберите позицию.", "Ошибка", showCancel: false);
                return;
            }

            if (!int.TryParse(Quantity.Text.Trim(), out int qty) || qty <= 0)
            {
                CustomMessageBox.Show("Количество должно быть положительным числом.", "Ошибка", showCancel: false);
                return;
            }

            var vm = new DeliveryItemVm { Quantity = qty, IsTractor = useTractor };

            if (useTractor)
            {
                dynamic r = TractorComboBox.SelectedItem;
                vm.TractorID = (int)r.tractorID;
                vm.ItemName = r.model;
                vm.LengthM = r.lengthM;
                vm.WidthM = r.widthM;
                vm.HeightM = r.heightM;
                vm.WeightKg = r.weightKg;
            }
            else
            {
                dynamic r = PartsComboBox.SelectedItem;
                vm.PartID = (int)r.partID;
                vm.ItemName = r.partName;
                vm.LengthM = r.lengthM;
                vm.WidthM = r.widthM;
                vm.HeightM = r.heightM;
                vm.WeightKg = null;
            }

            _onSave?.Invoke(_index, vm);
            NavigationService.GoBack();
        }

        // ---------- Удалить -------------------------
        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Удалить позицию?", "Подтверждение",
                                MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                _onDelete?.Invoke(_index);
                NavigationService.GoBack();
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e) => NavigationService.GoBack();
        private void Page_Loaded(object sender, RoutedEventArgs e) { }
    }
}
