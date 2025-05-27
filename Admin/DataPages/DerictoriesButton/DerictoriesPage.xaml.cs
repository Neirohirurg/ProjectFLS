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

namespace ProjectFLS.Admin.DataPages.DerictoriesButton
{
    /// <summary>
    /// Логика взаимодействия для DerictoriesPage.xaml
    /// </summary>
    public partial class DerictoriesPage : Page
    {
        private StackPanel _stackpanel;
        public DerictoriesPage(StackPanel stackPanel)
        {
            InitializeComponent();

            _stackpanel = stackPanel;
        }
    }
}
