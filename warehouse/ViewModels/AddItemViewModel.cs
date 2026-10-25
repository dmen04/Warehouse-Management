using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Windows;
using System.Windows.Input;
using warehouse.Models;

namespace warehouse.ViewModels
{
    class AddItemViewModel : INotifyPropertyChanged
    {
        private const string JsonFilePath = "items.json";

        private string _name;
        public string Name
        {
            get => _name;
            set { _name = value; OnPropertyChanged(); }
        }

        public ObservableCollection<Category> Categories { get; set; }

        private Category _selectedCategory;
        public Category SelectedCategory
        {
            get => _selectedCategory;
            set { _selectedCategory = value; OnPropertyChanged(); }
        }
        private string _volume;
        public string Volume
        {
            get => _volume;
            set { _volume = value; OnPropertyChanged(); }
        }

        private bool _isUnitPcs;
        public bool IsUnitPcs
        {
            get => _isUnitPcs;
            set
            {
                _isUnitPcs = value;
                if (value) SelectedUnit = "pcs";
                OnPropertyChanged();
            }
        }

        private bool _isUnitKg;
        public bool IsUnitKg
        {
            get => _isUnitKg;
            set
            {
                _isUnitKg = value;
                if (value) SelectedUnit = "kg";
                OnPropertyChanged();
            }
        }

        private bool _isUnitBox;
        public bool IsUnitBox
        {
            get => _isUnitBox;
            set
            {
                _isUnitBox = value;
                if (value) SelectedUnit = "box";
                OnPropertyChanged();
            }
        }

        private bool _isUnitLitre;
        public bool IsUnitLitre
        {
            get => _isUnitLitre;
            set
            {
                _isUnitLitre = value;
                if (value) SelectedUnit = "Litre";
                OnPropertyChanged();
            }
        }

        private string SelectedUnit { get; set; } = "pcs";

        public ICommand AddItemCommand { get; }
        public ICommand CancelCommand { get; }
        public ICommand ClearFormCommand { get; }

        public AddItemViewModel()
        {
            Categories = new ObservableCollection<Category>();
            LoadCategories();

            AddItemCommand = new RelayCommand(o => AddItem());
            CancelCommand = new RelayCommand(o => Cancel());
            ClearFormCommand = new RelayCommand(o => ClearForm());
        }

        private void LoadCategories()
        {
            if (!File.Exists("categories.json"))
                return;

            var json = File.ReadAllText("categories.json");
            var loadedCategories = JsonSerializer.Deserialize<ObservableCollection<Category>>(json);

            if (loadedCategories != null)
            {
                Categories.Clear();
                foreach (var category in loadedCategories)
                    Categories.Add(category);

                if (Categories.Count > 0)
                    SelectedCategory = Categories[0];
            }
        }

        private void AddItem()
        {
            var items = new ObservableCollection<Item>();
            if (File.Exists(JsonFilePath))
            {
                var json = File.ReadAllText(JsonFilePath);
                var existingItems = JsonSerializer.Deserialize<ObservableCollection<Item>>(json);
                if (existingItems != null)
                    items = existingItems;
            }

            var newItem = new Item
            {
                Id = items.Count > 0 ? items[^1].Id + 1 : 1,
                Name = this.Name,
                CategoryId = this.SelectedCategory?.Id ?? 0,
                Volume = decimal.TryParse(this.Volume, out var v) ? v : 0,
                Unit = SelectedUnit
            };
            items.Add(newItem);

            var options = new JsonSerializerOptions { WriteIndented = true };
            File.WriteAllText(JsonFilePath, JsonSerializer.Serialize(items, options));

            MessageBox.Show("Item added successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

            Name = string.Empty;
            if (Categories.Count > 0)
                SelectedCategory = Categories[0];
            Volume = string.Empty;
            IsUnitPcs = true;
            IsUnitKg = IsUnitBox = IsUnitLitre = false;
        }

        private void Cancel()
        {
            Name = string.Empty;
            if (Categories.Count > 0)
                SelectedCategory = Categories[0];
            IsUnitPcs = true;
            IsUnitKg = IsUnitBox = IsUnitLitre = false;
        }

        private void ClearForm()
        {
            Name = string.Empty;
            Volume = string.Empty;
            if (Categories.Count > 0)
                SelectedCategory = Categories[0];
            IsUnitPcs = true;
            IsUnitKg = IsUnitBox = IsUnitLitre = false;
        }





        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}