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
using warehouse.Models;

namespace warehouse.ViewModels
{
    class ShelvesViewModel : INotifyPropertyChanged
    {
        private const string JsonFilePath = "shelves.json";

        public ObservableCollection<Shelf> Shelves { get; set; } = new ObservableCollection<Shelf>();
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

        public RelayCommand DeleteShelfCommand { get; }

        public ShelvesViewModel()
        {
            LoadShelves();
            DeleteShelfCommand = new RelayCommand(o => DeleteShelf(SelectedShelf));
        }

        public void LoadShelves()
        {
            if (!File.Exists(JsonFilePath)) return;

            var json = File.ReadAllText(JsonFilePath);
            var loadedItems = JsonSerializer.Deserialize<ObservableCollection<Shelf>>(json);
            if (loadedItems != null)
            {
                Shelves.Clear();
                foreach (var item in loadedItems)
                    Shelves.Add(item);
            }
        }

        public void SaveJson()
        {
            var json = JsonSerializer.Serialize(Shelves, new JsonSerializerOptions
            {
                WriteIndented = true
            });

            File.WriteAllText(JsonFilePath, json);
        }

        private void DeleteShelf(Shelf SelectedShelf)
        {
            if (SelectedShelf == null) return;
            Shelves.Remove(SelectedShelf);      
            SaveJson();
        }



        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
