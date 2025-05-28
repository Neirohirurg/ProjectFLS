using System.Runtime.Remoting.Channels;
using System.Windows;
using System.Windows.Controls;

namespace ProjectFLS.Admin.DataPages.DerictoriesButton.Derictories
{
    public partial class AddEditUserStatusPage : Page
    {
        private int? statusID = null;
        private flsdbDataSetTableAdapters.userStatusTableAdapter adapter = new flsdbDataSetTableAdapters.userStatusTableAdapter();

        public AddEditUserStatusPage(int? id = null)
        {
            InitializeComponent();
            statusID = id;

            if (statusID.HasValue)
            {
                var row = adapter.GetData().FindBystatusID(statusID.Value);
                if (row != null)
                    StatusNameTextBox.Text = row.statusName;
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            string name = StatusNameTextBox.Text.Trim();

            if (string.IsNullOrEmpty(name))
            {
                MessageBox.Show("Введите название статуса.");
                return;
            }

            if (statusID.HasValue)
            {
                adapter.UpdateQuery(name, statusID.Value);
            }
            else
            {
                adapter.Insert(name);
            }

            App.mainStackPanel.IsEnabled = true;
            NavigationService?.GoBack();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            App.mainStackPanel.IsEnabled = true;
            NavigationService?.GoBack();
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {

        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {

        }
    }
}
