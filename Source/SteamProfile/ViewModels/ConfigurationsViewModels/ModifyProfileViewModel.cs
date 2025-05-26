using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using SteamProfile.Views;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;

namespace SteamProfile.ViewModels.ConfigurationsViewModels
{
    public partial class ModifyProfileViewModel : ObservableObject
    {
        private int userIdentifier;
        private string originalImagePath = string.Empty;
        private string originalDescription = string.Empty;
        private StorageFile selectedImageFile;

        public ModifyProfileViewModel(Frame frame)
        {
            // Get current user ID (you might need to adjust how you get this)
            userIdentifier = App.UserService.GetCurrentUser().UserId; // Assuming this exists

            // Load existing profile data
            LoadUserProfile();

            // Set initial save state
            UpdateCanSave();
        }

        private void LoadUserProfile()
        {
            var userProfile = App.UserProfileRepository.GetUserProfileByUserId(userIdentifier);
            if (userProfile != null)
            {
                originalImagePath = userProfile.ProfilePicture ?? string.Empty;
                originalDescription = userProfile.Bio ?? string.Empty;

                // Set current values
                SelectedImagePath = originalImagePath;
                SelectedImageName = !string.IsNullOrEmpty(originalImagePath) ?
                    System.IO.Path.GetFileName(originalImagePath) : "No image selected";
                Description = originalDescription;
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
            // Check if description has validation errors
            if (ValidateDescription())
            {
                bool changesMade = false;

                // Save new picture if changed
                if (selectedImageFile != null && SelectedImagePath != originalImagePath)
                {
                    App.UserProfileRepository.UpdateProfilePicture(userIdentifier, selectedImageFile.Path);
                    originalImagePath = SelectedImagePath;
                    changesMade = true;
                }

                // Save new description if changed
                if (Description != originalDescription)
                {
                    App.UserProfileRepository.UpdateProfileBio(userIdentifier, Description);
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