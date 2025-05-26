using Microsoft.UI.Composition;
using News;
using System.Data;
using System.Data.SqlClient;
using Windows.ApplicationModel.Activation;
using Moq;
using System.ComponentModel.Design;
using System.Data.Common;
using System.ComponentModel;

//MUST HAVE THESE INCLUDES IN EACH TEST FILE
using Microsoft.UI.Xaml.Controls;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualStudio.TestTools.UnitTesting.AppContainer;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Tests.Community_Tests
{
    // Stubs & Mocks
    public class FakeUsers
    {
        private static List<User> users = new();

        public FakeUsers()
        {
            users.Add(new User(1000, "TestUser1", false));
            users.Add(new User(1001, "TestUser2", true));
        }

        public User? GetUserById(int id)
        {
            return users.Find(user => user.id == id);
        }
    }

    public class FakeNewsService : NewsService
    {
        public bool SaveCommentCalled = false;
        public bool UpdateCommentCalled = false;
        public bool SavePostCalled = false;
        public bool UpdatePostCalled = false;

        public bool SaveCommentResult = true;
        public bool UpdateCommentResult = true;

        public override bool SaveComment(int postId, string content)
        {
            SaveCommentCalled = true;
            return SaveCommentResult;
        }

        public override bool UpdateComment(int commentId, string content)
        {
            UpdateCommentCalled = true;
            return UpdateCommentResult;
        }

        public override bool SavePost(string content)
        {
            SavePostCalled = true;
            return SavePostCalled;
        }

        public override bool UpdatePost(int postId, string content)
        {
            UpdatePostCalled = true;
            return UpdatePostCalled;
        }

        public override string FormatAsPost(string input) => input;
    }

    // Tests
    [TestClass]
    public sealed class NewsTests
    {
        NewsDatabase newConnection = new NewsDatabase();

        [TestMethod]
        public void Connect_WithConnectionClosed_OpensConnection()
        {
            // Assert precondition
            Assert.AreEqual(ConnectionState.Closed, newConnection.Connection.State, "Connection should be closed.");

            try
            {
                // Act
                newConnection.Connect();

                // Assert
                Assert.AreEqual(ConnectionState.Open, newConnection.Connection.State, "Connection should be open!");
            }
            finally
            {
                // Cleanup
                newConnection.Connection.Close();
            }
        }

        [TestMethod]
        public void Connect_WithConnectionOpen_ThrowsException()
        {
            try
            {
                // Arrange
                newConnection.Connection.Open();

                // Act & Assert
                Assert.ThrowsException<InvalidOperationException>(() => newConnection.Connect(), 
                    "Method should throw an exception if it tries to open an already opened connection!");
            }
            finally
            {
                // Cleanup
                newConnection.Connection.Close();
            }
        }

        [TestMethod]
        public void Disconnect_WithConnectionOpen_ClosesConnection()
        {
            try
            {
                // Arrange
                newConnection.Connection.Open();

                // Assert precondition
                Assert.AreEqual(ConnectionState.Open, newConnection.Connection.State, "Connection should be open!");

                // Act
                newConnection.Disconnect();

                // Assert
                Assert.AreEqual(ConnectionState.Closed, newConnection.Connection.State, "Connection should be closed!");
            }
            finally
            {
                // Cleanup
                newConnection.Connection.Close();
            }
        }

        [TestMethod]
        public void Disconnect_WithConnectionClosed_DoesNothing()
        {
            // Assert precondition
            Assert.AreEqual(ConnectionState.Closed, newConnection.Connection.State, "Connection should be closed.");

            // Act
            newConnection.Disconnect();

            // Assert
            Assert.AreEqual(ConnectionState.Closed, newConnection.Connection.State, "Connection should be closed.");
        }

        [TestMethod]
        public void ExecuteQuery_WithIncorrectQueryString_ThrowsException()
        {
            // Arrange
            string wrongQueryString = "INSERT INTO Ratings VALUES(1)";
            newConnection.Connection.Open();

            try
            {
                Assert.ThrowsException<Exception>(() => newConnection.ExecuteQuery(wrongQueryString));
            }
            finally
            {
                // Cleanup
                newConnection.Connection.Close();
            }
        }

        [TestMethod]
        public void ExecuteQuery_WithCorrectQueryString_ReturnsNumberOfAffectedRows()
        {
            // Arrange
            string correctQueryString = "SELECT * FROM Ratings";
            newConnection.Connection.Open();

            try
            {
                // Act
                int executionResult = newConnection.ExecuteQuery(correctQueryString);

                // Arrange

                // -1 because no rows were affected by the select query
                Assert.AreEqual(-1, executionResult, "Execution result should be -1 because of the test query.");
            }
            finally
            {
                // Cleanup
                newConnection.Connection.Close();
            }
        }

        [TestMethod]
        public void ExecuteReader_WithIncorrectQueryString_ThrowsException()
        {
            // Arrange
            string incorrectQueryString = "SELECT * FROM dddwdwdwa";
            newConnection.Connection.Open();

            try
            {
                // Act & Assert
                Assert.ThrowsException<SqlException>(() => newConnection.ExecuteReader(incorrectQueryString));
            }
            finally
            {
                // Cleanup
                newConnection.Connection.Close();
            }
        }

        [TestMethod]
        public void ExecuteReader_WithCorrectQueryString_ReturnsNewReader()
        {
            // Arrange
            string correctQueryString = "SELECT * FROM Ratings";
            newConnection.Connection.Open();

            try
            {
                // Act
                SqlDataReader newReader = newConnection.ExecuteReader(correctQueryString);

                Assert.IsNotNull(newReader, "Return value should never be null!");
            }
            finally
            {
                newConnection.Connection.Close();
            }
        }

        [TestMethod]
        public void ExecuteSearchReader_WithIncorrectQueryString_ThrowsException()
        {
            // Arrange
            string incorrectQueryString = "SELECT * FROM 71284814";
            newConnection.Connection.Open();

            try
            {
                // Act & Assert
                Assert.ThrowsException<SqlException>(() => newConnection.ExecuteSearchReader(incorrectQueryString, ""));
            }
            finally
            {
                newConnection.Connection.Close();
            }
        }

        [TestMethod]
        public void ExecuteSearchReader_WithCorrectQueryString_ReturnsCorrectReader()
        {
            // Arrange
            string noSearch = "";
            string correctQueryString = "SELECT * FROM NewsPosts WHERE content LIKE @search";
            newConnection.Connection.Open();

            try
            {
                // Act
                SqlDataReader noSearchReader = newConnection.ExecuteSearchReader(correctQueryString, noSearch);

                // Assert
                Assert.IsNotNull(noSearchReader, "No return value should be null!");
            }
            finally
            {
                newConnection.Connection.Close();
            }
        }

        [TestMethod]
        public void ExecuteScalar_WithIncorrectQueryString_ThrowsException()
        {
            // Arrange
            string wrongQueryString = "SELECT ratingType FROM Ratings WHERE postId={post.Id} AND authorId={userId}";
            newConnection.Connection.Open();

            try
            {
                // Act & Assert
                Assert.ThrowsException<SqlException>(() => newConnection.ExecuteScalar(wrongQueryString));
            }
            finally
            {
                newConnection.Connection.Close();
            }
        }

        [TestMethod]
        public void ExecuteScalar_WithCorrectQueryString_ReturnsFirstCell()
        {
            // Arrange
            string correctQueryString = "SELECT ratingType FROM Ratings WHERE postId=1 AND authorId=1";
            newConnection.Connection.Open();

            try
            {
                // Act
                var cell = newConnection.ExecuteScalar(correctQueryString);

                // Assert
                Assert.IsNotNull(cell, "Cell should not be null, unless it was changed in the database.");
            }
            finally
            {
                newConnection.Connection.Close();
            }
        }

        [TestMethod]
        public void GetUserById_WithWrongId_ReturnsNull()
        {
            // Arrange
            int notExistingId = 999;
            FakeUsers fakeUsers = new FakeUsers();

            // Act
            User fakeUser = fakeUsers.GetUserById(notExistingId);

            // Assert
            Assert.IsNull(fakeUser, "User instance should be null since it doesn't exist.");
        }

        [TestMethod]
        public void GetUserById_WithExistingId_ReturnsCorrectUser()
        {
            // Arrange
            int existingId = 1000;
            FakeUsers fakeUsers = new FakeUsers();

            // Act
            User fakeUser = fakeUsers.GetUserById(existingId);

            // Assert
            Assert.IsNotNull(fakeUser, "Expected a valid user instance for a valid id.");
            Assert.AreEqual("TestUser1", fakeUser.username, "User's username must match with the actual value.");
            Assert.IsFalse(fakeUser.bIsDeveloper, "The user searched in the test should not be a developer.");
        }

        [TestMethod]
        public void UpdatePostLikeCount_WithCorrectQuery_CallsExecuteQueryAndReturnsResult()
        {
            // Arrange
            var mockDb = new Mock<INewsDatabase>();
            mockDb.Setup(db => db.Connect());
            mockDb.Setup(db => db.Disconnect());
            mockDb.Setup(db => db.ExecuteQuery(It.IsAny<string>())).Returns(1);

            NewsRepository mockRepository = new NewsRepository(mockDb.Object);

            int postId = 5;
            string correctQuery = $"UPDATE NewsPosts SET nrLikes = nrLikes + 1 WHERE id = {postId}";

            // Act
            int executionResult = mockRepository.UpdatePostLikeCount(postId);

            // Assert
            mockDb.Verify(db => db.Connect(), Times.Once, "Connection to database should happen once!");
            mockDb.Verify(db => db.Disconnect(), Times.Once, "Closing the database should happen once!");
            mockDb.Verify(db => db.ExecuteQuery(correctQuery), Times.Once, "Method should execute with expected query!");
            Assert.AreEqual(1, executionResult, "Execution result should be more than one with correct query");
        }

        [TestMethod]
        public void UpdatePostLikeCount_WhenExecuteQueryFails_ThrowsException()
        {
            // Arrange
            var mockDb = new Mock<INewsDatabase>();
            mockDb.Setup(db => db.Connect());
            mockDb.Setup(db => db.Disconnect());
            mockDb.Setup(db => db.ExecuteQuery(It.IsAny<string>())).Throws(new Exception("Error"));

            NewsRepository mockRepository = new NewsRepository(mockDb.Object);

            // Act & Assert
            Assert.ThrowsException<Exception>(() => mockRepository.UpdatePostLikeCount(5));
        }

        [TestMethod]
        public void UpdatePostDislikeCount_WithCorrectQuery_CallsExecuteQueryAndReturnsResult()
        {
            // Arrange
            var mockDb = new Mock<INewsDatabase>();
            mockDb.Setup(db => db.Connect());
            mockDb.Setup(db => db.Disconnect());
            mockDb.Setup(db => db.ExecuteQuery(It.IsAny<string>())).Returns(1);

            NewsRepository mockRepository = new NewsRepository(mockDb.Object);

            int postId = 4;
            string correctQuery = $"UPDATE NewsPosts SET nrDislikes = nrDislikes + 1 WHERE id = {postId}";

            // Act
            int executionResult = mockRepository.UpdatePostDislikeCount(postId);

            // Assert
            mockDb.Verify(db => db.Connect(), Times.Once, "Connection to database should happen once!");
            mockDb.Verify(db => db.Disconnect(), Times.Once, "Closing the database should happen once!");
            mockDb.Verify(db => db.ExecuteQuery(correctQuery), Times.Once, "Method should execute with expected query!");
            Assert.AreEqual(1, executionResult, "Execution result should be more than one with correct query");
        }

        [TestMethod]
        public void UpdatePostDislikeCount_WhenExecuteQueryFails_ThrowsException()
        {
            // Arrange
            var mockDb = new Mock<INewsDatabase>();
            mockDb.Setup(db => db.Connect());
            mockDb.Setup(db => db.Disconnect());
            mockDb.Setup(db => db.ExecuteQuery(It.IsAny<string>())).Throws(new Exception("Error"));

            NewsRepository mockRepository = new NewsRepository(mockDb.Object);

            // Act & Assert
            Assert.ThrowsException<Exception>(() => mockRepository.UpdatePostDislikeCount(4));
        }

        [TestMethod]
        public void AddRatingToPost_WithCorrectQuery_ExecutesQueryAndReturnsResult()
        {
            // Arrange
            var mockDb = new Mock<INewsDatabase>();
            mockDb.Setup(db => db.Connect());
            mockDb.Setup(db => db.Disconnect());
            mockDb.Setup(db => db.ExecuteQuery(It.IsAny<string>())).Returns(1);
            var mockRepository = new NewsRepository(mockDb.Object);
            int postId = 5;
            int userId = 1;
            int ratingType = 1;
            string correctQuery = $"INSERT INTO Ratings VALUES({postId}, {userId}, {ratingType})";

            // Act
            int executionResult = mockRepository.AddRatingToPost(postId, userId, ratingType);

            // Assert
            mockDb.Verify(db => db.Connect(), Times.Once, "Connection to database should happen once!");
            mockDb.Verify(db => db.Disconnect(), Times.Once, "Closing the database should happen once!");
            mockDb.Verify(db => db.ExecuteQuery(correctQuery), Times.Once, "Method should execute with expected query!");
            Assert.AreEqual(1, executionResult, "Execution result should be more than one with correct query");
        }

        [TestMethod]
        public void AddRatingToPost_WithExecuteQueryFails_ThrowsException()
        {
            // Arrange
            var mockDb = new Mock<INewsDatabase>();
            mockDb.Setup(db => db.Connect());
            mockDb.Setup(db => db.Disconnect());
            mockDb.Setup(db => db.ExecuteQuery(It.IsAny<string>())).Throws(new Exception("Error"));
            var mockRepository = new NewsRepository(mockDb.Object);

            // Act & Assert
            Assert.ThrowsException<Exception>(() => mockRepository.AddRatingToPost(5, 1, 1));
        }

        [TestMethod]
        public void RemoveRatingFromPost_WithCorrectQuery_ExecutesQueryAndReturnsResult()
        {
            // Arrange
            var mockDb = new Mock<INewsDatabase>();
            mockDb.Setup(db => db.Connect());
            mockDb.Setup(db => db.Disconnect());
            mockDb.Setup(db => db.ExecuteQuery(It.IsAny<string>())).Returns(1);
            var mockRepository = new NewsRepository(mockDb.Object);
            int postId = 5;
            int userId = 1;
            string correctQuery = $"DELETE FROM Ratings WHERE postId={postId} AND authorId={userId}";

            // Act
            int executionResult = mockRepository.RemoveRatingFromPost(postId, userId);

            // Assert
            mockDb.Verify(db => db.Connect(), Times.Once, "Connection to database should happen once!");
            mockDb.Verify(db => db.Disconnect(), Times.Once, "Closing the database should happen once!");
            mockDb.Verify(db => db.ExecuteQuery(correctQuery), Times.Once, "Method should execute with expected query!");
            Assert.AreEqual(1, executionResult, "Execution result should be more than one with correct query");
        }

        [TestMethod]
        public void RemoveRatingFromPost_WithExecuteQueryFails_ThrowsException()
        {
            // Arrange
            var mockDb = new Mock<INewsDatabase>();
            mockDb.Setup(db => db.Connect());
            mockDb.Setup(db => db.Disconnect());
            mockDb.Setup(db => db.ExecuteQuery(It.IsAny<string>())).Throws(new Exception());
            var mockRepository = new NewsRepository(mockDb.Object);

            // Act & Assert
            Assert.ThrowsException<Exception>(() => mockRepository.RemoveRatingFromPost(5, 1));
        }

        [TestMethod]
        public void AddCommentToPost_WithCorrectQuery_ExecutesQueryAndReturnsResult()
        {
            // Arrange
            var mockDb = new Mock<INewsDatabase>();
            mockDb.Setup(db => db.Connect());
            mockDb.Setup(db => db.Disconnect());
            mockDb.Setup(db => db.ExecuteQuery(It.IsAny<string>())).Returns(1);
            var mockRepository = new NewsRepository(mockDb.Object);
            int postId = 5;
            int userId = 1;
            string commentContent = "test content";
            DateTime commentDate = DateTime.Now;
            string correctQuery = $"INSERT INTO NewsComments VALUES({userId}, {postId}, N'{commentContent}', '{commentDate}')";

            // Act
            int executionResult = mockRepository.AddCommentToPost(postId, commentContent, userId, commentDate);

            // Assert
            mockDb.Verify(db => db.Connect(), Times.Once, "Connection to database should happen once!");
            mockDb.Verify(db => db.Disconnect(), Times.Once, "Closing the database should happen once!");
            mockDb.Verify(db => db.ExecuteQuery(correctQuery), Times.Once, "Method should execute with expected query!");
            Assert.AreEqual(1, executionResult, "Execution result should be more than one with correct query");
        }

        [TestMethod]
        public void AddCommentToPost_WithExecuteQueryFails_ThrowsException()
        {
            // Arrange
            var mockDb = new Mock<INewsDatabase>();
            mockDb.Setup(db => db.Connect());
            mockDb.Setup(db => db.Disconnect());
            mockDb.Setup(db => db.ExecuteQuery(It.IsAny<string>())).Throws(new Exception("Error"));
            var mockRepository = new NewsRepository(mockDb.Object);
            int postId = 5;
            int userId = 1;
            string commentContent = "test content";
            DateTime commentDate = DateTime.Now;

            // Act & Assert
            Assert.ThrowsException<Exception>(() => mockRepository.AddCommentToPost(postId, commentContent, userId, commentDate));
        }

        [TestMethod]
        public void UpdateComment_WithCorrectQuery_ExectuesQueryAndReturnsResult()
        {
            // Arrange
            var mockDb = new Mock<INewsDatabase>();
            mockDb.Setup(db => db.Connect());
            mockDb.Setup(db => db.Disconnect());
            mockDb.Setup(db => db.ExecuteQuery(It.IsAny<string>())).Returns(1);
            var mockRepository = new NewsRepository(mockDb.Object);
            string commentContent = "test content";
            int commentId = 1;
            string correctQuery = $"UPDATE NewsComments SET content=N'{commentContent}' WHERE id={commentId}";

            // Act
            int executionResult = mockRepository.UpdateComment(commentId, commentContent);

            // Arrange
            mockDb.Verify(db => db.Connect(), Times.Once, "Connection to database should happen once!");
            mockDb.Verify(db => db.Disconnect(), Times.Once, "Closing the database should happen once!");
            mockDb.Verify(db => db.ExecuteQuery(correctQuery), Times.Once, "Method should execute with expected query!");
            Assert.AreEqual(1, executionResult, "Execution result should be more than one with correct query");
        }

        [TestMethod]
        public void UpdateComment_WithExecuteQueryFails_ThrowsException()
        {
            // Arrange
            var mockDb = new Mock<INewsDatabase>();
            mockDb.Setup(db => db.Connect());
            mockDb.Setup(db => db.Disconnect());
            mockDb.Setup(db => db.ExecuteQuery(It.IsAny<string>())).Throws(new Exception("Error"));
            var mockRepository = new NewsRepository(mockDb.Object);
            int commentId = 1;
            string commentContent = "test content";

            // Act & Assert
            Assert.ThrowsException<Exception>(() => mockRepository.UpdateComment(commentId, commentContent));
        }

        [TestMethod]
        public void DeleteCommentFromDatabase_WithCorrectQuery_ExectuesQueryAndReturnsResult()
        {
            // Arrange
            var mockDb = new Mock<INewsDatabase>();
            mockDb.Setup(db => db.Connect());
            mockDb.Setup(db => db.Disconnect());
            mockDb.Setup(db => db.ExecuteQuery(It.IsAny<string>())).Returns(1);
            var mockRepository = new NewsRepository(mockDb.Object);
            int commentId = 1;
            string correctQuery = $"DELETE FROM NewsComments WHERE id={commentId}";

            // Act
            int executionResult = mockRepository.DeleteCommentFromDatabase(commentId);

            // Arrange
            mockDb.Verify(db => db.Connect(), Times.Once, "Connection to database should happen once!");
            mockDb.Verify(db => db.Disconnect(), Times.Once, "Closing the database should happen once!");
            mockDb.Verify(db => db.ExecuteQuery(correctQuery), Times.Once, "Method should execute with expected query!");
            Assert.AreEqual(1, executionResult, "Execution result should be more than one with correct query");
        }

        [TestMethod]
        public void DeleteCommentFromDatabase_WithExecuteQueryFails_ThrowException()
        {
            // Arrange
            var mockDb = new Mock<INewsDatabase>();
            mockDb.Setup(db => db.Connect());
            mockDb.Setup(db => db.Disconnect());
            mockDb.Setup(db => db.ExecuteQuery(It.IsAny<string>())).Throws(new Exception("Error"));
            var mockRepository = new NewsRepository(mockDb.Object);
            int commentId = 1;

            // Act & Assert
            Assert.ThrowsException<Exception>(() => mockRepository.DeleteCommentFromDatabase(commentId));
        }

        [TestMethod]
        public void LoadFollowingComments_ForValidPostId_ReturnsComments()
        {
            // Arrange
            var mockDb = new Mock<INewsDatabase>();
            int postId = 5;

            var fakeComments = new List<Comment>
            {
                new Comment
                {
                    CommentId = 1,
                    PostId = postId,
                    AuthorId = 100,
                    Content = "Great article!",
                    CommentDate = new DateTime(2024, 4, 14)
                }
            };

            mockDb.Setup(d => d.Connect());
            mockDb.Setup(d => d.Disconnect());
            mockDb.Setup(d => d.FetchCommentsData(It.Is<string>(q => q.Contains($"postId={postId}"))))
                  .Returns(fakeComments);

            var repo = new NewsRepository(mockDb.Object);

            // Act
            var result = repo.LoadFollowingComments(postId);

            // Assert
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("Great article!", result[0].Content);
            Assert.AreEqual(postId, result[0].PostId);
        }

        [TestMethod]
        public void LoadFollowingComments_WhenFetchFails_ThrowsException()
        {
            // Arrange
            var mockDb = new Mock<INewsDatabase>();
            mockDb.Setup(d => d.Connect());
            mockDb.Setup(d => d.Disconnect());
            mockDb.Setup(d => d.FetchCommentsData(It.IsAny<string>())).Throws(new Exception("Error"));

            var mockRepository = new NewsRepository(mockDb.Object);

            // Act & Assert
            Assert.ThrowsException<Exception>(() => mockRepository.LoadFollowingComments(5));
        }

        [TestMethod]
        public void LoadFollowingComments_CallsConnectAndDisconnectOnce()
        {
            // Arrange
            var mockDb = new Mock<INewsDatabase>();
            mockDb.Setup(d => d.FetchCommentsData(It.IsAny<string>())).Returns(new List<Comment>());

            var mockRepository = new NewsRepository(mockDb.Object);

            // Act
            var result = mockRepository.LoadFollowingComments(1);

            // Assert
            mockDb.Verify(d => d.Connect(), Times.Once);
            mockDb.Verify(d => d.Disconnect(), Times.Once);
        }

        [TestMethod]
        public void AddPostToDatabase_WithCorrectQuery_ExecutesQueryAndReturnsResult()
        {
            // Arrange
            var mockDb = new Mock<INewsDatabase>();
            mockDb.Setup(db => db.Connect());
            mockDb.Setup(db => db.Disconnect());
            mockDb.Setup(db => db.ExecuteQuery(It.IsAny<string>())).Returns(1);
            var mockRepository = new NewsRepository(mockDb.Object);
            int userId = 1;
            string postContent = "Test Content";
            DateTime postDate = DateTime.Now;
            string correctQuery = $"INSERT INTO NewsPosts VALUES({userId}, N'{postContent}', '{postDate}', 0, 0, 0)";

            // Act
            int executionResult = mockRepository.AddPostToDatabase(userId, postContent, postDate);

            // Arrange
            mockDb.Verify(db => db.Connect(), Times.Once, "Connection to database should happen once!");
            mockDb.Verify(db => db.Disconnect(), Times.Once, "Closing the database should happen once!");
            mockDb.Verify(db => db.ExecuteQuery(correctQuery), Times.Once, "Method should execute with expected query!");
            Assert.AreEqual(1, executionResult, "Execution result should be more than one with correct query");
        }

        [TestMethod]
        public void AddPostToDatabase_WithExecuteQueryFails_ThrowException()
        {
            // Arrange
            var mockDb = new Mock<INewsDatabase>();
            mockDb.Setup(db => db.Connect());
            mockDb.Setup(db => db.Disconnect());
            mockDb.Setup(db => db.ExecuteQuery(It.IsAny<string>())).Throws(new Exception("Error"));
            var mockRepository = new NewsRepository(mockDb.Object);
            int userId = 1;
            string postContent = "Test Content";
            DateTime postDate = DateTime.Now;

            // Act & Assert
            Assert.ThrowsException<Exception>(() => mockRepository.AddPostToDatabase(userId, postContent, postDate));
        }

        [TestMethod]
        public void UpdatePost_WithCorrectQuery_ExectuesQueryAndReturnsResult()
        {
            // Arrange
            var mockDb = new Mock<INewsDatabase>();
            mockDb.Setup(db => db.Connect());
            mockDb.Setup(db => db.Disconnect());
            mockDb.Setup(db => db.ExecuteQuery(It.IsAny<string>())).Returns(1);
            var mockRepository = new NewsRepository(mockDb.Object);
            int postId = 5;
            string postContent = "Test Content";
            string correctQuery = $"UPDATE NewsPosts SET content=N'{postContent}' WHERE id={postId}";

            // Act
            int executionResult = mockRepository.UpdatePost(postId, postContent);

            // Arrange
            mockDb.Verify(db => db.Connect(), Times.Once, "Connection to database should happen once!");
            mockDb.Verify(db => db.Disconnect(), Times.Once, "Closing the database should happen once!");
            mockDb.Verify(db => db.ExecuteQuery(correctQuery), Times.Once, "Method should execute with expected query!");
            Assert.AreEqual(1, executionResult, "Execution result should be more than one with correct query");
        }

        [TestMethod]
        public void UpdatePost_WithExecuteQueryFails_ThrowsException()
        {
            // Arrange
            var mockDb = new Mock<INewsDatabase>();
            mockDb.Setup(db => db.Connect());
            mockDb.Setup(db => db.Disconnect());
            mockDb.Setup(db => db.ExecuteQuery(It.IsAny<string>())).Throws(new Exception("Error"));
            var mockRepository = new NewsRepository(mockDb.Object);
            int postId = 5;
            string postContent = "Test Content";
            DateTime postDate = DateTime.Now;

            // Act & Assert
            Assert.ThrowsException<Exception>(() => mockRepository.UpdatePost(postId, postContent));
        }

        [TestMethod]
        public void DeletePostFromDatabase_WithCorrectQuery_ExecutesQueryAndReturnsResult()
        {
            var mockDb = new Mock<INewsDatabase>();
            mockDb.Setup(db => db.Connect());
            mockDb.Setup(db => db.Disconnect());
            mockDb.Setup(db => db.ExecuteQuery(It.IsAny<string>())).Returns(1);
            var mockRepository = new NewsRepository(mockDb.Object);
            int postId = 5;

            string correctQuery = $"DELETE FROM NewsPosts WHERE id={postId}";

            // Act
            int executionResult = mockRepository.DeletePostFromDatabase(postId);

            // Arrange
            mockDb.Verify(db => db.Connect(), Times.Once, "Connection to database should happen once!");
            mockDb.Verify(db => db.Disconnect(), Times.Once, "Closing the database should happen once!");
            mockDb.Verify(db => db.ExecuteQuery(correctQuery), Times.Once, "Method should execute with expected query!");
            Assert.AreEqual(1, executionResult, "Execution result should be more than one with correct query");
        }

        [TestMethod]
        public void DeletePostFromDatabase_WithExecuteQueryFails_ThrowsException()
        {
            // Arrange
            var mockDb = new Mock<INewsDatabase>();
            mockDb.Setup(db => db.Connect());
            mockDb.Setup(db => db.Disconnect());
            mockDb.Setup(db => db.ExecuteQuery(It.IsAny<string>())).Throws(new Exception("Error"));
            var mockRepository = new NewsRepository(mockDb.Object);
            int postId = 5;

            // Act & Assert
            Assert.ThrowsException<Exception>(() => mockRepository.DeletePostFromDatabase(postId));
        }

        [TestMethod]
        public void LoadFollowingPosts_ReturnsPostsWithRatings()
        {
            // Arrange
            var mockDb = new Mock<INewsDatabase>();
            var mockRepository = new NewsRepository(mockDb.Object);
            int page = 1;
            int userId = 42;
            string search = "cool";

            var fakePosts = new List<Post>
        {
            new Post
            {
                Id = 10,
                AuthorId = 1,
                Content = "cool content",
                UploadDate = DateTime.Now,
                NrLikes = 3,
                NrDislikes = 1,
                NrComments = 0
            },
            new Post
            {
                Id = 11,
                AuthorId = 2,
                Content = "cool stuff",
                UploadDate = DateTime.Now,
                NrLikes = 5,
                NrDislikes = 0,
                NrComments = 0
            }
        };

            mockDb.Setup(d => d.Connect());
            mockDb.Setup(d => d.Disconnect());
            mockDb.Setup(d => d.FetchPostsData(It.IsAny<string>(), search)).Returns(fakePosts);
            mockDb.Setup(d => d.ExecuteScalar(It.Is<string>(q => q.Contains("postId=10")))).Returns(true);
            mockDb.Setup(d => d.ExecuteScalar(It.Is<string>(q => q.Contains("postId=11")))).Returns(false);

            // Act
            var result = mockRepository.LoadFollowingPosts(page, userId, search);

            // Assert
            Assert.AreEqual(2, result.Count, "Method should return 2 posts.");
            Assert.IsTrue(result[0].ActiveUserRating, "First post should have a positive rating (true).");
            Assert.IsFalse(result[1].ActiveUserRating, "Second post should have a negative rating (false).");
        }

        [TestMethod]
        public void LoadFollowingPosts_WithEmptyResult_ReturnsEmptyList()
        {
            // Arrange
            var mockDb = new Mock<INewsDatabase>();
            var mockRepository = new NewsRepository(mockDb.Object);

            mockDb.Setup(d => d.Connect());
            mockDb.Setup(d => d.Disconnect());
            mockDb.Setup(d => d.FetchPostsData(It.IsAny<string>(), It.IsAny<string>())).Returns(new List<Post>());

            // Act
            var executionResult = mockRepository.LoadFollowingPosts(1, 123, "test");

            // Assert
            Assert.AreEqual(0, executionResult.Count, "Result should be 0 after not finding any posts.");
        }

        [TestMethod]
        public void LoadFollowingPosts_WhenDatabaseFails_ThrowsException()
        {
            // Arrange
            var mockDb = new Mock<INewsDatabase>();
            var mockRepository = new NewsRepository(mockDb.Object);

            mockDb.Setup(d => d.Connect());
            mockDb.Setup(d => d.Disconnect());
            mockDb.Setup(d => d.FetchPostsData(It.IsAny<string>(), It.IsAny<string>())).Throws(new Exception("Error"));

            // Act & Assert
            Assert.ThrowsException<Exception>(() => mockRepository.LoadFollowingPosts(1, 42, ""));
        }

        [TestMethod]
        public void LoadFollowingPosts_CallsConnectDisconnectOnce()
        {
            // Arrange
            var mockDb = new Mock<INewsDatabase>();
            var mockRepository = new NewsRepository(mockDb.Object);

            mockDb.Setup(d => d.FetchPostsData(It.IsAny<string>(), It.IsAny<string>()))
                  .Returns(new List<Post>());

            // Act
            var result = mockRepository.LoadFollowingPosts(1, 42, "search");

            // Assert
            mockDb.Verify(d => d.Connect(), Times.Once);
            mockDb.Verify(d => d.Disconnect(), Times.Once);
        }

        [TestMethod]
        public void FormatAsPost_ReturnsParsedText()
        {
            // Arrange
            var mockRepository = new Mock<INewsRepository>();
            var service = new NewsService(mockRepository.Object);

            string randomTestText = "This is a <spoiler>test</spoiler> string.";

            // Act
            string parsedText = service.FormatAsPost(randomTestText);

            // Assert
            Assert.IsTrue(parsedText.StartsWith("<html>"), "Result should start with the <html> tag.");
            Assert.IsTrue(parsedText.Contains("This is a"), "Result should contain the original text.");
            Assert.IsTrue(parsedText.Contains("class=\"spoiler\""), "Spoiler tag should be converted to span with a spoiler class.");
            Assert.IsTrue(parsedText.Contains("</span>"), "Spoiler tag should be converted to span tag.");
        }

        [TestMethod]
        public void LikePost_WithAllUpdatesSucceeding_ReturnsTrue()
        {
            // Arrange
            var mockRepository = new Mock<INewsRepository>();
            var service = new NewsService(mockRepository.Object);
            int postId = 1;
            mockRepository.Setup(r => r.UpdatePostLikeCount(postId)).Returns(1);
            mockRepository.Setup(r => r.AddRatingToPost(postId, service.activeUser.id, 1)).Returns(1);

            // Act
            bool successfulExecution = service.LikePost(postId);

            // Assert
            Assert.IsTrue(successfulExecution);
        }

        [TestMethod]
        public void LikePost_WithOneUpdateSucceeding_ReturnsFalse()
        {
            // Arrange
            var mockRepository = new Mock<INewsRepository>();
            var service = new NewsService(mockRepository.Object);
            int postId = 1;
            mockRepository.Setup(r => r.UpdatePostLikeCount(postId)).Returns(1);
            mockRepository.Setup(r => r.AddRatingToPost(postId, service.activeUser.id, 1)).Returns(0); // Failed

            // Act
            bool successfulExecution = service.LikePost(postId);

            // Assert
            Assert.IsFalse(successfulExecution);
        }

        [TestMethod]
        public void LikePost_WithNoUpdateSucceeding_ReturnsFalse()
        {
            // Arrange
            var mockRepository = new Mock<INewsRepository>();
            var service = new NewsService(mockRepository.Object);
            int postId = 1;
            mockRepository.Setup(r => r.UpdatePostLikeCount(postId)).Returns(0);    // Failed
            mockRepository.Setup(r => r.AddRatingToPost(postId, service.activeUser.id, 1)).Returns(0);  // Failed

            // Act
            bool successfulExecution = service.LikePost(postId);

            // Assert
            Assert.IsFalse(successfulExecution);
        }

        [TestMethod]
        public void DislikePost_WithAllUpdatesSucceeding_ReturnsTrue()
        {
            // Arrange
            var mockRepository = new Mock<INewsRepository>();
            var service = new NewsService(mockRepository.Object);
            int postId = 1;
            mockRepository.Setup(r => r.UpdatePostLikeCount(postId)).Returns(1);
            mockRepository.Setup(r => r.AddRatingToPost(postId, service.activeUser.id, 0)).Returns(1);

            // Act
            bool successfulExecution = service.DislikePost(postId);

            // Assert
            Assert.IsTrue(successfulExecution);
        }

        [TestMethod]
        public void DislikePost_WithOneUpdateSucceeding_ReturnsFalse()
        {
            // Arrange
            var mockRepository = new Mock<INewsRepository>();
            var service = new NewsService(mockRepository.Object);
            int postId = 1;
            mockRepository.Setup(r => r.UpdatePostLikeCount(postId)).Returns(1);
            mockRepository.Setup(r => r.AddRatingToPost(postId, service.activeUser.id, 0)).Returns(0); // Failed

            // Act
            bool successfulExecution = service.DislikePost(postId);

            // Assert
            Assert.IsFalse(successfulExecution);
        }

        [TestMethod]
        public void DislikePost_WithNoUpdateSucceeding_ReturnsFalse()
        {
            // Arrange
            var mockRepository = new Mock<INewsRepository>();
            var service = new NewsService(mockRepository.Object);
            int postId = 1;
            mockRepository.Setup(r => r.UpdatePostLikeCount(postId)).Returns(0);    // Failed
            mockRepository.Setup(r => r.AddRatingToPost(postId, service.activeUser.id, 0)).Returns(0);  // Failed

            // Act
            bool successfulExecution = service.DislikePost(postId);

            // Assert
            Assert.IsFalse(successfulExecution);
        }

        [TestMethod]
        public void RemoveRatingFromPost_WithRemovalSucceeding_ReturnsTrue()
        {
            // Arrange
            var mockRepository = new Mock<INewsRepository>();
            var service = new NewsService(mockRepository.Object);
            int postId = 1;
            mockRepository.Setup(r => r.RemoveRatingFromPost(postId, service.activeUser.id)).Returns(1);

            // Act
            bool successfulExecution = service.RemoveRatingFromPost(postId);

            // Assert
            Assert.IsTrue(successfulExecution);
        }

        [TestMethod]
        public void RemoveRatingFromPost_WithUnsuccessfulRemoval_ReturnsFalse()
        {
            // Arrange
            var mockRepository = new Mock<INewsRepository>();
            var service = new NewsService(mockRepository.Object);
            int postId = 1;
            mockRepository.Setup(r => r.RemoveRatingFromPost(postId, service.activeUser.id)).Returns(0);

            // Act
            bool successfulExecution = service.RemoveRatingFromPost(postId);

            // Assert
            Assert.IsFalse(successfulExecution);
        }

        [TestMethod]
        public void SaveComment_WithInsertionSucceeding_ReturnsTrue()
        {
            // Arrange
            var mockRepository = new Mock<INewsRepository>();
            var service = new NewsService(mockRepository.Object);
            int postId = 1;
            string commentContent = "This is a comment";
            mockRepository.Setup(r => r.AddCommentToPost(postId, commentContent.Replace("'", "''"), service.activeUser.id, DateTime.Now)).Returns(1);

            // Act
            bool successfulExecution = service.SaveComment(postId, commentContent);

            // Assert
            Assert.IsTrue(successfulExecution);
        }

        [TestMethod]
        public void SaveComment_WithInsertionNotSucceeding_ReturnsFalse()
        {
            // Arrange
            var mockRepository = new Mock<INewsRepository>();
            var service = new NewsService(mockRepository.Object);
            int postId = 1;
            string commentContent = "This is a comment";
            mockRepository.Setup(r => r.AddCommentToPost(postId, commentContent, service.activeUser.id, DateTime.Now)).Returns(0);

            // Act
            bool successfulExecution = service.SaveComment(postId, commentContent);

            // Assert
            Assert.IsFalse(successfulExecution);
        }

        [TestMethod]
        public void UpdateComment_WithUpdateSucceeding_ReturnsTrue()
        {
            // Arrange
            var mockRepository = new Mock<INewsRepository>();
            var service = new NewsService(mockRepository.Object);
            int commentId = 1;
            string newCommentContent = "This is a new comment";
            mockRepository.Setup(r => r.UpdateComment(commentId, newCommentContent.Replace("'", "''"))).Returns(1);

            // Act
            bool successfulExecution = service.UpdateComment(commentId, newCommentContent);

            // Assert
            Assert.IsTrue(successfulExecution);
        }

        [TestMethod]
        public void UpdateComment_WithUpdateNotSucceeding_ReturnsFalse()
        {
            // Arrange
            var mockRepository = new Mock<INewsRepository>();
            var service = new NewsService(mockRepository.Object);
            int commentId = 1;
            string newCommentContent = "This is a new comment";
            mockRepository.Setup(r => r.UpdateComment(commentId, newCommentContent)).Returns(0);

            // Act
            bool successfulExecution = service.UpdateComment(commentId, newCommentContent);

            // Assert
            Assert.IsFalse(successfulExecution);
        }

        [TestMethod]
        public void DeleteComment_WithRemovalSucceeding_ReturnsTrue()
        {
            // Arrange
            var mockRepository = new Mock<INewsRepository>();
            var service = new NewsService(mockRepository.Object);
            int commentId = 1;
            mockRepository.Setup(r => r.DeleteCommentFromDatabase(commentId)).Returns(1);

            // Act
            bool successfulExecution = service.DeleteComment(commentId);

            // Assert
            Assert.IsTrue(successfulExecution);
        }

        [TestMethod]
        public void DeleteComment_WithRemovalNotSucceeding_ReturnsFalse()
        {
            // Arrange
            var mockRepository = new Mock<INewsRepository>();
            var service = new NewsService(mockRepository.Object);
            int commentId = 1;
            mockRepository.Setup(r => r.DeleteCommentFromDatabase(commentId)).Returns(0);

            // Act
            bool successfulExecution = service.DeleteComment(commentId);

            // Assert
            Assert.IsFalse(successfulExecution);
        }

        [TestMethod]
        public void LoadNextComments_WithExecutionSuccessful_ReturnsListOfComments()
        {
            // Arrange
            var mockRepository = new Mock<INewsRepository>();
            var service = new NewsService(mockRepository.Object);
            int postId = 1;
            var expectedComments = new List<Comment>
            {
                new Comment { CommentId = 1, Content = "Test comment", AuthorId = 2, PostId = postId, CommentDate = DateTime.Now }
            };
            mockRepository.Setup(r => r.LoadFollowingComments(postId)).Returns(expectedComments);

            // Act
            var loadedComments = service.LoadNextComments(postId);

            // Assert
            Assert.IsNotNull(loadedComments);
            Assert.AreEqual(expectedComments.Count, loadedComments.Count);
        }

        [TestMethod]
        public void LoadNextComments_WithNotExistingPost_ThrowsException()
        {
            // Arrange
            var mockRepository = new Mock<INewsRepository>();
            var service = new NewsService(mockRepository.Object);
            int postId = -1;
            mockRepository.Setup(r => r.LoadFollowingComments(postId)).Throws(new Exception("Error"));

            // Act & Assert
            Assert.ThrowsException<Exception>(() => service.LoadNextComments(postId));
        }

        [TestMethod]
        public void SavePost_WithInsertionSuccessful_ReturnsTrue()
        {
            // Arrange
            var mockRepository = new Mock<INewsRepository>();
            var service = new NewsService(mockRepository.Object);
            string postContent = "Post content";
            mockRepository.Setup(r => r.AddPostToDatabase(service.activeUser.id, postContent.Replace("'", "''"), DateTime.Today)).Returns(1);

            // Act
            bool successfulExecution = service.SavePost(postContent);

            // Assert
            Assert.IsTrue(successfulExecution);
        }

        [TestMethod]
        public void SavePost_WithInsertionUnsuccessful_ReturnsFalse()
        {
            // Arrange
            var mockRepository = new Mock<INewsRepository>();
            var service = new NewsService(mockRepository.Object);
            string postContent = "Post content";
            mockRepository.Setup(r => r.AddPostToDatabase(service.activeUser.id, postContent, DateTime.Now)).Returns(0);

            // Act
            bool successfulExecution = service.SavePost(postContent);

            // Assert
            Assert.IsFalse(successfulExecution);
        }

        [TestMethod]
        public void UpdatePost_WithUpdateSuccessful_ReturnsTrue()
        {
            // Arrange
            var mockRepository = new Mock<INewsRepository>();
            var service = new NewsService(mockRepository.Object);
            int postId = 1;
            string newPostContent = "New Post content";
            mockRepository.Setup(r => r.UpdatePost(postId, newPostContent)).Returns(1);

            // Act
            bool successfulExecution = service.UpdatePost(postId, newPostContent);

            // Assert
            Assert.IsTrue(successfulExecution);
        }

        [TestMethod]
        public void UpdatePost_WithUpdateUnsuccessful_ReturnsFalse()
        {
            // Arrange
            var mockRepository = new Mock<INewsRepository>();
            var service = new NewsService(mockRepository.Object);
            int postId = -1;
            string newPostContent = "New Post content";
            mockRepository.Setup(r => r.UpdatePost(postId, newPostContent)).Returns(0);

            // Act
            bool successfulExecution = service.UpdatePost(postId, newPostContent);

            // Assert
            Assert.IsFalse(successfulExecution);
        }

        [TestMethod]
        public void DeletePost_WithRemovalSuccessful_ReturnsTrue()
        {
            // Arrange
            var mockRepository = new Mock<INewsRepository>();
            var service = new NewsService(mockRepository.Object);
            int postId = 1;
            mockRepository.Setup(r => r.DeletePostFromDatabase(postId)).Returns(1);

            // Act
            bool successfulExecution = service.DeletePost(postId);

            // Assert
            Assert.IsTrue(successfulExecution);
        }

        [TestMethod]
        public void DeletePost_WithRemovalUnsuccessful_ReturnsFalse()
        {
            // Arrange
            var mockRepository = new Mock<INewsRepository>();
            var service = new NewsService(mockRepository.Object);
            int postId = -1;
            mockRepository.Setup(r => r.DeletePostFromDatabase(postId)).Returns(0);

            // Act
            bool successfulExecution = service.DeletePost(postId);

            // Assert
            Assert.IsFalse(successfulExecution);
        }

        [TestMethod]
        public void LoadNextPosts_WithExecutionSuccessful_ReturnsListOfPosts()
        {
            // Arrange
            var mockRepository = new Mock<INewsRepository>();
            var service = new NewsService(mockRepository.Object);
            int pageNumber = 1;
            string searchedText = "cool";
            var expectedPosts = new List<Post>
            {
                new Post { Id = 10,
                AuthorId = 1,
                Content = "cool content",
                UploadDate = DateTime.Today,
                NrLikes = 3,
                NrDislikes = 1,
                NrComments = 0 }
            };
            mockRepository.Setup(r => r.LoadFollowingPosts(pageNumber, service.activeUser.id, searchedText)).Returns(expectedPosts);

            // Act
            var loadedPosts = service.LoadNextPosts(pageNumber, searchedText);

            // Assert
            Assert.IsNotNull(loadedPosts);
            Assert.AreEqual(expectedPosts.Count, loadedPosts.Count);
        }

        [TestMethod]
        public void SetStringOnEditMode_WithEditModeOn_ReturnsSaveContent()
        {
            // Arrange
            var service = new NewsService();    // Using actual service instance, since this method doesn't modify any data
            bool fakeEditMode = true;

            // Act
            string buttonContent = service.SetStringOnEditMode(fakeEditMode);

            // Assert
            Assert.IsNotNull(buttonContent);
            Assert.AreEqual("Save", buttonContent);
        }

        [TestMethod]
        public void SetStringOnEditMode_WithEditModeOff_ReturnsPostContent()
        {
            // Arrange
            var service = new NewsService();    // Using actual service instance, since this method doesn't modify any data
            bool fakeEditMode = false;

            // Act
            string buttonContent = service.SetStringOnEditMode(fakeEditMode);

            // Assert
            Assert.IsNotNull(buttonContent);
            Assert.AreEqual("Post Comment", buttonContent);
        }

        [TestMethod]
        public void SetCommentMethodOnEditMode_EditModeOn_CallsUpdateComment()
        {
            // Arrange
            var service = new FakeNewsService();

            // Act
            var result = service.SetCommentMethodOnEditMode(true, 10, 5, "edited comment");

            // Assert
            Assert.IsTrue(result);
            Assert.IsTrue(service.UpdateCommentCalled);
            Assert.IsFalse(service.SaveCommentCalled);
        }

        [TestMethod]
        public void SetCommentMethodOnEditMode_EditModeOff_CallsSaveComment()
        {
            // Arrange
            var service = new FakeNewsService();

            // Act
            var result = service.SetCommentMethodOnEditMode(false, 10, 5, "new comment");

            // Assert
            Assert.IsTrue(result);
            Assert.IsTrue(service.SaveCommentCalled);
            Assert.IsFalse(service.UpdateCommentCalled);
        }

        [TestMethod]
        public void ExecutePostMethodOnEditMode_EditModeOn_CallsUpdatePost()
        {
            // Arrange
            var service = new FakeNewsService();

            // Act
            service.ExecutePostMethodOnEditMode(true, "updated post", 42);

            // Assert
            Assert.IsTrue(service.UpdatePostCalled);
            Assert.IsFalse(service.SavePostCalled);
        }

        [TestMethod]
        public void ExecutePostMethodOnEditMode_EditModeOff_CallsSavePost()
        {
            // Arrange
            var service = new FakeNewsService();

            // Act
            service.ExecutePostMethodOnEditMode(false, "new post", 42);

            // Assert
            Assert.IsTrue(service.SavePostCalled);
            Assert.IsFalse(service.UpdatePostCalled);
        }

        [TestMethod]
        public void ExecutePostMethodOnEditMode_WithEmptyContent_CallsNothing()
        {
            // Arrange
            var service = new FakeNewsService();

            // Act
            service.ExecutePostMethodOnEditMode(false, "", 42);

            // Assert
            Assert.IsFalse(service.SavePostCalled);
            Assert.IsFalse(service.UpdatePostCalled);
        }
    }
}
