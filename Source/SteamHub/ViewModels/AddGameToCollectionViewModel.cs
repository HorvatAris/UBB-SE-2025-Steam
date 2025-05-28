using CommunityToolkit.Mvvm.ComponentModel;
using SteamHub.ApiContract.Models.Game;
using SteamHub.ApiContract.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace SteamHub.ViewModels
{
    public partial class AddGameToCollectionViewModel : ObservableObject
    {
        private const string FailedToLoadAvailableGamesErrorMessage = "Failed to load available games. Please try again.";
        private const string FailedToAddGameErrorMessage = "Failed to add game to collection. Please try again.";

        private readonly ICollectionsService collectionsService;
        private readonly IUserService userService;

        private int userId;
        private int collectionId;

        [ObservableProperty]
        private ObservableCollection<OwnedGame> availableGames = new();

        [ObservableProperty]
        private bool isLoading;

        [ObservableProperty]
        private string errorMessage;

        public AddGameToCollectionViewModel(ICollectionsService collectionsService, IUserService userService)
        {
            this.collectionsService = collectionsService ?? throw new ArgumentNullException(nameof(collectionsService));
            this.userService = userService ?? throw new ArgumentNullException(nameof(userService));
        }

        public async Task InitializeAsync(int collectionId)
        {
            this.collectionId = collectionId;
            await LoadAvailableGamesAsync();
        }

        private async Task LoadAvailableGamesAsync()
        {
            try
            {
                IsLoading = true;
                ErrorMessage = null;

                var currentUser = await userService.GetCurrentUserAsync();
                userId = currentUser.UserId;

                var gamesNotInCollection = await collectionsService.GetGamesNotInCollection(collectionId, userId);
                AvailableGames.Clear();

                foreach (var game in gamesNotInCollection)
                {
                    AvailableGames.Add(game);
                }
            }
            catch (Exception)
            {
                ErrorMessage = FailedToLoadAvailableGamesErrorMessage;
            }
            finally
            {
                IsLoading = false;
            }
        }

        public async Task AddGameToCollectionAsync(OwnedGame ownedGame)
        {
            try
            {
                var currentUser = await userService.GetCurrentUserAsync();
                await collectionsService.AddGameToCollection(collectionId, ownedGame.GameId);
                AvailableGames.Remove(ownedGame);
            }
            catch (Exception)
            {
                ErrorMessage = FailedToAddGameErrorMessage;
            }
        }
    }
}
