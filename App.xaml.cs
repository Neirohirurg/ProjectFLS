using ProjectFLS.Interfaces;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace ProjectFLS
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static int CurrentUserId { get; set; }

        public static Border mainStackPanelBorder { get; set; }
        public static StackPanel mainStackPanel { get; set; }
        public static TextBox mainSearchTextBox { get; set; }

        public static void TryPerformSearch(object currentContent, string query)
        {
            if (currentContent is ISearchable searchable)
            {
                searchable.PerformSearch(query);
            }
        }   
    }
}
