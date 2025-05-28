using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.WinUI.UI.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SteamHub.ApiContract.Exceptions;
using SteamHub.ApiContract.Models;
using SteamHub.ApiContract.Models.User;
using SteamHub.ApiContract.Services.Interfaces;

namespace SteamHub.ViewModels
{
    public partial class AddFriendsViewModel : ObservableObject
    {
        #region Constants
        // Error message constants
        private const string ErrorLoadFriends = "Error loading friends: ";
        private const string ErrorUnexpectedLoadFriends = "Unexpected error loading friends: ";
        private const string ErrorRemoveFriend = "Error removing friend: ";
        private const string ErrorUnexpectedRemoveFriend = "Unexpected error removing friend: ";
        private const string ErrorDetailsPrefix = "\nDetails: ";
        #endregion

        private readonly IFriendsService friendsService;
        private readonly IUserService userService;

        [ObservableProperty]
        private ObservableCollection<PossibleFriendship> possibleFriendships = new ObservableCollection<PossibleFriendship>();

        [ObservableProperty]
        private Friendship selectedFriendship;

        [ObservableProperty]
        private bool isLoading;

        [ObservableProperty]
        private string errorMessage;

        public AddFriendsViewModel(IFriendsService friendsService, IUserService userService)
        {
            this.friendsService = friendsService ?? throw new ArgumentNullException(nameof(friendsService));
            this.userService = userService ?? throw new ArgumentNullException(nameof(userService));
        }

        public User GetCurrentUser()
        {
            return userService.GetCurrentUser();
        }

        [RelayCommand]
        public async Task LoadFriendsAsync()
        {
            try
            {
                IsLoading = true;
                ErrorMessage = null;

                var users = await userService.GetAllUsersAsync();
                var friendships = await friendsService.GetAllFriendshipsAsync(userService.GetCurrentUser().UserId);

                possibleFriendships.Clear();

                foreach (var user in users)
                {
                    if (user.UserId == userService.GetCurrentUser().UserId)
                    {
                        continue;
                    }
                    Friendship found = null;
                    foreach (var friendship in friendships)
                    {
                        if (friendship.FriendId == user.UserId)
                        {
                            found = friendship;
                            break;
                        }
                    }
                    var possibleFriendship = new PossibleFriendship
                    {
                        User = user,
                        IsFriend = found != null,
                        Friendship = found
                    };
                    possibleFriendships.Add(possibleFriendship);
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
        public async Task AddFriendAsync(int user_id)
        {
            try
            {
                IsLoading = true;
                ErrorMessage = null;

                await friendsService.AddFriendAsync(userService.GetCurrentUser().UserId, user_id);
                await LoadFriendsAsync();
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
        public async Task RemoveFriendAsync(int user_id)
        {
            try
            {
                IsLoading = true;
                ErrorMessage = null;

                var friendships = await friendsService.GetAllFriendshipsAsync(userService.GetCurrentUser().UserId);
                int idx = -1;
                foreach (var friendship in friendships)
                {
                    if (friendship.FriendId == user_id)
                    {
                        idx = friendship.FriendshipId;
                        break;
                    }
                }

                if (idx != -1)
                {
                    await friendsService.RemoveFriendAsync(idx);
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
                await LoadFriendsAsync();
                IsLoading = false;
            }
        }

        [RelayCommand]
        public void SelectFriendship(Friendship friendship)
        {
            SelectedFriendship = friendship;
        }
    }
}
