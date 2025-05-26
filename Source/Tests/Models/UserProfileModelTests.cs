using BusinessLayer.Models;

namespace Tests.Models
{
    [TestFixture]
    internal class UserProfileModelTests
    {
        [Test]
        public void ProfileId_GetterSetter_ReturnsExpectedValue()
        {
            // Arrange
            var userProfile = new UserProfile();
            int expectedProfileId = 1;

            // Act
            userProfile.ProfileId = expectedProfileId;

            // Assert
            Assert.That(userProfile.ProfileId, Is.EqualTo(expectedProfileId));
        }

        [Test]
        public void UserId_GetterSetter_ReturnsExpectedValue()
        {
            // Arrange
            var userProfile = new UserProfile();
            int expectedUserId = 1;

            // Act
            userProfile.UserId = expectedUserId;

            // Assert
            Assert.That(userProfile.UserId, Is.EqualTo(expectedUserId));
        }

        [Test]
        public void ProfilePicture_GetterSetter_ReturnsExpectedValue()
        {
            // Arrange
            var userProfile = new UserProfile();
            string expectedProfilePicture = "profile.jpg";

            // Act
            userProfile.ProfilePicture = expectedProfilePicture;

            // Assert
            Assert.That(userProfile.ProfilePicture, Is.EqualTo(expectedProfilePicture));
        }

        [Test]
        public void Bio_GetterSetter_ReturnsExpectedValue()
        {
            // Arrange
            var userProfile = new UserProfile();
            string expectedBio = "This is a bio.";

            // Act
            userProfile.Bio = expectedBio;

            // Assert
            Assert.That(userProfile.Bio, Is.EqualTo(expectedBio));
        }

        [Test]
        public void LastModified_GetterSetter_ReturnsExpectedValue()
        {
            // Arrange
            var userProfile = new UserProfile();
            DateTime expectedLastModified = DateTime.Now;

            // Act
            userProfile.LastModified = expectedLastModified;

            // Assert
            Assert.That(userProfile.LastModified, Is.EqualTo(expectedLastModified));
        }
    }
}
