using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using BusinessLayer.Services;
using BusinessLayer.Services.Interfaces;
using BusinessLayer.Models;

namespace SteamProfile.Views
{
    public sealed partial class AddCommentDialog : ContentDialog
    {
        // Current user ID from service
        private readonly int currentUserId = App.GetService<IForumService>().GetCurrentUserId();
        private User currentUser;
        // The post ID this comment will be added to
        public int PostId { get; private set; }
        // Result indicating if a comment was created
        public bool CommentCreated { get; private set; }
        public AddCommentDialog(int postId)
        {
            this.InitializeComponent();
            PostId = postId;
            // Get current user and set up user display
            currentUser = User.GetUserById((int)currentUserId);
            UserNameTextBlock.Text = currentUser.Username;
            UserProfileImage.Source = new Microsoft.UI.Xaml.Media.Imaging.BitmapImage(new Uri(currentUser.ProfilePicturePath));
            // Register for button click events
            this.PrimaryButtonClick += AddCommentDialog_PrimaryButtonClick;
        }
        private void AddCommentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            // Defer the close operation so we can validate inputs
            var deferral = args.GetDeferral();
            try
            {
                // Validate inputs
                bool isValid = ValidateInputs();
                if (!isValid)
                {
                    // Prevent dialog from closing
                    args.Cancel = true;
                    deferral.Complete();
                    return;
                }
                // Get comment text
                string commentBody = CommentTextBox.Text.Trim();
                // Create the comment
                string currentDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                App.GetService<IForumService>().CreateComment(commentBody, PostId, currentDate);
                // Log success
                System.Diagnostics.Debug.WriteLine($"Comment created successfully for post {PostId}");
                // Indicate that a comment was created
                CommentCreated = true;
            }
            catch (Exception ex)
            {
                // Log the error
                System.Diagnostics.Debug.WriteLine($"Error creating comment: {ex.Message}");
                // Prevent dialog from closing
                args.Cancel = true;
            }
            finally
            {
                // Complete the deferral
                deferral.Complete();
            }
        }
        private bool ValidateInputs()
        {
            bool isValid = true;
            // Check comment body
            if (string.IsNullOrWhiteSpace(CommentTextBox.Text))
            {
                CommentErrorText.Visibility = Visibility.Visible;
                isValid = false;
            }
            else
            {
                CommentErrorText.Visibility = Visibility.Collapsed;
            }
            return isValid;
        }
    }
}