using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using warehouse.Models;
using System.IO;

namespace warehouse.ViewModels
{
    class AddCategoryViewModel : INotifyPropertyChanged
    {
        private const string JsonFilePath = "categories.json";

        private string _name;
        public string Name
        {
            get => _name;
            set { _name = value; OnPropertyChanged(); }
        }

        public ICommand AddCategoryCommand { get; }
        public ICommand CancelCommand { get; }
        public ICommand ClearFormCommand { get; }

        public AddCategoryViewModel()
        {
            AddCategoryCommand = new RelayCommand(o => AddCategory());
            CancelCommand = new RelayCommand(o => Cancel());
            ClearFormCommand = new RelayCommand(o => ClearForm());
        }

        private void AddCategory()
        {
            var categories = new ObservableCollection<Category>();

            if (File.Exists(JsonFilePath))
            {
                var json = File.ReadAllText(JsonFilePath);
                var existingCategories = JsonSerializer.Deserialize<ObservableCollection<Category>>(json);
                if (existingCategories != null)
                    categories = existingCategories;
            }

            var newCategory = new Category
            {
                Id = categories.Count > 0 ? categories[^1].Id + 1 : 1,
                Name = this.Name
            };

            categories.Add(newCategory);

            var options = new JsonSerializerOptions { WriteIndented = true };
            File.WriteAllText(JsonFilePath, JsonSerializer.Serialize(categories, options));

            MessageBox.Show("Category added successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

            Name = string.Empty;
        }

        private void Cancel()
        {
            Name = string.Empty;
        }

        private void ClearForm()
        {
            Name = string.Empty;
        }





        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}