using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SteamHub.ApiContract.Models.User;
using SteamHub.ApiContract.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteamHub.ViewModels
{
    public partial class UsersViewModel : ObservableObject
    {
        // Singleton instance to ensure only one ViewModel exists for the Users page
        private static UsersViewModel userViewModelInstance;
        private readonly IUserService userService;
        private static Random randomUserIdentifier = new Random();

        // ObservableCollection automatically notifies the UI when items are added/removed
        // The [ObservableProperty] attribute generates the property with INotifyPropertyChanged implementation
        [ObservableProperty]
        private ObservableCollection<User> usersList;

        // SelectedUser property for tracking the currently selected user in the DataGrid
        // The [ObservableProperty] attribute ensures UI updates when selection changes
        [ObservableProperty]
        private User selectedUser;

        public static void Init(IUserService service)
        {
            if (userViewModelInstance == null)
            {
                userViewModelInstance = new UsersViewModel(service);
            }
        }

        public static UsersViewModel Instance => userViewModelInstance ?? throw new InvalidOperationException("UsersViewModel not initialized. Call Init() first.");

        public UsersViewModel(IUserService userService)
        {
            this.userService = userService ?? throw new ArgumentNullException(nameof(userService));
            usersList = new ObservableCollection<User>();
            _ = LoadUsersAsync(); // fire and forget async call
        }

        public async Task LoadUsersAsync()
        {
            try
            {
                var users = await userService.GetAllUsersAsync();
                usersList.Clear();
                foreach (var user in users)
                {
                    usersList.Add(user);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[UsersViewModel] Error loading users: {ex.Message}");
            }
        }

        [RelayCommand]
        private async Task AddRandomUserAsync()
        {
            var randomUser = new User
            {
                Username = $"RandomUser_{randomUserIdentifier.Next(1000)}",
                Email = $"random{randomUserIdentifier.Next(1000)}@example.com",
                Password = "RandomPassword123",
                //IsDeveloper = randomUserIdentifier.Next(2) == 1,
                CreatedAt = DateTime.Now
            };

            try
            {
                var createdUser = await userService.CreateUserAsync(randomUser);
                if (createdUser != null)
                {
                    usersList.Add(createdUser);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[UsersViewModel] Error adding user: {ex.Message}");
            }
        }

        public async Task<User?> GetCurrentUserAsync()
        {
            return await userService.GetCurrentUserAsync();
        }
    }
}
