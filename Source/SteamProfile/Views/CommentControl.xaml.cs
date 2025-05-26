using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Imaging;
using BusinessLayer.Models;
using BusinessLayer.Services.Interfaces;
using SteamProfile.ViewModels;

namespace SteamProfile.Views
{
    public sealed partial class CommentControl : UserControl
    {
        // The currently logged-in user ID
        private static readonly int CurrentUserId = App.GetService<IForumService>().GetCurrentUserId();
        // The comment being displayed
        private CommentDisplay comment;

        public Action<object, RoutedEventArgs> CommentDeleted { get; internal set; }
        public Action<object, RoutedEventArgs> CommentUpdated { get; internal set; }

        // Events for user interactions
        public event EventHandler<int> DeleteRequested;
        public event EventHandler<int> CommentVoted;
        public CommentControl()
        {
            this.InitializeComponent();
        }
        // Set up the control with comment data
        public void SetComment(CommentDisplay comment)
        {
            this.comment = comment;
            // Set up UI elements
            this.Loaded += async (s, e) =>
            {
                string html = comment.Body;
                if (!html.TrimStart().StartsWith("<html", StringComparison.OrdinalIgnoreCase))
                {
                    html = $"<html><body>{html}</body></html>";
                }
                await BodyWebView.EnsureCoreWebView2Async();
                BodyWebView.CoreWebView2.NavigateToString(html);
            };
            UsernameTextBlock.Text = comment.Username;
            TimeStampTextBlock.Text = comment.TimeStamp;
            ScoreTextBlock.Text = comment.Score.ToString();
            // Set profile image
            ProfileImage.Source = new BitmapImage(new Uri(comment.ProfilePicturePath));
            // Show delete button if this is the current user's comment
            DeleteButton.Visibility = (comment.AuthorId == CurrentUserId) ? Visibility.Visible : Visibility.Collapsed;
            // Set tag for delete button
            DeleteButton.Tag = comment.Id;
        }
        // Update the score display
        public void UpdateScore(int newScore)
        {
            ScoreTextBlock.Text = newScore.ToString();
        }
        // Handle delete button click
        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            // Trigger the DeleteRequested event
            DeleteRequested?.Invoke(this, (int)comment.Id);
        }
        // Handle upvote button click
        private void UpvoteButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Call the service to upvote the comment
                App.GetService<IForumService>().VoteOnComment(comment.Id, 1);
                // Update the score locally
                comment.Comment.Score += 1;
                ScoreTextBlock.Text = comment.Comment.Score.ToString();
                // Notify of vote
                CommentVoted?.Invoke(this, (int)comment.Id);
            }
            catch (Exception ex)
            {
                // Handle error
                System.Diagnostics.Debug.WriteLine($"Error upvoting comment: {ex.Message}");
            }
        }
        // Handle downvote button click
        private void DownvoteButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Call the service to downvote the comment
                App.GetService<IForumService>().VoteOnComment(comment.Id, -1);
                // Update the score locally
                comment.Comment.Score -= 1;
                ScoreTextBlock.Text = comment.Comment.Score.ToString();
                // Notify of vote
                CommentVoted?.Invoke(this, (int)comment.Id);
            }
            catch (Exception ex)
            {
                // Handle error
                System.Diagnostics.Debug.WriteLine($"Error downvoting comment: {ex.Message}");
            }
        }

        internal void SetCommentData(Comment commentModel)
        {
            // Map the Comment entity to CommentDisplay
            var user = App.UserService.GetUserByIdentifier(commentModel.AuthorId);
            var profile = App.UserProfileRepository.GetUserProfileByUserId(commentModel.AuthorId);

            var display = new ForumComment
            {
                Id = (int)commentModel.CommentId,
                AuthorId = (int)commentModel.AuthorId,
                Body = commentModel.Content,
                Score = commentModel.NrLikes,
                TimeStamp = commentModel.CommentDate
            };

            // Delegate to existing SetComment
            SetComment(CommentDisplay.FromComment(display));
        }
    }
}