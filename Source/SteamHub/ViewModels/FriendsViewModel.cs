using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SteamHub.ApiContract.Exceptions;
using SteamHub.ApiContract.Services.Interfaces;
using System;
using SteamHub.ApiContract.Models;
using System.Collections.ObjectModel;
using SteamHub.ApiContract.Models.User;
using System.Threading.Tasks;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml;
using System.Linq;
using SteamHub.Pages;

namespace SteamHub.ViewModels
{
    public partial class FriendsViewModel : ObservableObject
    {
        #region Constants
        private const string ErrorLoadFriends = "Error loading friends: ";
        private const string ErrorUnexpectedLoadFriends = "Unexpected error loading friends: ";
        private const string ErrorRemoveFriend = "Error removing friend: ";
        private const string ErrorUnexpectedRemoveFriend = "Unexpected error removing friend: ";
        private const string ErrorDetailsPrefix = "\nDetails: ";
        #endregion

        private readonly IFriendsService friendsService;
        private readonly IUserDetails user;

        [ObservableProperty]
        private ObservableCollection<Friendship> friendships = new ObservableCollection<Friendship>();

        [ObservableProperty]
        private Friendship selectedFriendship;

        [ObservableProperty]
        private bool isLoading;

        [ObservableProperty]
        private string errorMessage;

        public FriendsViewModel(IFriendsService friendsService)
        {
            this.friendsService = friendsService ?? throw new ArgumentNullException(nameof(friendsService));
            this.user = this.friendsService.GetUser();
            _ = InitializeAsync();
        }

        public async Task InitializeAsync()
        {
            await LoadFriendsAsync();
        }

        [RelayCommand]
        public async Task LoadFriendsAsync()
        {
            try
            {
                IsLoading = true;
                ErrorMessage = null;

                var friendships = await friendsService.GetAllFriendshipsAsync(user.UserId);

                Friendships.Clear();
                foreach (var friendship in friendships)
                {
                    Friendships.Add(friendship);
                }
            }
            catch (ServiceException serviceException)
            {
                ErrorMessage = ErrorLoadFriends + serviceException.Message;
                if (serviceException.InnerException != null)
                {
                    ErrorMessage += ErrorDetailsPrefix + serviceException.InnerException.Message;
                }
            }
            catch (Exception generalException)
            {
                ErrorMessage = ErrorUnexpectedLoadFriends + generalException.Message;
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        private async Task RemoveFriendship(int friendshipId)
        {
            try
            {
                IsLoading = true;
                ErrorMessage = null;

                await friendsService.RemoveFriendAsync(friendshipId);

                var friendshipToRemove = Friendships.FirstOrDefault(f => f.FriendshipId == friendshipId);
                if (friendshipToRemove != null)
                {
                    Friendships.Remove(friendshipToRemove);
                }
            }
            catch (ServiceException serviceException)
            {
                ErrorMessage = ErrorRemoveFriend + serviceException.Message;
                if (serviceException.InnerException != null)
                {
                    ErrorMessage += ErrorDetailsPrefix + serviceException.InnerException.Message;
                }
            }
            catch (Exception generalException)
            {
                ErrorMessage = ErrorUnexpectedRemoveFriend + generalException.Message;
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        public void SelectFriendship(Friendship friendship)
        {
            SelectedFriendship = friendship;
        }

        [RelayCommand]
        public void ViewFriend(int friendId)
        {
            // Navigation logic will be handled in code-behind or via a navigation service
            // Parameter is passed from XAML
            NavigationService.Instance.Navigate(typeof(FriendProfilePage), friendId);

        }

        [RelayCommand]
        public void ChatFriend(int friendId)
        {
            // Chat window opening logic
            // Parameter is passed from XAML
        }

        [RelayCommand]
        public void BackToProfile()
        {
            // Navigation back to profile
        }

        public async Task<bool> AreUsersFriendsAsync(int otherUserId)
        {
            try
            {
                return await friendsService.AreUsersFriendsAsync(user.UserId, otherUserId);
            }
            catch (ServiceException)
            {
                return false;
            }
        }
    }
}