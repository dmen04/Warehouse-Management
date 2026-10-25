using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using warehouse.ViewModels;

namespace warehouse
{
    public partial class DashboardWindow : Window
    {
        public DashboardWindow(string username, string role)
        {
            InitializeComponent();
            var viewModel = new MainViewModel();
            DataContext = viewModel;
            viewModel.CurrentUsername = username;
            viewModel.CurrentUserRole = role;
            viewModel.LogoutRequested += OnLogoutRequested;
        }

        private void OnLogoutRequested(object sender, System.EventArgs e)
        {
            var loginWindow = new LoginWindow();
            loginWindow.Show();
            this.Close();
        }
    }
}