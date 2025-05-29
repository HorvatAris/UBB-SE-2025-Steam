using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using BusinessLayer.Models;
using SteamHub.ApiContract.Models;
using SteamHub.ApiContract.Services.Interfaces;


namespace SteamHub.ViewModels
{
    public class FriendRequestViewModel : INotifyPropertyChanged
    {
        private readonly IFriendRequestService friendRequestService;
        private readonly IFriendsService friendService;
        private readonly IUserService userService;
        private string currentUsername;
        private ObservableCollection<FriendRequest> friendRequests;
        private ObservableCollection<Friend> friends;
        private bool isLoading;

        public FriendRequestViewModel(IFriendRequestService friendRequestService, IUserService userService, IFriendsService friendService)
        {
            this.friendRequestService = friendRequestService ?? throw new ArgumentNullException(nameof(friendRequestService));
            friendService = friendService ?? throw new ArgumentNullException(nameof(friendService)); 
            this.userService = userService ?? throw new ArgumentNullException(nameof(userService));
            SetCurrentUsernameAsync();
            friendRequests = new ObservableCollection<FriendRequest>();
            friends = new ObservableCollection<Friend>();

            // Initialize commands
            AcceptRequestCommand = new RelayCommand<FriendRequest>(AcceptRequest);
            RejectRequestCommand = new RelayCommand<FriendRequest>(RejectRequest);
            RemoveFriendCommand = new RelayCommand<Friend>(RemoveFriend);
            // Load friend requests and friends
            LoadFriendRequestsAsync();
            LoadFriendsAsync();
        }

        private async Task SetCurrentUsernameAsync()
        {
            currentUsername = (await userService.GetCurrentUserAsync()).Username ?? throw new InvalidOperationException("User is not logged in.");
        }

        public ObservableCollection<FriendRequest> FriendRequests
        {
            get => friendRequests;
            set
            {
                if (friendRequests != value)
                {
                    friendRequests = value;
                    OnPropertyChanged();
                }
            }
        }

        public ObservableCollection<Friend> Friends
        {
            get => friends;
            set
            {
                if (friends != value)
                {
                    friends = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool IsLoading
        {
            get => isLoading;
            set
            {
                if (isLoading != value)
                {
                    isLoading = value;
                    OnPropertyChanged();
                }
            }
        }

        // Commands
        public ICommand AcceptRequestCommand { get; }
        public ICommand RejectRequestCommand { get; }
        public ICommand RemoveFriendCommand { get; }

        private async void LoadFriendRequestsAsync()
        {
            try
            {
                IsLoading = true;
                var requests = await friendRequestService.GetFriendRequestsAsync(currentUsername);
                FriendRequests.Clear();
                foreach (var request in requests)
                {
                    FriendRequests.Add(request);
                }
            }
            catch (Exception ex)
            {
                // Handle exception (log or display to user)
                System.Diagnostics.Debug.WriteLine($"Error loading friend requests: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async void LoadFriendsAsync()
        {
            try
            {
                IsLoading = true;
                var friends = await friendService.GetAllFriendshipsAsync((await userService.GetCurrentUserAsync()).UserId);
                Friends.Clear();
                foreach (var friend in friends)
                {
                   // Friends.Add(friend);
                }
            }
            catch (Exception ex)
            {
                // Handle exception (log or display to user)
                System.Diagnostics.Debug.WriteLine($"Error loading friends: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async void AcceptRequest(FriendRequest request)
        {
            if (request == null)
            {
                return;
            }

            try
            {
                bool success = await friendRequestService.AcceptFriendRequestAsync(request.Username, currentUsername);
                if (success)
                {
                    // Remove from the local collection
                    FriendRequests.Remove(request);
                    // Add to friends collection
                    Friends.Add(new Friend
                    {
                        Username = request.Username,
                        Email = request.Email,
                        ProfilePhotoPath = request.ProfilePhotoPath
                    });
                }
            }
            catch (Exception ex)
            {
                // Handle exception (log or display to user)
                System.Diagnostics.Debug.WriteLine($"Error accepting friend request: {ex.Message}");
            }
        }

        private async void RejectRequest(FriendRequest request)
        {
            if (request == null)
            {
                return;
            }

            try
            {
                bool success = await friendRequestService.RejectFriendRequestAsync(request.Username, currentUsername);
                if (success)
                {
                    // Remove from the local collection
                    FriendRequests.Remove(request);
                }
            }
            catch (Exception ex)
            {
                // Handle exception (log or display to user)
                System.Diagnostics.Debug.WriteLine($"Error rejecting friend request: {ex.Message}");
            }
        }

        private async void RemoveFriend(Friend friend)
        {
            if (friend == null)
            {
                return;
            }

            //try
            //{
            //    bool success = await friendService.RemoveFriendAsync(currentUsername, friend.Username);
            //    if (success)
            //    {
            //        // Remove from the local collection
            //        Friends.Remove(friend);
            //    }
            //}
            //catch (Exception ex)
            //{
            //    // Handle exception (log or display to user)
            //    System.Diagnostics.Debug.WriteLine($"Error removing friend: {ex.Message}");
            //}
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}