using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Windows;
using System.Windows.Input;
using warehouse.Models;

namespace warehouse.ViewModels
{
    class AddStockViewModel : INotifyPropertyChanged
    {
        private const string StockJsonFilePath = "shelfstock.json";
        private const string ItemsJsonFilePath = "items.json";

        public ObservableCollection<Item> AvailableItems { get; set; } = new ObservableCollection<Item>();

        private Item _selectedItem;
        public Item SelectedItem
        {
            get => _selectedItem;
            set
            {
                _selectedItem = value;
                OnPropertyChanged();
            }
        }

        private Shelf _selectedShelf;
        public Shelf SelectedShelf
        {
            get => _selectedShelf;
            set
            {
                _selectedShelf = value;
                OnPropertyChanged();
            }
        }

        private string _quantity;
        public string Quantity
        {
            get => _quantity;
            set
            {
                _quantity = value;
                OnPropertyChanged();
            }
        }

        public ICommand AddStockCommand { get; }
        public ICommand CancelCommand { get; }
        public ICommand ClearFormCommand { get; }

        public AddStockViewModel(Shelf shelf)
        {
            SelectedShelf = shelf;
            LoadItems();

            AddStockCommand = new RelayCommand(o => AddStock());
            CancelCommand = new RelayCommand(o => Cancel());
            ClearFormCommand = new RelayCommand(o => ClearForm());
        }

        private void LoadItems()
        {
            if (!File.Exists(ItemsJsonFilePath))
                return;

            var json = File.ReadAllText(ItemsJsonFilePath);
            var items = JsonSerializer.Deserialize<ObservableCollection<Item>>(json);

            if (items != null)
            {
                AvailableItems.Clear();
                foreach (var item in items)
                    AvailableItems.Add(item);
            }
        }

        private void AddStock()
        {
            if (SelectedItem == null || SelectedShelf == null || string.IsNullOrEmpty(Quantity))
                return;

            var stockList = new ObservableCollection<ShelfStock>();

            if (File.Exists(StockJsonFilePath))
            {
                var json = File.ReadAllText(StockJsonFilePath);
                var loaded = JsonSerializer.Deserialize<ObservableCollection<ShelfStock>>(json);
                if (loaded != null)
                    stockList = loaded;
            }

            var existing = stockList.FirstOrDefault(s => s.ShelfId == SelectedShelf.Id && s.ItemId == SelectedItem.Id);

            if (existing != null)
            {
                existing.Quantity += int.Parse(Quantity);
            }
            else
            {
                stockList.Add(new ShelfStock
                {
                    ShelfId = SelectedShelf.Id,
                    ItemId = SelectedItem.Id,
                    Quantity = int.Parse(Quantity)
                });
            }

            var options = new JsonSerializerOptions { WriteIndented = true };
            File.WriteAllText(StockJsonFilePath, JsonSerializer.Serialize(stockList, options));

            MessageBox.Show("Stock added successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

            SelectedItem = null;
            Quantity = string.Empty;
        }

        private void Cancel()
        {
            SelectedItem = null;
            Quantity = string.Empty;
        }

        private void ClearForm()
        {
            SelectedItem = null;
            Quantity = string.Empty;
        }









        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}