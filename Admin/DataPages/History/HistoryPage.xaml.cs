using ProjectFLS.flsdbDataSetTableAdapters;
using ProjectFLS.Interfaces;
using ProjectFLS.Models;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace ProjectFLS.Admin.DataPages.History
{
    public partial class HistoryPage : Page, ISearchable
    {
        private deliveryAuditTableAdapter _auditAdapter;
        private deliveryStatusesTableAdapter _statusAdapter;
        private usersTableAdapter _usersAdapter;
        private StackPanel _stackpanel;
        private Border _stackpanelBorder;

        private string _currentSortField = null;
        private bool _sortAscending = true;

        public HistoryPage()
        {
            InitializeComponent();
            _auditAdapter = new deliveryAuditTableAdapter();
            _statusAdapter = new deliveryStatusesTableAdapter();
            _usersAdapter = new usersTableAdapter();
            _stackpanel = App.mainStackPanel;
            _stackpanelBorder = App.mainStackPanelBorder;
        }

        public void EnableSearch() { }

        // Реализация поиска
        public void PerformSearch(string query)
        {
            var auditList = _auditAdapter.GetData();
            query = query?.ToLowerInvariant() ?? "";

            // Получаем статусы и пользователей
            var statuses = _statusAdapter.GetData();
            var users = _usersAdapter.GetData();

            // Фильтруем данные по всем полям
            var filtered = auditList.Where(audit =>
                (audit.auditID.ToString().Contains(query)) ||  // Поиск по ID аудита
                (audit.deliveryID.ToString().Contains(query)) ||  // Поиск по ID доставки
                (audit.oldStatus != null && Convert.ToString(audit.oldStatus).ToLower().Contains(query)) ||  // Поиск по старому статусу
                (audit.newStatus != null && Convert.ToString(audit.newStatus).ToLower().Contains(query)) ||  // Поиск по новому статусу
                (users.FirstOrDefault(u => u.userID == Convert.ToInt32(audit.changeBy))?.surname?.ToLower().Contains(query) ?? false) // Поиск по фамилии пользователя
            ).ToList();

            // Преобразуем в нужный формат
            var historyList = filtered.Select(audit =>
            {
                // Получаем статусы по ID
                var oldStatusName = statuses.FirstOrDefault(s => s.statusID == audit.oldStatus)?.statusName ?? "Неизвестный статус";
                var newStatusName = statuses.FirstOrDefault(s => s.statusID == audit.newStatus)?.statusName ?? "Неизвестный статус";
                var user = users.FirstOrDefault(u => u.userID == Convert.ToInt32(audit.changeBy));

                // Логируем, чтобы понять, что происходит
                if (user == null)
                {
                    Console.WriteLine($"Пользователь с ID {audit.changeBy} не найден.");
                }

                return new AuditRecord
                {
                    AuditID = audit.auditID,
                    DeliveryID = audit.deliveryID,
                    ChangedAt = audit.changedAt,
                    OldStatus = oldStatusName,
                    NewStatus = newStatusName,
                    ChangedBy = user != null ? $"{user.surname} {user.firstname}" : "Неизвестный пользователь", // Добавляем имя пользователя
                };
            }).ToList();

            HistoryListView.ItemsSource = historyList; // Устанавливаем источник данных
        }

        // Метод для сортировки
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
        private void ApplySorting()
        {
            var data = HistoryListView.ItemsSource.Cast<AuditRecord>().ToList();  // Преобразуем ItemsSource в список AuditRecord

            var sortedData = _sortAscending
                ? data.OrderBy(row => GetFieldValue(row, _currentSortField)).ToList()
                : data.OrderByDescending(row => GetFieldValue(row, _currentSortField)).ToList();

            HistoryListView.ItemsSource = sortedData;  // Обновляем ItemsSource с отсортированными данными
        }

        private object GetFieldValue(AuditRecord record, string field)
        {
            switch (field)
            {
                case "AuditID":
                    return record.AuditID;
                case "DeliveryID":
                    return record.DeliveryID;
                case "ChangedAt":
                    return record.ChangedAt;
                case "OldStatus":
                    return record.OldStatus;
                case "NewStatus":
                    return record.NewStatus;
                case "ChangedBy":
                    return record.ChangedBy;
                default:
                    return null;
            }
        }

        // Структура данных для отображения
        public class AuditRecord
        {
            public int AuditID { get; set; }
            public int DeliveryID { get; set; }
            public DateTime ChangedAt { get; set; }
            public string OldStatus { get; set; }
            public string NewStatus { get; set; }
            public string ChangedBy { get; set; }
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            _stackpanelBorder.Visibility = Visibility.Visible;
            await RefreshAsync();
        }

        private async Task RefreshAsync()
        {
            // Получаем все данные
            var auditList = await Task.Run(() => _auditAdapter.GetData());
            var statuses = _statusAdapter.GetData();
            var users = _usersAdapter.GetData();

            var historyList = auditList.Select(audit =>
            {
                var oldStatusName = statuses.FirstOrDefault(s => s.statusID == audit.oldStatus)?.statusName ?? "Неизвестный статус";
                var newStatusName = statuses.FirstOrDefault(s => s.statusID == audit.newStatus)?.statusName ?? "Неизвестный статус";
                var user = users.FirstOrDefault(u => u.userID == Convert.ToInt32(audit.changeBy));

                // Логируем, чтобы понять, что происходит
                if (user == null)
                {
                    Console.WriteLine($"Пользователь с ID {audit.changeBy} не найден.");
                }

                return new AuditRecord
                {
                    AuditID = audit.auditID,
                    DeliveryID = audit.deliveryID,
                    ChangedAt = audit.changedAt,
                    OldStatus = oldStatusName,
                    NewStatus = newStatusName,
                    ChangedBy = user != null ? $"{user.surname} {user.firstname}" : "Неизвестный пользователь", // Имя и фамилия
                };
            }).ToList();

            HistoryListView.ItemsSource = historyList;
        }

        private void HistoryListView_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            // Здесь можно добавить логику для двойного клика, если нужно
        }
    }
}
