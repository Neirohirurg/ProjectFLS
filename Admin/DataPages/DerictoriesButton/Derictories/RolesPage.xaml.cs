using ProjectFLS.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ProjectFLS.Admin.DataPages.DerictoriesButton.Derictories
{
    /// <summary>
    /// Логика взаимодействия для RolesPage.xaml
    /// </summary>
    public partial class RolesPage : Page, INavigationPanelHost, ISearchable
    {
        public RolesPage()
        {
            InitializeComponent();
        }

        public void EnableSearch()
        {

        }
        public void SetupNavigationPanel(StackPanel panel, Border border)
        {
            panel.Children.Clear();
            border.Visibility = Visibility.Visible;

            void AddItem(string name, MouseButtonEventHandler handler)
            {
                var label = new Label
                {
                    Content = name,
                    Style = (Style)Application.Current.FindResource("menuLabel")
                };
                label.MouseLeftButtonUp += handler;
                panel.Children.Add(label);
            }

            AddItem("Добавить", Add_Click);
            AddItem("Удалить", Delete_Click);
            AddItem("Редактировать", Edit_Click);
        }

        private void Add_Click(object sender, MouseButtonEventArgs e)
        {
            // Навигация или логика
        }

        private void Delete_Click(object sender, MouseButtonEventArgs e)
        {
            // Навигация или логика
        }

        private void Edit_Click(object sender, MouseButtonEventArgs e)
        {
            // Навигация или логика
        }

        public void PerformSearch(string query)
        {
            // Поиск внутри списка ролей
        }
    }

}
