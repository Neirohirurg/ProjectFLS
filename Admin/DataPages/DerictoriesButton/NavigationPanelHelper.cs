using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows;

namespace ProjectFLS.Admin.DataPages.DerictoriesButton
{
    internal class NavigationPanelHelper
    {
        public static void SetupDefaultPanel(StackPanel panel, Border border,
            MouseButtonEventHandler addHandler,
            MouseButtonEventHandler deleteHandler,
            MouseButtonEventHandler editHandler)
        {
            panel.Children.Clear();

            panel.Children.Add(CreateLabel("Добавить", addHandler));
            panel.Children.Add(CreateLabel("Удалить", deleteHandler));
            panel.Children.Add(CreateLabel("Редактировать", editHandler));

            border.Visibility = Visibility.Visible;
        }

        private static Label CreateLabel(string content, MouseButtonEventHandler handler)
        {
            var label = new Label
            {
                Content = content,
                Style = (Style)Application.Current.FindResource("menuLabel"),
                Cursor = Cursors.Hand
            };

            label.MouseLeftButtonUp += handler;
            return label;
        }
    }
}
