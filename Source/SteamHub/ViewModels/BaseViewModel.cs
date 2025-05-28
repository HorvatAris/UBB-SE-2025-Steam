using System;
using System.Diagnostics;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using SteamHub.ApiContract.Models.User;
using SteamHub.ApiContract.Services.Interfaces;

namespace SteamHub.ViewModels
{
    public abstract class BaseViewModel : ObservableObject
    {
        protected readonly IUserService userService;
        private User currentUser;

        protected BaseViewModel(IUserService userService, User currentUser)
        {
            this.userService = userService ?? throw new ArgumentNullException(nameof(userService));
            this.currentUser = currentUser ?? throw new ArgumentNullException(nameof(currentUser));
            Debug.WriteLine($"Initialized {GetType().Name} with user: {currentUser.Username}");
        }

        public User CurrentUser
        {
            get => currentUser;
            protected set
            {
                if (SetProperty(ref currentUser, value))
                {
                    Debug.WriteLine($"Updated user in {GetType().Name}: {value?.Username ?? "null"}");
                    OnUserChanged();
                }
            }
        }

        protected virtual void OnUserChanged()
        {
            // Override in derived classes to handle user changes
            Debug.WriteLine($"User changed in {GetType().Name}");
        }

        protected async Task RefreshUserAsync()
        {
            try
            {
                Debug.WriteLine($"{GetType().Name}: Refreshing user data");
                var updatedUser = await userService.GetCurrentUserAsync();
                if (updatedUser != null)
                {
                    CurrentUser = updatedUser;
                    Debug.WriteLine($"{GetType().Name}: User data refreshed successfully");
                }
                else
                {
                    Debug.WriteLine($"{GetType().Name}: Failed to refresh user data - GetCurrentUserAsync returned null");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"{GetType().Name}: Error refreshing user data - {ex.Message}");
                throw;
            }
        }
    }
} 