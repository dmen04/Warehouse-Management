using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.Json;
using warehouse.Models;

namespace warehouse.ViewModels
{
    class HomeViewModel : INotifyPropertyChanged
    {
        private const string ItemsJsonFilePath = "items.json";
        private const string ShelvesJsonFilePath = "shelves.json";
        private const string CategoriesJsonFilePath = "categories.json";
        private const string StockJsonFilePath = "shelfstock.json";

        private int _totalItems;
        public int TotalItems
        {
            get => _totalItems;
            set { _totalItems = value; OnPropertyChanged(); }
        }

        private int _totalShelves;
        public int TotalShelves
        {
            get => _totalShelves;
            set { _totalShelves = value; OnPropertyChanged(); }
        }

        private int _totalCategories;
        public int TotalCategories
        {
            get => _totalCategories;
            set { _totalCategories = value; OnPropertyChanged(); }
        }

        private int _totalStock;
        public int TotalStock
        {
            get => _totalStock;
            set { _totalStock = value; OnPropertyChanged(); }
        }

        public HomeViewModel()
        {
            LoadStatistics();
        }

        private void LoadStatistics()
        {
            TotalItems = CountItems();
            TotalShelves = CountShelves();
            TotalCategories = CountCategories();
            TotalStock = CountTotalStock();
        }

        private int CountItems()
        {
            if (!File.Exists(ItemsJsonFilePath)) return 0;

            var json = File.ReadAllText(ItemsJsonFilePath);
            var items = JsonSerializer.Deserialize<ObservableCollection<Item>>(json);
            return items?.Count ?? 0;
        }

        private int CountShelves()
        {
            if (!File.Exists(ShelvesJsonFilePath)) return 0;

            var json = File.ReadAllText(ShelvesJsonFilePath);
            var shelves = JsonSerializer.Deserialize<ObservableCollection<Shelf>>(json);
            return shelves?.Count ?? 0;
        }

        private int CountCategories()
        {
            if (!File.Exists(CategoriesJsonFilePath)) return 0;

            var json = File.ReadAllText(CategoriesJsonFilePath);
            var categories = JsonSerializer.Deserialize<ObservableCollection<Category>>(json);
            return categories?.Count ?? 0;
        }

        private int CountTotalStock()
        {
            if (!File.Exists(StockJsonFilePath)) return 0;

            var json = File.ReadAllText(StockJsonFilePath);
            var stock = JsonSerializer.Deserialize<ObservableCollection<ShelfStock>>(json);
            return stock?.Sum(s => s.Quantity) ?? 0;
        }





        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}