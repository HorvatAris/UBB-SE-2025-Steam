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
    public class IntegrationTests
    {
        private IRepository _repo;
        private IService _service;

        [TestInitialize]
        public void Setup()
        {
            _repo = new Repository();
            _service = new Service(_repo);
        }

        [TestMethod]
        public void GetFriendshipStatus_SameUser_ReturnsFriends()
        {
            var result = _service.GetFriendshipStatus(1, 1);
            Assert.AreEqual(FriendshipStatus.Friends, result);
        }

        [TestMethod]
        public void MessageRequest_SameSenderAndReceiver_ReturnsErrorCode()
        {
            int result = _service.MessageRequest(3, 3);
            Assert.AreEqual(Service.ERROR_CODE, result);
        }


        [TestMethod]
        public void ToggleFriendRequest_RequestSent_CancelsRequest()
        {
            _service.ToggleFriendRequest(FriendshipStatus.RequestSent, 1, 2);
        }

        [TestMethod]
        public void ToggleFriendRequest_RequestReceived_SendsRequest()
        {
            _service.ToggleFriendRequest(FriendshipStatus.RequestReceived, 2, 1);
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
        public void HandleMessageAcceptOrDecline_CallsRepository()
        {
            _service.HandleMessageAcceptOrDecline(1, 2);
        }

        [TestMethod]
        public void SendFriendRequest_CallsRepository()
        {
            _service.SendFriendRequest(1, 2);
        }

        [TestMethod]
        public void CancelFriendRequest_CallsRepository()
        {
            _service.CancelFriendRequest(1, 2);
        }
    }
}
