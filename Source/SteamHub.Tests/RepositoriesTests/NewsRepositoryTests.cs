using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SteamHub.Api.Context;
using SteamHub.Api.Context.Repositories;
using SteamHub.Api.Entities;
using Xunit;

namespace SteamHub.Tests.RepositoriesTests
{
	public class NewsRepositoryTests
	{
		private readonly DataContext _context;
		private readonly NewsRepository _repository;

		public NewsRepositoryTests()
		{
			var options = new DbContextOptionsBuilder<DataContext>()
				.UseInMemoryDatabase(Guid.NewGuid().ToString())
				.Options;

			var configuration = new ConfigurationBuilder().AddInMemoryCollection().Build();
			_context = new DataContext(options, configuration);
			_repository = new NewsRepository(_context);
			SeedData();
		}

		private void SeedData()
		{
			var post = new Post
			{
				Id = 1,
				AuthorId = 1,
				Content = "Initial post",
				UploadDate = DateTime.UtcNow,
				NrLikes = 1,
				NrDislikes = 0,
				NrComments = 1
			};
			_context.NewsPosts.Add(post);

			var rating = new PostRatingType
			{
				PostId = 1,
				AuthorId = 2,
				RatingType = true
			};
			_context.NewsPostRatingTypes.Add(rating);

			var comment = new Comment
			{
				CommentId = 1,
				PostId = 1,
				AuthorId = 2,
				Content = "Initial comment",
				CommentDate = DateTime.UtcNow,
				NrLikes = 0,
				NrDislikes = 0
			};
			_context.NewsComments.Add(comment);

			_context.SaveChanges();
		}

		[Fact]
		public async Task UpdatePostLikeCount_WhenPostExists_IncrementsLikeCount()
		{
			var result = await _repository.UpdatePostLikeCount(1);

			var post = await _context.NewsPosts.FindAsync(1);
			Assert.Equal(2, post.NrLikes);
		}

		[Fact]
		public async Task UpdatePostDislikeCount_WhenPostExists_IncrementsDislikeCount()
		{
			await _repository.UpdatePostDislikeCount(1);

			var post = await _context.NewsPosts.FindAsync(1);
			Assert.Equal(1, post.NrDislikes);
		}

		[Fact]
		public async Task AddRatingToPost_WhenValid_AddsRating()
		{
			await _repository.AddRatingToPost(1, 3, 1);

			var rating = await _context.NewsPostRatingTypes.FindAsync(1, 3);
			Assert.True(rating.RatingType);
		}

		[Fact]
		public async Task RemoveRatingFromPost_WhenRatingExists_RemovesRatingAndUpdatesPost()
		{
			await _repository.RemoveRatingFromPost(1, 2);
			
			var rating = await _context.NewsPostRatingTypes.FindAsync(1, 2);
			Assert.Null(rating);
		}

		[Fact]
		public async Task AddCommentToPost_WhenValid_AddsCommentAndIncrementsCount()
		{
			await _repository.AddCommentToPost(1, "New comment", 1, DateTime.UtcNow);

			var postAfter = await _context.NewsPosts.FindAsync(1);
			Assert.Equal(2, postAfter.NrComments);
		}

		[Fact]
		public async Task UpdateComment_WhenValid_UpdatesContent()
		{
			await _repository.UpdateComment(1, "Updated content");

			var comment = await _context.NewsComments.FindAsync(1);
			Assert.Equal("Updated content", comment.Content);
		}

		[Fact]
		public async Task DeleteCommentFromDatabase_WhenExists_DeletesComment()
		{
			await _repository.DeleteCommentFromDatabase(1);

			var comment = await _context.NewsComments.FindAsync(1);
			Assert.Null(comment);
		}

		[Fact]
		public async Task DeleteCommentFromDatabase_WhenExists_UpdatesCount()
		{
			await _repository.DeleteCommentFromDatabase(1);

			var post = await _context.NewsPosts.FindAsync(1);
			Assert.Equal(0, post.NrComments);
		}

		[Fact]
		public async Task LoadFollowingComments_WhenCalled_ReturnsCommentsForPost()
		{
			var result = await _repository.LoadFollowingComments(1);
			Assert.Single(result);
		}

		[Fact]
		public async Task AddPostToDatabase_WhenValid_AddsPost()
		{
			await _repository.AddPostToDatabase(3, "New Post", DateTime.UtcNow);

			var post = await _context.NewsPosts.FirstOrDefaultAsync(p => p.AuthorId == 3);
			Assert.NotNull(post);
		}

		[Fact]
		public async Task UpdatePost_WhenExists_UpdatesContent()
		{
			await _repository.UpdatePost(1, "Updated post content");

			var post = await _context.NewsPosts.FindAsync(1);
			Assert.Equal("Updated post content", post.Content);
		}

		[Fact]
		public async Task DeletePostFromDatabase_WhenExists_DeletesPost()
		{
			await _repository.DeletePostFromDatabase(1);

			var post = await _context.NewsPosts.FindAsync(1);
			Assert.Null(post);
		}

		[Fact]
		public async Task LoadFollowingPosts_WhenSearchedTextMatches_ReturnsFilteredPosts()
		{
			var result = await _repository.LoadFollowingPosts(1, 2, "Initial");
			Assert.Equal("Initial post", result.First().Content);
		}
	}
}
