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
    class StockViewModel : INotifyPropertyChanged
    {
        private const string StockJsonFilePath = "shelfstock.json";
        private const string ItemsJsonFilePath = "items.json";

        private ObservableCollection<Item> _items = new ObservableCollection<Item>();

        private Shelf _shelf;
        public Shelf Shelf
        {
            get => _shelf;
            set
            {
                _shelf = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<ShelfStock> StockItems { get; set; } = new ObservableCollection<ShelfStock>();


        private ShelfStock _selectedStockItem;
        public ShelfStock SelectedStockItem
        {
            get => _selectedStockItem;
            set
            {
                _selectedStockItem = value;
                OnPropertyChanged();
            }
        }
        public RelayCommand DeleteStockCommand { get; }


        public StockViewModel(Shelf shelf)
        {
            Shelf = shelf;
            LoadItems();
            LoadStock();
            DeleteStockCommand = new RelayCommand(o => DeleteStock(SelectedStockItem));
        }

        private void LoadItems()
        {
            if (!File.Exists(ItemsJsonFilePath)) return;

            var json = File.ReadAllText(ItemsJsonFilePath);
            var loadedItems = JsonSerializer.Deserialize<ObservableCollection<Item>>(json);
            if (loadedItems != null)
            {
                _items = loadedItems;
            }
        }

        private void LoadStock()
        {
            StockItems.Clear();

            if (!File.Exists(StockJsonFilePath))
                return;

            var json = File.ReadAllText(StockJsonFilePath);
            var stockList = JsonSerializer.Deserialize<ObservableCollection<ShelfStock>>(json);

            if (stockList == null)
                return;

            var shelfStock = stockList.Where(s => s.ShelfId == Shelf.Id);

            foreach (var stock in shelfStock)
            {
                var item = _items.FirstOrDefault(i => i.Id == stock.ItemId);
                stock.ItemName = item?.Name ?? "Unknown";
                StockItems.Add(stock);
            }
        }

        private void SaveJson()
        {
            if (!File.Exists(StockJsonFilePath))
                return;

            var json = File.ReadAllText(StockJsonFilePath);
            var allStock = JsonSerializer.Deserialize<ObservableCollection<ShelfStock>>(json);

            if (allStock == null)
                allStock = new ObservableCollection<ShelfStock>();

            var itemsToRemove = allStock.Where(s => s.ShelfId == Shelf.Id).ToList();
            foreach (var item in itemsToRemove)
                allStock.Remove(item);

            foreach (var item in StockItems)
                allStock.Add(item);

            var options = new JsonSerializerOptions { WriteIndented = true };
            File.WriteAllText(StockJsonFilePath, JsonSerializer.Serialize(allStock, options));
        }

        private void DeleteStock(ShelfStock selectedStockItem)
        {
            if (selectedStockItem == null) return;

            StockItems.Remove(selectedStockItem);
            SaveJson();
        }



        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}