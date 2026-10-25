using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Text.Json;
using System.Runtime.CompilerServices;
using warehouse.Models;

namespace warehouse.ViewModels
{
    class CategoriesViewModel : INotifyPropertyChanged
    {
        private const string JsonFilePath = "categories.json";

        public ObservableCollection<Category> Categories { get; set; } = new ObservableCollection<Category>();
        private Category _selectedCategory;
        public Category SelectedCategory
        {
            get => _selectedCategory;
            set
            {
                _selectedCategory = value;
                OnPropertyChanged();
            }
        }

        public RelayCommand DeleteCategoryCommand { get; }

        public CategoriesViewModel()
        {
            LoadCategories();
            DeleteCategoryCommand = new RelayCommand(o => DeleteCategory(SelectedCategory));
        }

        private void LoadCategories()
        {
            if (!File.Exists(JsonFilePath)) return;

            var json = File.ReadAllText(JsonFilePath);
            var loadedCategories = JsonSerializer.Deserialize<ObservableCollection<Category>>(json);
            if (loadedCategories != null)
            {
                Categories.Clear();
                foreach (var category in loadedCategories)
                    Categories.Add(category);
            }
        }

        public void SaveJson()
        {
            var json = JsonSerializer.Serialize(Categories, new JsonSerializerOptions
            {
                WriteIndented = true
            });

            File.WriteAllText(JsonFilePath, json);
        }

        private void DeleteCategory(Category SelectedCategory)
        {
            if (SelectedCategory == null) return;
            Categories.Remove(SelectedCategory);
            SaveJson();
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}