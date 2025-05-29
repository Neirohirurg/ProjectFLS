using System;
using System.Windows;
using System.Windows.Controls;
using ProjectFLS.flsdbDataSetTableAdapters;
using ProjectFLS.Models;

namespace ProjectFLS.Manager.DataPages
{
    public partial class WarehousesPage : Page
    {
        // Адаптер для работы с таблицей складов
        private warehousesTableAdapter _warehousesAdapter;

        // ID текущего пользователя
        private int CurrentUserID = App.CurrentUserId;  // Замените на реальный ID текущего пользователя

        public WarehousesPage()
        {
            InitializeComponent();
            _warehousesAdapter = new warehousesTableAdapter();
            LoadWarehouses();
        }

        // Загрузка складов из базы данных и создание кнопок
        private void LoadWarehouses()
        {
            try
            {
                // Загрузим данные о складах для текущего пользователя
                var warehousesData = _warehousesAdapter.GetWarehousesByUserId(CurrentUserID);

                // Очистим панель от старых кнопок
                WarehousesPanel.Children.Clear();

                // Динамически создаём кнопки для каждого склада
                foreach (var warehouse in warehousesData)
                {
                    var warehouseButton = new Button
                    {
                        Content = warehouse.warehouseName, // Название склада
                        Tag = warehouse.warehouseID, // Для получения ID склада
                        Style = (Style)FindResource("RoundButtonStyleBig"), // Применение стиля
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
        private void WarehouseButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button != null)
            {
                // Получаем ID склада из свойства Tag
                int warehouseId = (int)button.Tag;

                // Открываем страницу с деталями склада (или информацию о нем)
                MessageBox.Show($"Вы выбрали склад с ID: {warehouseId}", "Информация о складе", MessageBoxButton.OK, MessageBoxImage.Information);

                // Можно добавить логику для перехода на новую страницу или отображения деталей склада
            }
        }
    }
}
