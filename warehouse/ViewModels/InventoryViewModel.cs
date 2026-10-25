using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Runtime.CompilerServices;
using warehouse.Models;

namespace warehouse.ViewModels
{
    class InventoryViewModel : INotifyPropertyChanged
    {
        private const string JsonFilePath = "items.json";
        private const string CategoriesJsonFilePath = "categories.json";

        private ObservableCollection<Category> _categories = new ObservableCollection<Category>();
        public ObservableCollection<Item> Items { get; set; } = new ObservableCollection<Item>();


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

        public RelayCommand DeleteItemCommand { get; }

        public InventoryViewModel()
        {
            LoadCategories();
            LoadItems();
            DeleteItemCommand = new RelayCommand(o => DeleteItem(SelectedItem));
        }


        private void LoadCategories()
        {
            if (!File.Exists(CategoriesJsonFilePath)) return;

            var json = File.ReadAllText(CategoriesJsonFilePath);
            var loadedCategories = JsonSerializer.Deserialize<ObservableCollection<Category>>(json);
            if (loadedCategories != null)
            {
                _categories = loadedCategories;
            }
        }

        private void LoadItems()
        {
            if (!File.Exists(JsonFilePath)) return;

            var json = File.ReadAllText(JsonFilePath);
            var loadedItems = JsonSerializer.Deserialize<ObservableCollection<Item>>(json);

            if (loadedItems != null)
            {
                Items.Clear();
                foreach (var item in loadedItems)
                {
                    var category = _categories.FirstOrDefault(c => c.Id == item.CategoryId);
                    item.CategoryName = category?.Name ?? "Unknown";
                    Items.Add(item);
                }
            }
        }

        public void SaveJson()
        {
            var json = JsonSerializer.Serialize(Items, new JsonSerializerOptions
            {
                WriteIndented = true
            });

            File.WriteAllText(JsonFilePath, json);
        }

        private void DeleteItem(Item SelectedItem)
        {
            if (SelectedItem == null) return;
            Items.Remove(SelectedItem);
            SaveJson();
        }







        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}