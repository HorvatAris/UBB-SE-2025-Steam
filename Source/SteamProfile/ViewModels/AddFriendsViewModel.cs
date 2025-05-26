using System;
using System.Diagnostics;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml.Controls;
using BusinessLayer.Models;
using BusinessLayer.Services.Interfaces;
using BusinessLayer.Exceptions;
using SteamProfile.Views;
using Windows.System;

namespace SteamProfile.ViewModels
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

        [RelayCommand]
        public void LoadFriends()
        {
            try
            {
                IsLoading = true;
                ErrorMessage = null;

                var users = userService.GetAllUsers();
                var friendships = friendsService.GetAllFriendships();

                possibleFriendships.Clear();

                foreach (var user in users)
                {
                    if (user.UserId == App.CurrentUser.UserId)
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
        public void AddFriend(int user_id)
        {
            try
            {
                IsLoading = true;
                ErrorMessage = null;

                friendsService.AddFriend(App.CurrentUser.UserId, user_id);
                LoadFriends();
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
        public void RemoveFriend(int user_id)
        {
            try
            {
                IsLoading = true;
                ErrorMessage = null;

                var friendships = friendsService.GetAllFriendships();
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
                    friendsService.RemoveFriend(idx);
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
                LoadFriends();
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
