namespace SteamHub.Tests.Services
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Moq;
    using SteamHub.Api.Context.Repositories;
    using SteamHub.ApiContract.Models;
    using SteamHub.ApiContract.Repositories;
    using SteamHub.ApiContract.Services;
    using Xunit;

    public class NewsServiceTests
    {
        private const int TestPostId = 1;
        private const int TestUserId = 1;
        private const int TestCommentId = 1;
        private const string TestPostContent = "Test post content";
        private const string TestCommentContent = "Test comment content";
        private const string TestHtmlContent = "<h2>Test</h2><b>Content</b>";
        private const string TestSpoilerContent = "<spoiler>Hidden content</spoiler>";

        private readonly NewsService newsService;
        private readonly Mock<INewsRepository> newsRepositoryMock;

        public NewsServiceTests()
        {
            newsRepositoryMock = new Mock<INewsRepository>();
            newsService = new NewsService(newsRepositoryMock.Object);
        }

        [Fact]
        public async Task FormatAsPostAsync_WhenValidContent_ShouldReturnFormattedHtml()
        {
            // Arrange
            var content = TestHtmlContent;

            // Act
            var result = await newsService.FormatAsPostAsync(content);

            // Assert
            Assert.Contains("<html>", result);
            Assert.Contains("<head>", result);
            Assert.Contains("<body>", result);
            Assert.Contains(TestHtmlContent, result);
        }

        [Fact]
        public async Task FormatAsPostAsync_WhenSpoilerContent_ShouldConvertToSpan()
        {
            // Arrange
            var content = TestSpoilerContent;

            // Act
            var result = await newsService.FormatAsPostAsync(content);

            // Assert
            Assert.Contains("class=\"spoiler\"", result);
            Assert.Contains("onclick=\"this.classList.toggle('revealed')\"", result);
        }

        [Fact]
        public async Task LikePostAsync_WhenValid_ShouldReturnTrue()
        {
            // Arrange
            newsRepositoryMock.Setup(x => x.UpdatePostLikeCount(TestPostId))
                .ReturnsAsync(1);
            newsRepositoryMock.Setup(x => x.AddRatingToPost(TestPostId, TestUserId, 1))
                .ReturnsAsync(1);

            // Act
            var result = await newsService.LikePostAsync(TestPostId, TestUserId);

            // Assert
            Assert.True(result);
            newsRepositoryMock.Verify(x => x.UpdatePostLikeCount(TestPostId), Times.Once);
            newsRepositoryMock.Verify(x => x.AddRatingToPost(TestPostId, TestUserId, 1), Times.Once);
        }

        [Fact]
        public async Task DislikePostAsync_WhenValid_ShouldReturnTrue()
        {
            // Arrange
            newsRepositoryMock.Setup(x => x.UpdatePostDislikeCount(TestPostId))
                .ReturnsAsync(1);
            newsRepositoryMock.Setup(x => x.AddRatingToPost(TestPostId, TestUserId, 0))
                .ReturnsAsync(1);

            // Act
            var result = await newsService.DislikePostAsync(TestPostId, TestUserId);

            // Assert
            Assert.True(result);
            newsRepositoryMock.Verify(x => x.UpdatePostDislikeCount(TestPostId), Times.Once);
            newsRepositoryMock.Verify(x => x.AddRatingToPost(TestPostId, TestUserId, 0), Times.Once);
        }

        [Fact]
        public async Task RemoveRatingFromPostAsync_WhenValid_ShouldReturnTrue()
        {
            // Arrange
            newsRepositoryMock.Setup(x => x.RemoveRatingFromPost(TestPostId, TestUserId))
                .ReturnsAsync(1);

            // Act
            var result = await newsService.RemoveRatingFromPostAsync(TestPostId, TestUserId);

            // Assert
            Assert.True(result);
            newsRepositoryMock.Verify(x => x.RemoveRatingFromPost(TestPostId, TestUserId), Times.Once);
        }

        [Fact]
        public async Task SaveCommentAsync_WhenValid_ShouldReturnTrue()
        {
            // Arrange
            newsRepositoryMock.Setup(x => x.AddCommentToPost(TestPostId, It.IsAny<string>(), TestUserId, It.IsAny<DateTime>()))
                .ReturnsAsync(1);

            // Act
            var result = await newsService.SaveCommentAsync(TestPostId, TestCommentContent, TestUserId);

            // Assert
            Assert.True(result);
            newsRepositoryMock.Verify(x => x.AddCommentToPost(TestPostId, It.IsAny<string>(), TestUserId, It.IsAny<DateTime>()), Times.Once);
        }

        [Fact]
        public async Task UpdateCommentAsync_WhenValid_ShouldReturnTrue()
        {
            // Arrange
            newsRepositoryMock.Setup(x => x.UpdateComment(TestCommentId, It.IsAny<string>()))
                .ReturnsAsync(1);

            // Act
            var result = await newsService.UpdateCommentAsync(TestCommentId, TestCommentContent, TestUserId);

            // Assert
            Assert.True(result);
            newsRepositoryMock.Verify(x => x.UpdateComment(TestCommentId, It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task DeleteCommentAsync_WhenValid_ShouldReturnTrue()
        {
            // Arrange
            newsRepositoryMock.Setup(x => x.DeleteCommentFromDatabase(TestCommentId))
                .ReturnsAsync(1);

            // Act
            var result = await newsService.DeleteCommentAsync(TestCommentId, TestUserId);

            // Assert
            Assert.True(result);
            newsRepositoryMock.Verify(x => x.DeleteCommentFromDatabase(TestCommentId), Times.Once);
        }

        [Fact]
        public async Task LoadNextCommentsAsync_WhenValid_ShouldReturnComments()
        {
            // Arrange
            var expectedComments = new List<Comment>
            {
                new Comment { CommentId = 1, Content = "Comment 1" },
                new Comment { CommentId = 2, Content = "Comment 2" }
            };

            newsRepositoryMock.Setup(x => x.LoadFollowingComments(TestPostId))
                .ReturnsAsync(expectedComments);

            // Act
            var result = await newsService.LoadNextCommentsAsync(TestPostId, TestUserId);

            // Assert
            Assert.Equal(expectedComments, result);
            newsRepositoryMock.Verify(x => x.LoadFollowingComments(TestPostId), Times.Once);
        }

        [Fact]
        public async Task SavePostAsync_WhenValid_ShouldReturnTrue()
        {
            // Arrange
            newsRepositoryMock.Setup(x => x.AddPostToDatabase(TestUserId, It.IsAny<string>(), It.IsAny<DateTime>()))
                .ReturnsAsync(1);

            // Act
            var result = await newsService.SavePostAsync(TestPostContent, TestUserId);

            // Assert
            Assert.True(result);
            newsRepositoryMock.Verify(x => x.AddPostToDatabase(TestUserId, It.IsAny<string>(), It.IsAny<DateTime>()), Times.Once);
        }

        [Fact]
        public async Task UpdatePostAsync_WhenValid_ShouldReturnTrue()
        {
            // Arrange
            newsRepositoryMock.Setup(x => x.UpdatePost(TestPostId, It.IsAny<string>()))
                .ReturnsAsync(1);

            // Act
            var result = await newsService.UpdatePostAsync(TestPostId, TestPostContent, TestUserId);

            // Assert
            Assert.True(result);
            newsRepositoryMock.Verify(x => x.UpdatePost(TestPostId, It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task DeletePostAsync_WhenValid_ShouldReturnTrue()
        {
            // Arrange
            newsRepositoryMock.Setup(x => x.DeletePostFromDatabase(TestPostId))
                .ReturnsAsync(1);

            // Act
            var result = await newsService.DeletePostAsync(TestPostId, TestUserId);

            // Assert
            Assert.True(result);
            newsRepositoryMock.Verify(x => x.DeletePostFromDatabase(TestPostId), Times.Once);
        }

        [Fact]
        public async Task LoadNextPostsAsync_WhenValid_ShouldReturnPosts()
        {
            // Arrange
            var pageNumber = 1;
            var searchedText = "test";
            var expectedPosts = new List<Post>
            {
                new Post { Id = 1, Content = "Post 1" },
                new Post { Id = 2, Content = "Post 2" }
            };

            newsRepositoryMock.Setup(x => x.LoadFollowingPosts(pageNumber, TestUserId, searchedText))
                .ReturnsAsync(expectedPosts);

            // Act
            var result = await newsService.LoadNextPostsAsync(pageNumber, searchedText, TestUserId);

            // Assert
            Assert.Equal(expectedPosts, result);
            newsRepositoryMock.Verify(x => x.LoadFollowingPosts(pageNumber, TestUserId, searchedText), Times.Once);
        }

        [Fact]
        public void SetStringOnEditMode_WhenEditModeTrue_ShouldReturnSave()
        {
            // Act
            var result = newsService.SetStringOnEditMode(true);

            // Assert
            Assert.Equal("Save", result);
        }

        [Fact]
        public void SetStringOnEditMode_WhenEditModeFalse_ShouldReturnPostComment()
        {
            // Act
            var result = newsService.SetStringOnEditMode(false);

            // Assert
            Assert.Equal("Post Comment", result);
        }
    }
}