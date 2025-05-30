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

            // Получаем статусы и пользователей, но создаем словари для быстрого доступа
            var statuses = _statusAdapter.GetData()
                .ToDictionary(s => s.statusID, s => s.statusName);

            var users = _usersAdapter.GetData()
                .ToDictionary(u => $"{u.surname.ToLower()} {u.firstname.ToLower()}", u => $"{u.surname} {u.firstname}");

            // Фильтруем данные по всем полям
            var filtered = auditList.Where(audit =>
                audit.auditID.ToString().Contains(query) ||  // Поиск по ID аудита
                audit.deliveryID.ToString().Contains(query) ||  // Поиск по ID доставки
                (audit.oldStatus != null && statuses.Values.Any(s => s.ToLower().Contains(query))) ||  // Поиск по старому статусу
                (audit.newStatus != null && statuses.Values.Any(s => s.ToLower().Contains(query))) ||  // Поиск по новому статусу
                (audit.changeBy != null && users.Keys.Any(u => u.Contains(query.ToLower()))) // Поиск по фамилии и имени пользователя
            ).ToList();

            // Преобразуем в нужный формат
            var historyList = filtered.Select(audit =>
            {
                // Используем Contains для поиска статуса
                var oldStatusName = statuses.Values.FirstOrDefault(s => s.ToLower().Contains(query)) ?? "Неизвестный статус";
                var newStatusName = statuses.Values.FirstOrDefault(s => s.ToLower().Contains(query)) ?? "Неизвестный статус";

                // Поиск пользователя по фамилии и имени
                var userName = users.Keys.FirstOrDefault(u => u.Contains(query.ToLower())) ?? "Неизвестный пользователь";

                return new AuditRecord
                {
                    AuditID = audit.auditID,
                    DeliveryID = audit.deliveryID,
                    ChangedAt = audit.changedAt,
                    OldStatus = oldStatusName,
                    NewStatus = newStatusName,
                    ChangedBy = userName,
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

            // Для правильной сортировки учитываем тип данных
            var sortedData = _sortAscending
                ? data.OrderBy(row => GetFieldValue(row, _currentSortField)).ToList()
                : data.OrderByDescending(row => GetFieldValue(row, _currentSortField)).ToList();

            HistoryListView.ItemsSource = sortedData;  // Обновляем ItemsSource с отсортированными данными
        }

        private object GetFieldValue(AuditRecord record, string field)
        {
            switch (field)
            {
                case "auditID":
                    return record.AuditID;
                case "deliveryID":
                    return record.DeliveryID;
                case "changedAt":
                    return record.ChangedAt;  // Должен корректно работать с типом DateTime
                case "oldStatus":
                    return record.OldStatus;
                case "newStatus":
                    return record.NewStatus;
                case "changedBy":
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
            // Получаем все данные асинхронно
            var auditList = await Task.Run(() => _auditAdapter.GetData());
            var statuses = await Task.Run(() => _statusAdapter.GetData());
            var users = await Task.Run(() => _usersAdapter.GetData());

            // Создаем словари для быстрого поиска
            var statusesDict = statuses.ToDictionary(s => s.statusID, s => s.statusName);
            var usersDict = users.ToDictionary(u => u.userID.ToString(), u => $"{u.surname} {u.firstname}");

            var historyList = auditList.Select(audit =>
            {
                // Поиск статусов с использованием словаря
                var oldStatusName = statusesDict.TryGetValue(audit.oldStatus, out var oldStatus) ? oldStatus : "Неизвестный статус";
                var newStatusName = statusesDict.TryGetValue(audit.newStatus, out var newStatus) ? newStatus : "Неизвестный статус";

                // Используем App.CurentUserID для нахождения текущего пользователя
                var userName = usersDict.TryGetValue(App.CurrentUserId.ToString(), out var user) ? user : "Неизвестный пользователь";

                return new AuditRecord
                {
                    AuditID = audit.auditID,
                    DeliveryID = audit.deliveryID,
                    ChangedAt = audit.changedAt,
                    OldStatus = oldStatusName,
                    NewStatus = newStatusName,
                    ChangedBy = userName, // Имя и фамилия пользователя
                };
            }).ToList();

            // Устанавливаем источник данных в ListView
            HistoryListView.ItemsSource = historyList;
        }


        private void HistoryListView_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            // Здесь можно добавить логику для двойного клика, если нужно
        }
    }
}
