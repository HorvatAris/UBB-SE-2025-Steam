using SteamHub.ApiContract.Models;
using System.Collections.ObjectModel;

namespace SteamHub.ViewModels
{
	public class PostViewModel
	{
		public PostViewModel(Post post)
		{
			Post = post;
		}

		public Post Post { get; set; }
		public ObservableCollection<Comment> Comments { get; set; } = new ObservableCollection<Comment>();

		// Optionally add shortcut properties for convenience:
		public int Id => Post.Id; // or the appropriate type of Id
	}

}
