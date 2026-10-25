using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using warehouse.Models;
using warehouse.Views;

namespace warehouse.ViewModels
{
    class MainViewModel : INotifyPropertyChanged
    {
        private DispatcherTimer _sessionTimer;
        private DateTime _sessionStartTime;

        private string _sessionTime;
        public string SessionTime
        {
            get => _sessionTime;
            set { _sessionTime = value; OnPropertyChanged(); }
        }

        private string _currentUsername;
        public string CurrentUsername
        {
            get => _currentUsername;
            set { _currentUsername = value; OnPropertyChanged(); }
        }

        private string _currentUserRole;
        public string CurrentUserRole
        {
            get => _currentUserRole;
            set
            {
                _currentUserRole = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsAdmin));
            }
        }

        public bool IsAdmin => CurrentUserRole == "Admin";

        private object _currentViewModel;
        public object CurrentViewModel
        {
            get => _currentViewModel;
            set { _currentViewModel = value; OnPropertyChanged(); }
        }

        private string _headerTitle;
        public string HeaderTitle
        {
            get => _headerTitle;
            set { _headerTitle = value; OnPropertyChanged(); }
        }

        public RelayCommand SetHomeViewModel { get; }
        public RelayCommand SetAddCategoryViewModel { get; }
        public RelayCommand SetCategoriesViewModel { get; }
        public RelayCommand SetAddItemViewModel { get; }
        public RelayCommand SetInventoryViewModel { get; }
        public RelayCommand SetAddStockViewModel { get; }
        public RelayCommand SetShelvesViewModel { get; }
        public RelayCommand SetTransfersViewModel { get; }
        public RelayCommand SetUserManagementViewModel { get; }
        public RelayCommand SetAddShelfViewModel { get; }
        public RelayCommand SetStockViewModel { get; }
        public RelayCommand LogoutCommand { get; }

        public event EventHandler LogoutRequested;

        private void NavigateToHome()
        {
            CurrentViewModel = new HomeViewModel();
            HeaderTitle = "Home";
        }
        private void NavigateToAddCategory()
        {
            CurrentViewModel = new AddCategoryViewModel();
            HeaderTitle = "Add Category";
        }
        private void NavigateToCategories()
        {
            CurrentViewModel = new CategoriesViewModel();
            HeaderTitle = "Categories";
        }
        private void NavigateToAddItem()
        {
            CurrentViewModel = new AddItemViewModel();
            HeaderTitle = "Add Item";
        }
        private void NavigateToInventory()
        {
            CurrentViewModel = new InventoryViewModel();
            HeaderTitle = "Inventory";
        }
        private void NavigateToAddStock(Shelf shelf = null)
        {
            CurrentViewModel = new AddStockViewModel(shelf);
            HeaderTitle = "Add Stock";
        }
        private void NavigateToShelves()
        {
            CurrentViewModel = new ShelvesViewModel();
            HeaderTitle = "Shelves";
        }
        private void NavigateToTransfers()
        {
            CurrentViewModel = new TransfersViewModel();
            HeaderTitle = "Transfers";
        }
        private void NavigateToUserManagement()
        {
            CurrentViewModel = new UserManagementViewModel();
            HeaderTitle = "User Management";
        }
        private void NavigateToAddShelf()
        {
            CurrentViewModel = new AddShelfViewModel();
            HeaderTitle = "Add Shelf";
        }
        private void NavigateToStock(Shelf shelf)
        {
            CurrentViewModel = new StockViewModel(shelf);
            HeaderTitle = $"{shelf.Name} - Stock";
        }

        public MainViewModel()
        {
            SetHomeViewModel = new RelayCommand(o => NavigateToHome());
            SetAddCategoryViewModel = new RelayCommand(o => NavigateToAddCategory());
            SetCategoriesViewModel = new RelayCommand(o => NavigateToCategories());
            SetAddItemViewModel = new RelayCommand(o => NavigateToAddItem());
            SetInventoryViewModel = new RelayCommand(o => NavigateToInventory());
            SetAddStockViewModel = new RelayCommand(shelfObj => NavigateToAddStock(shelfObj as Shelf));
            SetShelvesViewModel = new RelayCommand(o => NavigateToShelves());
            SetTransfersViewModel = new RelayCommand(o => NavigateToTransfers());
            SetUserManagementViewModel = new RelayCommand(o => NavigateToUserManagement());
            SetAddShelfViewModel = new RelayCommand(o => NavigateToAddShelf());
            SetStockViewModel = new RelayCommand(shelfObj => NavigateToStock(shelfObj as Shelf));
            LogoutCommand = new RelayCommand(o => Logout());

            _sessionStartTime = DateTime.Now;
            _sessionTimer = new DispatcherTimer();
            _sessionTimer.Interval = TimeSpan.FromSeconds(1);
            _sessionTimer.Tick += SessionTimer_Tick;
            _sessionTimer.Start();

            CurrentViewModel = new HomeViewModel();
            HeaderTitle = "Home";
        }

        private void Logout()
        {
            LogoutRequested?.Invoke(this, EventArgs.Empty);
        }

        private void SessionTimer_Tick(object sender, EventArgs e)
        {
            var elapsed = DateTime.Now - _sessionStartTime;
            SessionTime = $"Session: {elapsed.Hours:D2}:{elapsed.Minutes:D2}:{elapsed.Seconds:D2}";
        }





        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}