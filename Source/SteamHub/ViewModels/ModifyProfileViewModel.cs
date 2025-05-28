using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using SteamHub.ApiContract.Services.Interfaces;
using SteamHub.Pages;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;

namespace SteamHub.ViewModels
{
    public partial class ModifyProfileViewModel : ObservableObject
    {
        private int userIdentifier;
        private string originalImagePath = string.Empty;
        private string originalDescription = string.Empty;
        private StorageFile selectedImageFile;
        private readonly IUserService userService;

        public ModifyProfileViewModel(IUserService userService, Frame frame)
        {
            this.userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _ = InitializeAsync();
        }

        private async Task InitializeAsync()
        {
            try
            {
                // Get current user ID
                var currentUser = await userService.GetCurrentUserAsync();
                if (currentUser != null)
                {
                    userIdentifier = currentUser.UserId;
                    await LoadUserProfileAsync();
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("Failed to get current user during initialization");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in InitializeAsync: {ex.Message}");
            }
        }

        private async Task LoadUserProfileAsync()
        {
            try
            {
                var user = await userService.GetUserByIdentifierAsync(userIdentifier);
                if (user != null)
                {
                    originalImagePath = user.ProfilePicture ?? string.Empty;
                    originalDescription = user.Bio ?? string.Empty;

                    // Set current values
                    SelectedImagePath = originalImagePath;
                    SelectedImageName = !string.IsNullOrEmpty(originalImagePath) ?
                        System.IO.Path.GetFileName(originalImagePath) : "No image selected";
                    Description = originalDescription;

                    // Set initial save state
                    UpdateCanSave();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in LoadUserProfileAsync: {ex.Message}");
            }
        }

        [RelayCommand]
        private async Task ChooseNewPhotoAsync()
        {
            var filePicker = new FileOpenPicker();

            // Initialize the picker with the app window
            var window = new Window();
            WinRT.Interop.InitializeWithWindow.Initialize(filePicker, WinRT.Interop.WindowNative.GetWindowHandle(window));

            filePicker.ViewMode = PickerViewMode.Thumbnail;
            filePicker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            filePicker.FileTypeFilter.Add(".jpg");
            filePicker.FileTypeFilter.Add(".jpeg");
            filePicker.FileTypeFilter.Add(".png");

            selectedImageFile = await filePicker.PickSingleFileAsync();

            if (selectedImageFile != null)
            {
                SelectedImageName = selectedImageFile.Name;
                SelectedImagePath = selectedImageFile.Path;
                UpdateCanSave();
            }
        }

        [RelayCommand]
        private async Task SaveChangesAsync()
        {
            try
            {
                // Check if description has validation errors
                if (ValidateDescription())
                {
                    bool changesMade = false;

                    // Save new picture if changed
                    if (selectedImageFile != null && SelectedImagePath != originalImagePath)
                    {
                        await userService.UpdateProfilePictureAsync(selectedImageFile.Path);
                        originalImagePath = SelectedImagePath;
                        changesMade = true;
                    }

                    // Save new description if changed
                    if (Description != originalDescription)
                    {
                        await App.UserService.UpdateProfileBioAsync(userIdentifier, Description);
                        originalDescription = Description;
                        changesMade = true;
                    }

                    if (changesMade)
                    {
                        SuccessMessage = "Your profile has been updated successfully!";
                        SuccessMessageVisibility = Visibility.Visible;

                        // Hide success message after a few seconds
                        await Task.Delay(3000);
                        SuccessMessageVisibility = Visibility.Collapsed;
                    }

                    UpdateCanSave();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in SaveChangesAsync: {ex.Message}");
                SuccessMessage = "Failed to update profile. Please try again.";
                SuccessMessageVisibility = Visibility.Visible;
                await Task.Delay(3000);
                SuccessMessageVisibility = Visibility.Collapsed;
            }
        }

        private bool ValidateDescription()
        {
            // Example validation - you can customize this
            if (Description.Length > 500)
            {
                DescriptionErrorMessage = "Description must be less than 500 characters.";
                DescriptionErrorVisibility = Visibility.Visible;
                return false;
            }

            DescriptionErrorVisibility = Visibility.Collapsed;
            DescriptionErrorMessage = string.Empty;
            return true;
        }

        private void UpdateCanSave()
        {
            // Enable save button if anything has changed
            CanSave = (Description != originalDescription || SelectedImagePath != originalImagePath)
                    && DescriptionErrorVisibility != Visibility.Visible;
        }

        partial void OnDescriptionChanged(string value)
        {
            ValidateDescription();
            UpdateCanSave();
        }

        [ObservableProperty]
        private string selectedImageName = string.Empty;

        [ObservableProperty]
        private string selectedImagePath = string.Empty;

        [ObservableProperty]
        private string description = string.Empty;

        [ObservableProperty]
        private string descriptionErrorMessage = string.Empty;

        [ObservableProperty]
        private Visibility descriptionErrorVisibility = Visibility.Collapsed;

        [ObservableProperty]
        private bool canSave;

        [ObservableProperty]
        private string successMessage = string.Empty;

        [ObservableProperty]
        private Visibility successMessageVisibility = Visibility.Collapsed;
    }
}