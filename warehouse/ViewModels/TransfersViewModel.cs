using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Windows.Input;
using warehouse.Models;

namespace warehouse.ViewModels
{
    class TransfersViewModel : INotifyPropertyChanged
    {
        private const string ItemsJsonFilePath = "items.json";
        private const string ShelvesJsonFilePath = "shelves.json";
        private const string StockJsonFilePath = "shelfstock.json";

        public ObservableCollection<Item> Items { get; set; } = new ObservableCollection<Item>();
        public ObservableCollection<Shelf> Shelves { get; set; } = new ObservableCollection<Shelf>();

        private Item _selectedItem;
        public Item SelectedItem
        {
            get => _selectedItem;
            set { _selectedItem = value; OnPropertyChanged(); }
        }

        private Shelf _fromShelf;
        public Shelf FromShelf
        {
            get => _fromShelf;
            set { _fromShelf = value; OnPropertyChanged(); }
        }

        private Shelf _toShelf;
        public Shelf ToShelf
        {
            get => _toShelf;
            set { _toShelf = value; OnPropertyChanged(); }
        }

        private string _quantity;
        public string Quantity
        {
            get => _quantity;
            set { _quantity = value; OnPropertyChanged(); }
        }

        public ICommand TransferCommand { get; }

        public TransfersViewModel()
        {
            LoadItems();
            LoadShelves();
            TransferCommand = new RelayCommand(o => Transfer());
        }

        private void LoadItems()
        {
            if (!File.Exists(ItemsJsonFilePath)) return;

            var json = File.ReadAllText(ItemsJsonFilePath);
            var loadedItems = JsonSerializer.Deserialize<ObservableCollection<Item>>(json);
            if (loadedItems != null)
            {
                Items.Clear();
                foreach (var item in loadedItems)
                    Items.Add(item);
            }
        }

        private void LoadShelves()
        {
            if (!File.Exists(ShelvesJsonFilePath)) return;

            var json = File.ReadAllText(ShelvesJsonFilePath);
            var loadedShelves = JsonSerializer.Deserialize<ObservableCollection<Shelf>>(json);
            if (loadedShelves != null)
            {
                Shelves.Clear();
                foreach (var shelf in loadedShelves)
                    Shelves.Add(shelf);
            }
        }

        private void Transfer()
        {
            if (SelectedItem == null || FromShelf == null || ToShelf == null || string.IsNullOrEmpty(Quantity))
                return;

            if (FromShelf.Id == ToShelf.Id)
                return;

            if (!int.TryParse(Quantity, out int qty) || qty <= 0)
                return;



            var stockList = new ObservableCollection<ShelfStock>();
            if (File.Exists(StockJsonFilePath))
            {
                var json = File.ReadAllText(StockJsonFilePath);
                var loaded = JsonSerializer.Deserialize<ObservableCollection<ShelfStock>>(json);
                if (loaded != null)
                    stockList = loaded;
            }



            var fromStock = stockList.FirstOrDefault(s => s.ShelfId == FromShelf.Id && s.ItemId == SelectedItem.Id);

            if (fromStock == null || fromStock.Quantity < qty)
                return;

            fromStock.Quantity -= qty;

            if (fromStock.Quantity == 0)
                stockList.Remove(fromStock);



            var toStock = stockList.FirstOrDefault(s => s.ShelfId == ToShelf.Id && s.ItemId == SelectedItem.Id);
            if (toStock != null)
            {
                toStock.Quantity += qty;
            }
            else
            {
                stockList.Add(new ShelfStock
                {
                    ShelfId = ToShelf.Id,
                    ItemId = SelectedItem.Id,
                    Quantity = qty
                });
            }



            var options = new JsonSerializerOptions { WriteIndented = true };
            File.WriteAllText(StockJsonFilePath, JsonSerializer.Serialize(stockList, options));



            SelectedItem = null;
            FromShelf = null;
            ToShelf = null;
            Quantity = string.Empty;
        }





        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}