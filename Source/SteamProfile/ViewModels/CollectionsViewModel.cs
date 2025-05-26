using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using BusinessLayer.Models;
using BusinessLayer.Services.Interfaces;

namespace SteamProfile.ViewModels
{
    public class CreateCollectionParams
    {
        public string CollectionName { get; set; }
        public string CoverPicture { get; set; }
        public bool IsPublic { get; set; }
        public DateOnly CreatedAt { get; set; }
    }

    public class UpdateCollectionParams
    {
        public int CollectionId { get; set; }
        public string CollectionName { get; set; }
        public string CoverPicture { get; set; }
        public bool IsPublic { get; set; }
    }

    public partial class CollectionsViewModel : ObservableObject
    {
        #region Constants
        // Error message constants
        private const string ErrorNoCollectionsFound = "No collections found.";
        private const string ErrorLoadCollections = "Error loading collections. Please try again.";
        private const string ErrorDeleteCollection = "Error deleting collection. Please try again.";
        private const string ErrorViewCollection = "Error viewing collection. Please try again.";
        private const string ErrorAddGameToCollection = "Error adding game to collection. Please try again.";
        private const string ErrorRemoveGameFromCollection = "Error removing game from collection. Please try again.";
        private const string ErrorCreateCollection = "Error creating collection. Please try again.";
        private const string ErrorUpdateCollection = "Error updating collection. Please try again.";
        #endregion

        private readonly ICollectionsService collectionsService;
        private readonly IUserService userService;
        private int userIdentifier;
        private ObservableCollection<Collection> collections;

        [ObservableProperty]
        private Collection selectedCollection;

        [ObservableProperty]
        private string errorMessage = string.Empty;

        [ObservableProperty]
        private bool isLoading;

        public ObservableCollection<Collection> Collections
        {
            get => collections;
            set
            {
                if (collections != value)
                {
                    collections = value;
                    OnPropertyChanged();
                }
            }
        }

        public CollectionsViewModel(ICollectionsService collectionsService, IUserService userService)
        {
            this.collectionsService = collectionsService ?? throw new ArgumentNullException(nameof(collectionsService));
            this.userService = userService ?? throw new ArgumentNullException(nameof(userService));
            collections = new ObservableCollection<Collection>();
        }

        [RelayCommand]
        public void LoadCollections()
        {
            try
            {
                userIdentifier = userService.GetCurrentUser().UserId;
                IsLoading = true;
                ErrorMessage = string.Empty;

                var collections = collectionsService.GetAllCollections(userIdentifier);

                if (collections == null || collections.Count == 0)
                {
                    ErrorMessage = ErrorNoCollectionsFound;
                    Collections.Clear();
                    return;
                }

                Collections.Clear();
                foreach (var collection in collections)
                {
                    Collections.Add(collection);
                }
            }
            catch (Exception)
            {
                ErrorMessage = ErrorLoadCollections;
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        private void DeleteCollection(int collectionId)
        {
            try
            {
                collectionsService.DeleteCollection(collectionId, userIdentifier);
                LoadCollections(); // Reload collections after deletion
            }
            catch (Exception)
            {
                ErrorMessage = ErrorDeleteCollection;
            }
        }

        [RelayCommand]
        private void ViewCollection(Collection collection)
        {
            try
            {
                if (collection == null)
                {
                    return;
                }

                SelectedCollection = collection;
                // TODO: Navigate to collection details page
            }
            catch (Exception)
            {
                ErrorMessage = ErrorViewCollection;
            }
        }

        [RelayCommand]
        private void AddGameToCollection(int gameId)
        {
            try
            {
                if (SelectedCollection == null)
                {
                    return;
                }

                collectionsService.AddGameToCollection(SelectedCollection.CollectionId, gameId, userIdentifier);
                LoadCollections(); // Reload collections to update the UI
            }
            catch (Exception)
            {
                ErrorMessage = ErrorAddGameToCollection;
            }
        }

        [RelayCommand]
        private void RemoveGameFromCollection(int gameId)
        {
            try
            {
                if (SelectedCollection == null)
                {
                    return;
                }

                collectionsService.RemoveGameFromCollection(SelectedCollection.CollectionId, gameId);
                LoadCollections(); // Reload collections to update the UI
            }
            catch (Exception)
            {
                ErrorMessage = ErrorRemoveGameFromCollection;
            }
        }

        [RelayCommand]
        private void CreateCollection(CreateCollectionParams parameters)
        {
            try
            {
                collectionsService.CreateCollection(
                    userIdentifier,
                    parameters.CollectionName,
                    parameters.CoverPicture,
                    parameters.IsPublic,
                    parameters.CreatedAt);
                LoadCollections(); // Reload collections after creation
            }
            catch (Exception)
            {
                ErrorMessage = ErrorCreateCollection;
            }
        }

        [RelayCommand]
        private void UpdateCollection(UpdateCollectionParams parameters)
        {
            try
            {
                collectionsService.UpdateCollection(
                    parameters.CollectionId,
                    userIdentifier,
                    parameters.CollectionName,
                    parameters.CoverPicture,
                    parameters.IsPublic);
                LoadCollections(); // Reload collections after update
            }
            catch (Exception)
            {
                ErrorMessage = ErrorUpdateCollection;
            }
        }

        public List<Collection> GetPublicCollectionsForUser(int userId)
        {
            return collectionsService.GetPublicCollectionsForUser(userId);
        }

        public Collection GetCollectionById(int collectionId, int userId)
        {
            return collectionsService.GetCollectionByIdentifier(collectionId, userId);
        }

        public void RemoveGameFromCollection(int collectionId, int gameId)
        {
            collectionsService.RemoveGameFromCollection(collectionId, gameId);
        }
    }
}
