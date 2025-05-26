using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using Search;

//MUST HAVE THESE INCLUDES IN EACH TEST FILE
using Microsoft.UI.Xaml.Controls;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualStudio.TestTools.UnitTesting.AppContainer;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Tests.Community_Tests
{
    [TestClass]
    public class ServiceTests
    {
        private Mock<IRepository> _mockRepo;
        private IService _service;

        [TestInitialize]
        public void Setup()
        {
            _mockRepo = new Mock<IRepository>();
            _service = new Service(_mockRepo.Object);
        }

        [TestMethod]
        public void GetFriendshipStatus_SameUser_ReturnsFriends()
        {
            var result = _service.GetFriendshipStatus(1, 1);
            Assert.AreEqual(FriendshipStatus.Friends, result);
        }

        [TestMethod]
        public void GetFriendshipStatus_UsersAreFriends_ReturnsFriends()
        {
            _mockRepo.Setup(r => r.CheckFriendshipExists(1, 2)).Returns(true);

            var result = _service.GetFriendshipStatus(1, 2);

            Assert.AreEqual(FriendshipStatus.Friends, result);
        }

        [TestMethod]
        public void GetFriendshipStatus_RequestSent_ReturnsRequestSent()
        {
            _mockRepo.Setup(r => r.CheckFriendshipExists(1, 2)).Returns(false);
            _mockRepo.Setup(r => r.CheckFriendRequestExists(1, 2)).Returns(true);

            var result = _service.GetFriendshipStatus(1, 2);

            Assert.AreEqual(FriendshipStatus.RequestSent, result);
        }

        [TestMethod]
        public void MessageRequest_SameSenderAndReceiver_ReturnsErrorCode()
        {
            int result = _service.MessageRequest(3, 3);
            Assert.AreEqual(Service.ERROR_CODE, result);
        }

        [TestMethod]
        public void MessageRequest_RequestExists_RemovesRequest_ReturnsFound()
        {
            _mockRepo.Setup(r => r.CheckMessageInviteRequestExistance(1, 2)).Returns(true);

            int result = _service.MessageRequest(1, 2);

            _mockRepo.Verify(r => r.RemoveMessageRequest(It.IsAny<Dictionary<string, object>>()), Times.Once);
            Assert.AreEqual(Service.MESSAGE_REQUEST_FOUND, result);
        }

        [TestMethod]
        public void MessageRequest_RequestNotExists_SendsNewRequest_ReturnsNotFound()
        {
            _mockRepo.Setup(r => r.CheckMessageInviteRequestExistance(1, 2)).Returns(false);

            int result = _service.MessageRequest(1, 2);

            _mockRepo.Verify(r => r.SendNewMessageRequest(It.IsAny<Dictionary<string, object>>()), Times.Once);
            Assert.AreEqual(Service.MESSAGE_REQUEST_NOT_FOUND, result);
        }

        [TestMethod]
        public void ToggleFriendRequest_RequestSent_CancelsRequest()
        {
            _service.ToggleFriendRequest(FriendshipStatus.RequestSent, 1, 2);
            _mockRepo.Verify(r => r.CancelFriendRequest(1, 2), Times.Once);
        }

        [TestMethod]
        public void ToggleFriendRequest_RequestReceived_SendsRequest()
        {
            _service.ToggleFriendRequest(FriendshipStatus.RequestReceived, 2, 1);
            _mockRepo.Verify(r => r.SendFriendRequest(2, 1), Times.Once);
        }

        [TestMethod]
        public void SortAscending_ReturnsSortedList()
        {
            var unsorted = new List<User>
            {
                new User(1, "zeta", "127.0.0.1"),
                new User(2, "alpha", "127.0.0.1")
            };

            var sorted = _service.SortAscending(unsorted);

            Assert.AreEqual("alpha", sorted[0].UserName);
            Assert.AreEqual("zeta", sorted[1].UserName);
        }


        [TestMethod]
        public void GetFirst10UsersMatchedSorted_ReturnsSortedLimitedUsersWithStatus()
        {
            var users = new List<User>();
            for (int i = 15; i >= 1; i--)
            {
                users.Add(new User(i, "user" + i, "ip"));
            }

            _mockRepo.Setup(r => r.GetUsers(It.IsAny<string>())).Returns(users);
            _mockRepo.Setup(r => r.CheckFriendshipExists(It.IsAny<int>(), It.IsAny<int>())).Returns(true);

            var result = _service.GetFirst10UsersMatchedSorted("user");

            Assert.AreEqual(10, result.Count);
            Assert.IsTrue(result[0].UserName.CompareTo(result[1].UserName) < 0);
            Assert.AreEqual(FriendshipStatus.Friends, result[0].FriendshipStatus);
        }


        [TestMethod]
        public void GetUsersWhoSentMessageRequest_ReturnsCorrectUsers()
        {
            var ids = new List<int> { 1, 2 };
            var user1 = new User(1, "u1", "ip");
            var user2 = new User(2, "u2", "ip");

            _mockRepo.Setup(r => r.GetInvites(5)).Returns(ids);

            _mockRepo.Setup(r => r.GetUsers(It.Is<string>(s => s.Contains("1")))).Returns(new List<User> { user1 });
            _mockRepo.Setup(r => r.GetUsers(It.Is<string>(s => s.Contains("2")))).Returns(new List<User> { user2 });


            var result = _service.GetUsersWhoSentMessageRequest(5);

            Assert.AreEqual(2, result.Count);
        }

        [TestMethod]
        public void HandleMessageAcceptOrDecline_CallsRepository()
        {
            _service.HandleMessageAcceptOrDecline(1, 2);
            _mockRepo.Verify(r => r.RemoveMessageRequest(It.IsAny<Dictionary<string, object>>()), Times.Once);
        }

        [TestMethod]
        public void SendFriendRequest_CallsRepository()
        {
            _service.SendFriendRequest(1, 2);
            _mockRepo.Verify(r => r.SendFriendRequest(1, 2), Times.Once);
        }

        [TestMethod]
        public void CancelFriendRequest_CallsRepository()
        {
            _service.CancelFriendRequest(1, 2);
            _mockRepo.Verify(r => r.CancelFriendRequest(1, 2), Times.Once);
        }
    }
}
