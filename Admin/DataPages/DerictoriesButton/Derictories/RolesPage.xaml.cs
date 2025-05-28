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
    public partial class RolesPage : Page, INavigationPanelHost
    {
        public RolesPage()
        {
            InitializeComponent();
        }

        public void SetupNavigationPanel(StackPanel panel, Border border)
        {
            NavigationPanelHelper.SetupDefaultPanel(
                panel, border,
                Add_Click, Delete_Click, Edit_Click
            );
        }

        private void Add_Click(object sender, MouseButtonEventArgs e)
        {
            // логика добавления
        }

        private void Delete_Click(object sender, MouseButtonEventArgs e)
        {
            // логика удаления
        }

        private void Edit_Click(object sender, MouseButtonEventArgs e)
        {
            // логика редактирования
        }
    }
}
