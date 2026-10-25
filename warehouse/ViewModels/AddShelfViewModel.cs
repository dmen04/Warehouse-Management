using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using warehouse.Models;

namespace warehouse.ViewModels
{
    class AddShelfViewModel : INotifyPropertyChanged
    {
        private const string JsonFilePath = "shelves.json";

        private string _name;
        public string Name
        {
            get => _name;
            set { _name = value; OnPropertyChanged(); }
        }

        public ICommand AddShelfCommand { get; }
        public ICommand CancelCommand { get; }
        public ICommand ClearFormCommand { get; }

        public AddShelfViewModel()
        {
            AddShelfCommand = new RelayCommand(o => AddShelf());
            CancelCommand = new RelayCommand(o => Cancel());
            ClearFormCommand = new RelayCommand(o => ClearForm());
        }

        private void AddShelf()
        {
            var shelves = new ObservableCollection<Shelf>();

            if (File.Exists(JsonFilePath))
            {
                var json = File.ReadAllText(JsonFilePath);
                var existingShelves = JsonSerializer.Deserialize<ObservableCollection<Shelf>>(json);
                if (existingShelves != null)
                    shelves = existingShelves;
            }

            var newShelf = new Shelf
            {
                Id = shelves.Count > 0 ? shelves[^1].Id + 1 : 1,
                Name = this.Name
            };

            shelves.Add(newShelf);

            var options = new JsonSerializerOptions { WriteIndented = true };
            File.WriteAllText(JsonFilePath, JsonSerializer.Serialize(shelves, options));

            MessageBox.Show("Shelf added successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

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