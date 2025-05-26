using System;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using BusinessLayer.Models;
using BusinessLayer.Services;
using BusinessLayer.Services.Interfaces;

namespace SteamProfile.ViewModels
{
    public partial class UsersViewModel : ObservableObject
    {
        // Singleton instance to ensure only one ViewModel exists for the Users page
        private static UsersViewModel userViewModelInstance;
        private readonly IUserService userService;
        private static Random randomUserIdentifer = new Random();

        // ObservableCollection automatically notifies the UI when items are added/removed
        // The [ObservableProperty] attribute generates the property with INotifyPropertyChanged implementation
        [ObservableProperty]
        private ObservableCollection<User> usersList;

        // SelectedUser property for tracking the currently selected user in the DataGrid
        // The [ObservableProperty] attribute ensures UI updates when selection changes
        [ObservableProperty]
        private User selectedUser;

        public static UsersViewModel Instance
        {
            get
            {
                if (userViewModelInstance == null)
                {
                    userViewModelInstance = new UsersViewModel(App.UserService);
                }
                return userViewModelInstance;
            }
        }

        private UsersViewModel(IUserService userService)
        {
            this.userService = userService ?? throw new ArgumentNullException(nameof(userService));
            usersList = new ObservableCollection<User>();
            LoadUsers();
        }

        public void LoadUsers()
        {
            var users = userService.GetAllUsers();
            usersList.Clear();
            foreach (var user in users)
            {
                usersList.Add(user);
            }
        }

        [RelayCommand]
        private void AddRandomUser()
        {
            // Create a new random user
            var randomUser = new User
            {
                Username = $"RandomUser_{randomUserIdentifer.Next(1000)}",
                Email = $"random{randomUserIdentifer.Next(1000)}@example.com",
                Password = "RandomPassword123",
               // Description = "This is a random test user",
                IsDeveloper = randomUserIdentifer.Next(2) == 1,
                CreatedAt = DateTime.Now
            };

            // First save to database through the service
            var createdUser = userService.CreateUser(randomUser);
            // Only update UI if database operation was successful
            if (createdUser != null)
            {
                usersList.Add(createdUser);
            }
        }

        public User? GetCurrentUser()
        {
            return userService.GetCurrentUser();
        }
    }
}