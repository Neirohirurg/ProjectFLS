using ProjectFLS.Models;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace ProjectFLS.Admin.DataPages.DerictoriesButton
{
    public partial class AddEditPage : Page
    {
        private List<FieldDefinition> FieldDefinitions;

        public AddEditPage()
        {
            InitializeComponent();

            // Здесь задаются поля
            FieldDefinitions = new List<FieldDefinition>
            {
                new FieldDefinition("RoleName", "Название роли:", FieldType.TextBox),
                new FieldDefinition("RoleType", "Тип роли:", FieldType.ComboBox, new List<string> { "Админ", "Пользователь", "Гость" })
            };
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            GenerateFields();
        }

        private void GenerateFields()
        {
            FieldsContainer.Children.Clear();

            foreach (var field in FieldDefinitions)
            {
                var border = new Border
                {
                    Style = (Style)FindResource("RoundedBorderBoxStyle"),
                    Margin = new Thickness(0, 0, 0, 10)
                };

                var stack = new StackPanel { Orientation = Orientation.Vertical };

                var label = new Label
                {
                    Content = field.Label,
                    Style = (Style)FindResource("menuLable")
                };

                Control inputControl;

                if (field.Type == FieldType.TextBox)
                {
                    inputControl = new TextBox
                    {
                        Style = (Style)FindResource("RoundedTextBoxStyle"),
                        Name = field.Name
                    };
                }
                else if (field.Type == FieldType.ComboBox)
                {
                    inputControl = new ComboBox
                    {
                        Style = (Style)FindResource("RoundedComboBoxStyle"),
                        Name = field.Name,
                        ItemsSource = field.Options
                    };
                }
                else continue;

                label.Target = inputControl;
                stack.Children.Add(label);
                stack.Children.Add(inputControl);
                border.Child = stack;

                FieldsContainer.Children.Add(border);
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            // Пример получения значения:
            var roleName = ((TextBox)FieldsContainer.FindName("RoleName"))?.Text;
            MessageBox.Show($"Сохраняем: {roleName}");
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            App.mainStackPanel.IsEnabled = true;
        }
    }
}
