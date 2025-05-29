using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using SteamHub.ApiContract.Models;
using SteamHub.ApiContract.Models.User;
using SteamHub.ApiContract.Services.Interfaces;

namespace SteamHub.ViewModels
{
	public class NewsPageViewModel : BaseViewModel
	{
		private readonly INewsService newsService;
		private string searchText;
		private ObservableCollection<PostViewModel> posts;

		public NewsPageViewModel(INewsService newsService, IUserService userService, User currentUser)
			: base(userService, currentUser)
		{
			this.newsService = newsService ?? throw new ArgumentNullException(nameof(newsService));
			Posts = new ObservableCollection<PostViewModel>();
		}

		public string SearchText
		{
			get => searchText;
			set
			{
				if (SetProperty(ref searchText, value))
				{
					_ = LoadPosts();
				}
			}
		}

		public ObservableCollection<PostViewModel> Posts
		{
			get => posts;
			set => SetProperty(ref posts, value);
		}
		public ObservableCollection<Comment> AllComments { get; set; } = new ObservableCollection<Comment>();

		public ObservableCollection<Comment> GetCommentsForPost(int postId)
		{
			// Filter comments where Comment.PostId equals the provided postId
			var filteredComments = AllComments.Where(c => c.PostId == postId);
			return new ObservableCollection<Comment>(filteredComments);
		}

		public async Task LoadPosts()
		{
			try
			{
				var user = await this.userService.GetCurrentUserAsync();
				var loadedPosts = await newsService.LoadNextPostsAsync(0, "", user.UserId);
				Posts.Clear();

				foreach (var post in loadedPosts)
				{
					var postVM = new PostViewModel(post);
					var comments = await newsService.LoadNextCommentsAsync(post.Id, user.UserId);
					foreach (var comment in comments)
						postVM.Comments.Add(comment);
					Posts.Add(postVM);
				}
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine($"Error loading posts: {ex.Message}");
			}
		}

		public async Task CreatePost(string title, string content)
		{
			try
			{
				var user = await this.userService.GetCurrentUserAsync();
				var formattedContent = await newsService.FormatAsPostAsync(content);
				var result = await newsService.SavePostAsync(formattedContent, user.UserId);
				if (result)
				{
					await LoadPosts();
				}
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine($"Error creating post: {ex.Message}");
			}
		}

		public async Task LikePost(int postId)
		{
			try
			{
				var user = await this.userService.GetCurrentUserAsync();
				var result = await newsService.LikePostAsync(postId, user.UserId);
				if (result)
				{
					await LoadPosts();
				}
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine($"Error liking post: {ex.Message}");
			}
		}

		public async Task DislikePost(int postId)
		{
			try
			{
				var user = await this.userService.GetCurrentUserAsync();
				var result = await newsService.DislikePostAsync(postId, user.UserId);
				if (result)
				{
					await LoadPosts();
				}
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine($"Error disliking post: {ex.Message}");
			}
		}

		public async Task AddComment(int postId, string content)
		{
			try
			{
				var user = await this.userService.GetCurrentUserAsync();
				var result = await newsService.SaveCommentAsync(postId, content, user.UserId);
				if (result)
				{
					var postVM = Posts.FirstOrDefault(p => p.Id == postId);
					if (postVM != null)
					{
						postVM.Comments.Clear();
						var comments = await newsService.LoadNextCommentsAsync(postId, user.UserId);
						foreach (var c in comments)
							postVM.Comments.Add(c);
					}
				}
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine($"Error adding comment: {ex.Message}");
			}
		}
	}
}
