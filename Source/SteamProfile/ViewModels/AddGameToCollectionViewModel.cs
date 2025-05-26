using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml.Controls;
using BusinessLayer.Models;
using BusinessLayer.Services.Interfaces;

namespace SteamProfile.ViewModels
{
    public class AddGameToCollectionViewModel : ObservableObject
    {
        // Constants to replace magic string literals
        private const string FailedToLoadAvailableGamesErrorMessage = "Failed to load available games. Please try again.";
        private const string FailedToAddGameErrorMessage = "Failed to add game to collection. Please try again.";

        private readonly ICollectionsService collectionsService;
        private readonly IUserService userService;
        private int userId;
        private ObservableCollection<OwnedGame> availableGames;
        private bool isLoading;
        private string errorMessage;
        private int collectionId;

        public ObservableCollection<OwnedGame> AvailableGames
        {
            get => availableGames;
            set => SetProperty(ref availableGames, value);
        }

        public bool IsLoading
        {
            get => isLoading;
            set => SetProperty(ref isLoading, value);
        }

        public string ErrorMessage
        {
            get => errorMessage;
            set => SetProperty(ref errorMessage, value);
        }

        public AddGameToCollectionViewModel(ICollectionsService collectionsService, IUserService userService)
        {
            this.collectionsService = collectionsService ?? throw new ArgumentNullException(nameof(collectionsService));
            this.userService = userService ?? throw new ArgumentNullException(nameof(userService));
            availableGames = new ObservableCollection<OwnedGame>();
        }

        public void Initialize(int collectionId)
        {
            this.collectionId = collectionId;
            LoadAvailableGames();
        }

        private void LoadAvailableGames()
        {
            try
            {
                IsLoading = true;
                ErrorMessage = null;

                var currentUser = userService.GetCurrentUser();
                var gamesNotInCollection = collectionsService.GetGamesNotInCollection(collectionId, currentUser.UserId);

                AvailableGames.Clear();
                foreach (var availableGame in gamesNotInCollection)
                {
                    AvailableGames.Add(availableGame);
                }
            }
            catch (Exception exception)
            {
                ErrorMessage = FailedToLoadAvailableGamesErrorMessage;
            }
            finally
            {
                IsLoading = false;
            }
        }

        public void AddGameToCollection(OwnedGame ownedGame)
        {
            try
            {
                var currentUser = userService.GetCurrentUser();
                collectionsService.AddGameToCollection(collectionId, ownedGame.GameId, currentUser.UserId);
                AvailableGames.Remove(ownedGame);
            }
            catch (Exception exception)
            {
                ErrorMessage = FailedToAddGameErrorMessage;
            }
        }

        protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value))
            {
                return false;
            }
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
