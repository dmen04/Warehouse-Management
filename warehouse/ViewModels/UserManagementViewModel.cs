using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Windows;
using warehouse.Models;

namespace warehouse.ViewModels
{
    class UserManagementViewModel : INotifyPropertyChanged
    {
        private const string UsersJsonFilePath = "users.json";

        public ObservableCollection<string> Roles { get; set; } = new ObservableCollection<string>
        {
            "Admin",
            "Worker"
        };

        public ObservableCollection<User> Users { get; set; } = new ObservableCollection<User>();

        private User _selectedUser;
        public User SelectedUser
        {
            get => _selectedUser;
            set
            {
                _selectedUser = value;
                OnPropertyChanged();
                if (value != null)
                {
                    EditUsername = value.Username;
                    EditPassword = value.Password;
                    EditRole = value.Role;
                }
            }
        }

        private string _newUsername;
        public string NewUsername
        {
            get => _newUsername;
            set { _newUsername = value; OnPropertyChanged(); }
        }

        private string _newPassword;
        public string NewPassword
        {
            get => _newPassword;
            set { _newPassword = value; OnPropertyChanged(); }
        }

        private string _newRole;
        public string NewRole
        {
            get => _newRole;
            set { _newRole = value; OnPropertyChanged(); }
        }

        private string _editUsername;
        public string EditUsername
        {
            get => _editUsername;
            set { _editUsername = value; OnPropertyChanged(); }
        }

        private string _editPassword;
        public string EditPassword
        {
            get => _editPassword;
            set { _editPassword = value; OnPropertyChanged(); }
        }

        private string _editRole;
        public string EditRole
        {
            get => _editRole;
            set { _editRole = value; OnPropertyChanged(); }
        }

        public RelayCommand AddUserCommand { get; }
        public RelayCommand UpdateUserCommand { get; }
        public RelayCommand DeleteUserCommand { get; }

        public UserManagementViewModel()
        {
            LoadUsers();
            AddUserCommand = new RelayCommand(o => AddUser());
            UpdateUserCommand = new RelayCommand(o => UpdateUser());
            DeleteUserCommand = new RelayCommand(o => DeleteUser());
        }

        private void LoadUsers()
        {
            if (!File.Exists(UsersJsonFilePath)) return;

            var json = File.ReadAllText(UsersJsonFilePath);
            var loadedUsers = JsonSerializer.Deserialize<ObservableCollection<User>>(json);
            if (loadedUsers != null)
            {
                Users.Clear();
                foreach (var user in loadedUsers)
                    Users.Add(user);
            }
        }

        private void SaveUsers()
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            File.WriteAllText(UsersJsonFilePath, JsonSerializer.Serialize(Users, options));
        }

        private void AddUser()
        {
            if (string.IsNullOrEmpty(NewUsername) || string.IsNullOrEmpty(NewPassword) || string.IsNullOrEmpty(NewRole))
            {
                MessageBox.Show("Please fill in all fields.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (Users.Any(u => u.Username == NewUsername))
            {
                MessageBox.Show("Username already exists.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var newUser = new User
            {
                Id = Users.Count > 0 ? Users.Max(u => u.Id) + 1 : 1,
                Username = NewUsername,
                Password = NewPassword,
                Role = NewRole
            };

            Users.Add(newUser);
            SaveUsers();

            MessageBox.Show("User added successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

            NewUsername = string.Empty;
            NewPassword = string.Empty;
            NewRole = string.Empty;
        }

        private void UpdateUser()
        {
            if (SelectedUser == null)
            {
                MessageBox.Show("Please select a user to update.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (string.IsNullOrEmpty(EditUsername) || string.IsNullOrEmpty(EditPassword) || string.IsNullOrEmpty(EditRole))
            {
                MessageBox.Show("Please fill in all fields.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            SelectedUser.Username = EditUsername;
            SelectedUser.Password = EditPassword;
            SelectedUser.Role = EditRole;

            SaveUsers();
            LoadUsers();

            MessageBox.Show("User updated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void DeleteUser()
        {
            if (SelectedUser == null)
            {
                MessageBox.Show("Please select a user to delete.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var result = MessageBox.Show($"Are you sure you want to delete user '{SelectedUser.Username}'?",
                "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                Users.Remove(SelectedUser);
                SaveUsers();
                MessageBox.Show("User deleted successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }





        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}