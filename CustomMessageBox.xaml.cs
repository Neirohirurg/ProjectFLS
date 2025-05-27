using System.Windows;

namespace ProjectFLS.UI
{
    public partial class CustomMessageBox : Window
    {
        public static bool? Show(string message, string title = "Сообщение", bool showCancel = false)
        {
            var msgBox = new CustomMessageBox(message, title, showCancel);
            return msgBox.ShowDialog();
        }

        private CustomMessageBox(string message, string title, bool showCancel)
        {
            InitializeComponent();
            titleText.Text = title;
            MessageTextBlock.Text = message;

            if (showCancel)
            {
                CancelButton.Visibility = Visibility.Visible;
            }
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
