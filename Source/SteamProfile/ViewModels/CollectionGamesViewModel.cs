using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using BusinessLayer.Models;
using BusinessLayer.Services.Interfaces;

namespace SteamProfile.ViewModels
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

        public void LoadGames(int collectionId)
        {
            try
            {
                IsLoading = true;
                ErrorMessage = string.Empty;
                IsAllOwnedGamesCollection = collectionId == AllOwnedGamesCollectionId;
                var gamesInCollection = collectionsService.GetGamesInCollection(collectionId);
                ownedGames.Clear();
                foreach (var game in gamesInCollection)
                {
                    ownedGames.Add(game);
                }
            }
            catch (Exception exception)
            {
                ErrorMessage = FailedToLoadGamesErrorMessage;
            }
            finally
            {
                IsLoading = false;
            }
        }
    }
}
