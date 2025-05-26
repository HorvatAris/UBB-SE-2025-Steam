using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using BusinessLayer.Models;
using BusinessLayer.Services;
using BusinessLayer.Services.Interfaces;

namespace SteamProfile.Views
{
    public sealed partial class CreatePostDialog : ContentDialog
    {
        // Hard-coded current user ID for demo
        private readonly int currentUserId = App.GetService<IForumService>().GetCurrentUserId();
        private User currentUser;
        // Result indicating if a post was created
        public bool PostCreated { get; private set; }
        public CreatePostDialog()
        {
            this.InitializeComponent();
            // Get current user and set up user display
            currentUser = User.GetUserById((int)currentUserId);
            UserNameTextBlock.Text = currentUser.Username;
            UserProfileImage.Source = new Microsoft.UI.Xaml.Media.Imaging.BitmapImage(new Uri(currentUser.ProfilePicturePath));
            // Register for button click events
            this.PrimaryButtonClick += CreatePostDialog_PrimaryButtonClick;
        }
        private void CreatePostDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            // Defer the close operation so we can validate inputs
            var deferral = args.GetDeferral();
            // Validate inputs
            bool isValid = ValidateInputs();
            // Prevent dialog from closing
            if (!isValid)
            {
                args.Cancel = true;
                deferral.Complete();
                return;
            }
            try
            {
                // Get values from controls
                string title = TitleTextBox.Text.Trim();
                string body = BodyTextBox.Text.Trim();
                // Get game ID (if selected)
                int? gameId = null;
                if (GameComboBox.SelectedIndex > 0)
                {
                    if (GameComboBox.SelectedItem is ComboBoxItem selectedItem)
                    {
                        var tagString = selectedItem.Tag.ToString();
                        if (int.TryParse(tagString, out int parsedGameId))
                        {
                            gameId = parsedGameId;
                        }
                    }
                }
                // Create the post
                string currentDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                App.GetService<IForumService>().CreatePost(title, body, currentDate, gameId);
                // Indicate that a post was created
                PostCreated = true;
            }
            catch (Exception ex)
            {
                // Show error inside the dialog instead of opening a new ContentDialog
                var message = $"There was an error creating your post: {ex.Message}";
                if (ex.InnerException != null)
                {
                    message += $"\nDetails: {ex.InnerException.Message}";
                }
                GeneralErrorText.Text = message;
                GeneralErrorText.Visibility = Visibility.Visible;
                // Prevent dialog from closing
                args.Cancel = true;
            }
            // Complete the deferral
            deferral.Complete();
        }
        private bool ValidateInputs()
        {
            bool isValid = true;
            // Check title
            if (string.IsNullOrWhiteSpace(TitleTextBox.Text))
            {
                TitleErrorText.Visibility = Visibility.Visible;
                isValid = false;
            }
            else
            {
                TitleErrorText.Visibility = Visibility.Collapsed;
            }
            // Check body
            if (string.IsNullOrWhiteSpace(BodyTextBox.Text))
            {
                BodyErrorText.Visibility = Visibility.Visible;
                isValid = false;
            }
            else
            {
                BodyErrorText.Visibility = Visibility.Collapsed;
            }
            return isValid;
        }
    }
}