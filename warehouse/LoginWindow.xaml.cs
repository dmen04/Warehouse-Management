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
using System.Windows.Shapes;
using warehouse.Models;
using warehouse.ViewModels;

namespace warehouse
{
    public partial class LoginWindow : Window
    {
        private LoginViewModel _viewModel;

        public LoginWindow()
        {
            InitializeComponent();
            _viewModel = new LoginViewModel();
            DataContext = _viewModel;
            _viewModel.LoginSuccessful += OnLoginSuccessful;
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is LoginViewModel vm)
            {
                vm.Password = PasswordBox.Password;
            }
        }

        private void OnLoginSuccessful(object sender, User user)
        {
            var dashboardWindow = new DashboardWindow(user.Username, user.Role);
            dashboardWindow.Show();
            this.Close();
        }
    }
}