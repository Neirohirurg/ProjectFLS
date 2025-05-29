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

namespace ProjectFLS.Manager.DataPages
{
    /// <summary>
    /// Логика взаимодействия для PartnersPage.xaml
    /// </summary>
    public partial class PartnersPage : Page, ISearchable
    {
        private Frame _managerMainFrame;
        private Border _mainBorder;
        public PartnersPage(Frame managerMainFrame)
        {
            InitializeComponent();
            _managerMainFrame = managerMainFrame;
            _mainBorder = App.mainStackPanelBorder;
        }

        public void PerformSearch(string query)
        {

        }

        public void EnableSearch() { }

        public void RefreshPartners()
        {

        }

        public void DeleteSelectedPartner()
        {

        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            _mainBorder.Visibility = Visibility.Visible;
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            _mainBorder.Visibility = Visibility.Collapsed;
        }
    }
}
