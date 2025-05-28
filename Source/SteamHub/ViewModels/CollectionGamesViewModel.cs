using CommunityToolkit.Mvvm.ComponentModel;
using SteamHub.ApiContract.Models.Game;
using SteamHub.ApiContract.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteamHub.ViewModels
{
    public partial class CollectionGamesViewModel : ObservableObject
    {
        // Constants to replace magic numbers and strings
        private const int AllOwnedGamesCollectionId = 1;
        private const string FailedToLoadGamesErrorMessage = "Failed to load games";

        private readonly ICollectionsService collectionsService;

        [ObservableProperty]
        private string collectionName;

        [ObservableProperty]
        private ObservableCollection<OwnedGame> ownedGames;

        [ObservableProperty]
        private bool isLoading;

        [ObservableProperty]
        private string errorMessage;

        [ObservableProperty]
        private bool isAllOwnedGamesCollection;

        public CollectionGamesViewModel(ICollectionsService collectionsService)
        {
            this.collectionsService = collectionsService;
            ownedGames = new ObservableCollection<OwnedGame>();
        }

        public async Task LoadGamesAsync(int collectionId)
        {
            try
            {
                isLoading = true;
                errorMessage = string.Empty;
                isAllOwnedGamesCollection = collectionId == AllOwnedGamesCollectionId;

                // Await the task to get the list of games
                var gamesInCollection = await collectionsService.GetGamesInCollection(collectionId);

                ownedGames.Clear();
                foreach (var game in gamesInCollection)
                {
                    ownedGames.Add(game);
                }
            }
            catch (Exception exception)
            {
                errorMessage = FailedToLoadGamesErrorMessage;
            }
            finally
            {
                isLoading = false;
            }
        }
    }
}

