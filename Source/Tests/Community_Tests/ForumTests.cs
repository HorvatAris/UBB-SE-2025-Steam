using Forum_Lib;

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
    //simple unit tests (no mocking)
    public sealed class ForumTests
    {
        [TestMethod]
        public void DatabaseConnectionCreatesOneConnectionPerObject()
        {
            //Arrange
            IDatabaseConnection databaseConnection1 = new DatabaseConnection();
            IDatabaseConnection databaseConnection2 = new DatabaseConnection();

            //Act


            //Assert
            Assert.IsFalse(databaseConnection1.Equals(databaseConnection2));
        }
        [TestMethod]
        public void GetForumRepoInstanceNeverReturnsNull()
        {
            //Arrange
            IForumRepository forumRepository = ForumRepository.GetRepoInstance();

            //Act


            //Assert
            Assert.IsNotNull(forumRepository);
        }

        [TestMethod]
        public void GetForumRepoInstanceAlwaysReturnsTheSameInstance()
        {
            //Arrange
            IForumRepository forumRepository1 = ForumRepository.GetRepoInstance();
            IForumRepository forumRepository2 = ForumRepository.GetRepoInstance();

            //Act


            //Assert
            Assert.IsTrue(forumRepository1.Equals(forumRepository2));
        }
        [TestMethod]
        public void GetForumServiceInstanceNeverReturnsNull()
        {
            //Arrange
            IForumService forumService = ForumService.GetForumServiceInstance();

            //Act


            //Assert
            Assert.IsNotNull(forumService);
        }

        [TestMethod]
        public void GetForumServiceInstanceAlwaysReturnsTheSameInstance()
        {
            //Arrange
            IForumService forumService1 = ForumService.GetForumServiceInstance();
            IForumService forumService2 = ForumService.GetForumServiceInstance();

            //Act


            //Assert
            Assert.IsTrue(forumService1.Equals(forumService2));
        }
    }
}
