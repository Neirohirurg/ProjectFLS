using ProjectFLS.Interfaces;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace ProjectFLS
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static int CurrentUserId { get; set; }

        public static void TryPerformSearch(object currentContent, string query)
        {
            if (currentContent is ISearchable searchable)
            {
                searchable.PerformSearch(query);
            }
        }   
    }
}
