using ProjectFLS.flsdbDataSetTableAdapters;
using ProjectFLS.UI;
using System;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;

namespace ProjectFLS.Admin.DataPages.DerictoriesButton.Derictories
{
    public partial class AddEditRolesPage : Page
    {
        private rolesTableAdapter _roles;
        private int? _roleId;

        public AddEditRolesPage()
        {
            InitializeComponent();
            _roles = new rolesTableAdapter();
        }

        public AddEditRolesPage(int roleId) : this()
        {
            _roleId = roleId;
            LoadRoleData();
        }

        private void LoadRoleData()
        {
            var roleData = _roles.GetData().FindByroleID(_roleId.Value);
            if (roleData != null)
            {
                RoleNameTextBox.Text = roleData.roleName;
            }
        }


        private void Save_Click(object sender, RoutedEventArgs e)
        {
            string roleName = RoleNameTextBox.Text.Trim();

            if (string.IsNullOrEmpty(roleName))
            {
                CustomMessageBox.Show("Пожалуйста, введите название роли.", "Ошибка", showCancel: false);
                return;
            }

            try
            {
                if (_roleId.HasValue)
                {
                    var row = _roles.GetData().FindByroleID(_roleId.Value);
                    if (row != null)
                    {
                        row.roleName = roleName;
                        _roles.Update(row);
                        CustomMessageBox.Show("Роль успешно обновлена.", "Успех", showCancel: false);
                    }
                    else
                    {
                        CustomMessageBox.Show("Роль не найдена.", "Ошибка", showCancel: false);
                    }
                }
                else
                {
                    _roles.Insert(roleName);
                    CustomMessageBox.Show("Роль успешно добавлена.", "Успех", showCancel: false);
                }

                // После сохранения можно вернуться на RolesPage
                NavigationService.GoBack();
            }
            catch (Exception ex)
            {
                CustomMessageBox.Show("Произошла ошибка при сохранении роли:\n" + ex.Message, "Ошибка", showCancel: false);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            App.mainStackPanel.Visibility = Visibility.Visible;
            App.mainStackPanel.IsEnabled = false;
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            App.mainStackPanel.IsEnabled = true;
        }
    }
}

