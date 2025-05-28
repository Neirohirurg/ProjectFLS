using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace ProjectFLS.Interfaces
{
    internal interface INavigationPanelHost
    {
        void SetupNavigationPanel(StackPanel panel, Border border);
    }
}
