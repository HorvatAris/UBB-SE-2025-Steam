using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SteamHub.ApiContract.Models.Collections;
using SteamHub.ApiContract.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteamHub.ViewModels
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
            _ = InitializeAsync();
        }

        private async Task InitializeAsync()
        {
            try
            {
                var currentUser = await userService.GetCurrentUserAsync();
                if (currentUser != null)
                {
                    userIdentifier = currentUser.UserId;
                    await LoadCollectionsAsync();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in InitializeAsync: {ex.Message}");
                errorMessage = ErrorLoadCollections;
            }
        }

        [RelayCommand]
        public async Task LoadCollectionsAsync()
        {
            try
            {
                isLoading = true;
                errorMessage = string.Empty;

                var collections = await collectionsService.GetAllCollections(userIdentifier);



                if (collections == null || collections.Count == 0)
                {
                    errorMessage = ErrorNoCollectionsFound;
                    Collections.Clear();
                    return;
                }

                Collections.Clear();
                foreach (var collection in collections)
                {
                    Debug.WriteLine($"[Load] Collection: {collection.CollectionId} - {collection.CollectionName}");
                    Collections.Add(collection);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in LoadCollectionsAsync: {ex.Message}");
                errorMessage = ErrorLoadCollections;
            }
            finally
            {
                isLoading = false;
            }
        }

        [RelayCommand]
        private async Task DeleteCollectionAsync(int collectionId)
        {
            try
            {
                await collectionsService.DeleteCollection(collectionId, userIdentifier);

                await LoadCollectionsAsync(); // Reload collections after deletion
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in DeleteCollectionAsync: {ex.Message}");
                errorMessage = ErrorDeleteCollection;
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

                selectedCollection = collection;
                // TODO: Navigate to collection details page
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in ViewCollection: {ex.Message}");
                errorMessage = ErrorViewCollection;
            }
        }

        [RelayCommand]
        private async Task AddGameToCollectionAsync(int gameId)
        {
            try
            {
                if (selectedCollection == null)
                {
                    return;
                }

                await collectionsService.AddGameToCollection(selectedCollection.CollectionId, gameId);

                await LoadCollectionsAsync(); // Reload collections to update the UI
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in AddGameToCollectionAsync: {ex.Message}");
                errorMessage = ErrorAddGameToCollection;
            }
        }

        [RelayCommand]
        private async Task RemoveGameFromCollectionAsync(int gameId)
        {
            try
            {
                if (selectedCollection == null)
                {
                    return;
                }

                await collectionsService.RemoveGameFromCollection(selectedCollection.CollectionId, gameId);

                await LoadCollectionsAsync(); // Reload collections to update the UI
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in RemoveGameFromCollectionAsync: {ex.Message}");
                errorMessage = ErrorRemoveGameFromCollection;
            }
        }

        [RelayCommand]
        private async Task CreateCollectionAsync(CreateCollectionParams parameters)
        {
            try
            {
                await collectionsService.CreateCollection(
                    userIdentifier,
                    parameters.CollectionName,
                    parameters.CoverPicture,
                    parameters.IsPublic,
                    parameters.CreatedAt);
                await LoadCollectionsAsync(); // Reload collections after creation
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in CreateCollectionAsync: {ex.Message}");
                errorMessage = ErrorCreateCollection;
            }
        }

        [RelayCommand]
        private async Task UpdateCollectionAsync(UpdateCollectionParams parameters)
        {
            try
            {
                await collectionsService.UpdateCollection(
                    parameters.CollectionId,
                    userIdentifier,
                    parameters.CollectionName,
                    parameters.CoverPicture,
                    parameters.IsPublic);
                await LoadCollectionsAsync(); // Reload collections after update
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in UpdateCollectionAsync: {ex.Message}");
                errorMessage = ErrorUpdateCollection;
            }
        }

        public async Task<List<Collection>> GetPublicCollectionsForUserAsync(int userId)
        {
            return await collectionsService.GetPublicCollectionsForUser(userId);
        }

        public async Task<Collection> GetCollectionByIdAsync(int collectionId, int userId)
        {
            return await collectionsService.GetCollectionByIdentifier(collectionId, userId);
        }

        public async Task RemoveGameFromCollectionAsync(int collectionId, int gameId)
        {
            await collectionsService.RemoveGameFromCollection(collectionId, gameId);
        }
    }
}
