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
    class LoginViewModel : INotifyPropertyChanged
    {
        private const string UsersJsonFilePath = "users.json";

        private string _username;
        public string Username
        {
            get => _username;
            set { _username = value; OnPropertyChanged(); }
        }

        private string _password;
        public string Password
        {
            get => _password;
            set { _password = value; OnPropertyChanged(); }
        }

        public ICommand LoginCommand { get; }

        public event EventHandler<User> LoginSuccessful;

        public LoginViewModel()
        {
            LoginCommand = new RelayCommand(o => Login());
            CreateDefaultUser();
        }

        private void CreateDefaultUser()
        {
            if (File.Exists(UsersJsonFilePath))
                return;

            var users = new ObservableCollection<User>
            {
                new User
                {
                    Id = 1,
                    Username = "admin",
                    Password = "admin",
                    Role = "Admin"
                }
            };

            var options = new JsonSerializerOptions { WriteIndented = true };
            File.WriteAllText(UsersJsonFilePath, JsonSerializer.Serialize(users, options));
        }

        private void Login()
        {
            if (string.IsNullOrEmpty(Username) || string.IsNullOrEmpty(Password))
            {
                MessageBox.Show("Please enter username and password.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!File.Exists(UsersJsonFilePath))
            {
                MessageBox.Show("No users found. Please contact administrator.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var json = File.ReadAllText(UsersJsonFilePath);
            var users = JsonSerializer.Deserialize<ObservableCollection<User>>(json);

            var user = users?.FirstOrDefault(u => u.Username == Username && u.Password == Password);

            if (user != null)
            {
                LoginSuccessful?.Invoke(this, user);
            }
            else
            {
                MessageBox.Show("Invalid username or password.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Password = string.Empty;
            }
        }





        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}