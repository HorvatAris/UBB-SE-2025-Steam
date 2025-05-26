using NUnit.Framework;
using BusinessLayer.Models;
using BusinessLayer.Validators;

namespace Tests.Validators
{
    [TestFixture]
    public class FeaturesValidatorTests
    {
        [Test]
        public void ValidateFeature_WithValidFeature_ReturnsTrue()
        {
            // Arrange
            var feature = new Feature
            {
                FeatureId = 1,
                Name = "Gold Frame",
                Type = "frame",
                Value = 100,
                Description = "A premium gold frame"
            };

            // Act
            var result = FeaturesValidator.ValidateFeature(feature);

            // Assert
            Assert.That(result.isValid, Is.True);
        }

        [Test]
        public void ValidateFeature_WithValidFeature_ReturnsEmptyErrorMessage()
        {
            // Arrange
            var feature = new Feature
            {
                FeatureId = 1,
                Name = "Gold Frame",
                Type = "frame",
                Value = 100,
                Description = "A premium gold frame"
            };

            // Act
            var result = FeaturesValidator.ValidateFeature(feature);

            // Assert
            Assert.That(result.errorMessage, Is.Empty);
        }

        [Test]
        public void ValidateFeature_WithNullFeature_ReturnsFalse()
        {
            // Arrange
            Feature feature = null;

            // Act
            var result = FeaturesValidator.ValidateFeature(feature);

            // Assert
            Assert.That(result.isValid, Is.False);
        }

        [Test]
        public void ValidateFeature_WithNullFeature_ReturnsNullErrorMessage()
        {
            // Arrange
            Feature feature = null;

            // Act
            var result = FeaturesValidator.ValidateFeature(feature);

            // Assert
            Assert.That(result.errorMessage, Is.EqualTo("Feature cannot be null."));
        }

        [Test]
        public void ValidateFeature_WithEmptyName_ReturnsFalse()
        {
            // Arrange
            var feature = new Feature
            {
                FeatureId = 1,
                Name = string.Empty,
                Type = "frame",
                Value = 100,
                Description = "A premium gold frame"
            };

            // Act
            var result = FeaturesValidator.ValidateFeature(feature);

            // Assert
            Assert.That(result.isValid, Is.False);
        }

        [Test]
        public void ValidateFeature_WithEmptyName_ReturnsEmptyNameErrorMessage()
        {
            // Arrange
            var feature = new Feature
            {
                FeatureId = 1,
                Name = string.Empty,
                Type = "frame",
                Value = 100,
                Description = "A premium gold frame"
            };

            // Act
            var result = FeaturesValidator.ValidateFeature(feature);

            // Assert
            Assert.That(result.errorMessage, Is.EqualTo("Feature name cannot be empty."));
        }

        [Test]
        public void ValidateFeature_WithNullName_ReturnsFalse()
        {
            // Arrange
            var feature = new Feature
            {
                FeatureId = 1,
                Name = null,
                Type = "frame",
                Value = 100,
                Description = "A premium gold frame"
            };

            // Act
            var result = FeaturesValidator.ValidateFeature(feature);

            // Assert
            Assert.That(result.isValid, Is.False);
        }

        [Test]
        public void ValidateFeature_WithNullName_ReturnsEmptyNameErrorMessage()
        {
            // Arrange
            var feature = new Feature
            {
                FeatureId = 1,
                Name = null,
                Type = "frame",
                Value = 100,
                Description = "A premium gold frame"
            };

            // Act
            var result = FeaturesValidator.ValidateFeature(feature);

            // Assert
            Assert.That(result.errorMessage, Is.EqualTo("Feature name cannot be empty."));
        }

        [Test]
        public void ValidateFeature_WithEmptyType_ReturnsFalse()
        {
            // Arrange
            var feature = new Feature
            {
                FeatureId = 1,
                Name = "Gold Frame",
                Type = string.Empty,
                Value = 100,
                Description = "A premium gold frame"
            };

            // Act
            var result = FeaturesValidator.ValidateFeature(feature);

            // Assert
            Assert.That(result.isValid, Is.False);
        }

        [Test]
        public void ValidateFeature_WithEmptyType_ReturnsEmptyTypeErrorMessage()
        {
            // Arrange
            var feature = new Feature
            {
                FeatureId = 1,
                Name = "Gold Frame",
                Type = string.Empty,
                Value = 100,
                Description = "A premium gold frame"
            };

            // Act
            var result = FeaturesValidator.ValidateFeature(feature);

            // Assert
            Assert.That(result.errorMessage, Is.EqualTo("Feature type cannot be empty."));
        }

        [Test]
        public void ValidateFeature_WithNullType_ReturnsFalse()
        {
            // Arrange
            var feature = new Feature
            {
                FeatureId = 1,
                Name = "Gold Frame",
                Type = null,
                Value = 100,
                Description = "A premium gold frame"
            };

            // Act
            var result = FeaturesValidator.ValidateFeature(feature);

            // Assert
            Assert.That(result.isValid, Is.False);
        }

        [Test]
        public void ValidateFeature_WithNullType_ReturnsEmptyTypeErrorMessage()
        {
            // Arrange
            var feature = new Feature
            {
                FeatureId = 1,
                Name = "Gold Frame",
                Type = null,
                Value = 100,
                Description = "A premium gold frame"
            };

            // Act
            var result = FeaturesValidator.ValidateFeature(feature);

            // Assert
            Assert.That(result.errorMessage, Is.EqualTo("Feature type cannot be empty."));
        }

        [Test]
        public void ValidateFeature_WithNegativeValue_ReturnsFalse()
        {
            // Arrange
            var feature = new Feature
            {
                FeatureId = 1,
                Name = "Gold Frame",
                Type = "frame",
                Value = -100,
                Description = "A premium gold frame"
            };

            // Act
            var result = FeaturesValidator.ValidateFeature(feature);

            // Assert
            Assert.That(result.isValid, Is.False);
        }

        [Test]
        public void ValidateFeature_WithNegativeValue_ReturnsNegativeValueErrorMessage()
        {
            // Arrange
            var feature = new Feature
            {
                FeatureId = 1,
                Name = "Gold Frame",
                Type = "frame",
                Value = -100,
                Description = "A premium gold frame"
            };

            // Act
            var result = FeaturesValidator.ValidateFeature(feature);

            // Assert
            Assert.That(result.errorMessage, Is.EqualTo("Feature value cannot be negative."));
        }

        [Test]
        public void ValidateFeatureType_WithValidType_ReturnsTrue()
        {
            // Arrange
            string type = "frame";

            // Act
            var result = FeaturesValidator.ValidateFeatureType(type);

            // Assert
            Assert.That(result.isValid, Is.True);
        }

        [Test]
        public void ValidateFeatureType_WithValidType_ReturnsEmptyErrorMessage()
        {
            // Arrange
            string type = "frame";

            // Act
            var result = FeaturesValidator.ValidateFeatureType(type);

            // Assert
            Assert.That(result.errorMessage, Is.Empty);
        }

        [Test]
        public void ValidateFeatureType_WithEmptyType_ReturnsFalse()
        {
            // Arrange
            string type = string.Empty;

            // Act
            var result = FeaturesValidator.ValidateFeatureType(type);

            // Assert
            Assert.That(result.isValid, Is.False);
        }

        [Test]
        public void ValidateFeatureType_WithEmptyType_ReturnsEmptyTypeErrorMessage()
        {
            // Arrange
            string type = string.Empty;

            // Act
            var result = FeaturesValidator.ValidateFeatureType(type);

            // Assert
            Assert.That(result.errorMessage, Is.EqualTo("Feature type cannot be empty."));
        }

        [Test]
        public void ValidateFeatureType_WithNullType_ReturnsFalse()
        {
            // Arrange
            string type = null;

            // Act
            var result = FeaturesValidator.ValidateFeatureType(type);

            // Assert
            Assert.That(result.isValid, Is.False);
        }

        [Test]
        public void ValidateFeatureType_WithNullType_ReturnsEmptyTypeErrorMessage()
        {
            // Arrange
            string type = null;

            // Act
            var result = FeaturesValidator.ValidateFeatureType(type);

            // Assert
            Assert.That(result.errorMessage, Is.EqualTo("Feature type cannot be empty."));
        }

        [Test]
        public void ValidateFeatureType_WithInvalidType_ReturnsFalse()
        {
            // Arrange
            string type = "invalid_type";

            // Act
            var result = FeaturesValidator.ValidateFeatureType(type);

            // Assert
            Assert.That(result.isValid, Is.False);
        }

        [Test]
        public void ValidateFeatureType_WithInvalidType_ReturnsInvalidTypeErrorMessage()
        {
            // Arrange
            string type = "invalid_type";

            // Act
            var result = FeaturesValidator.ValidateFeatureType(type);

            // Assert
            Assert.That(result.errorMessage, Is.EqualTo("Invalid feature type."));
        }

        [TestCase("frame")]
        [TestCase("emoji")]
        [TestCase("background")]
        [TestCase("pet")]
        [TestCase("hat")]
        public void ValidateFeatureType_WithValidTypes_ReturnsTrue(string type)
        {
            // Act
            var result = FeaturesValidator.ValidateFeatureType(type);

            // Assert
            Assert.That(result.isValid, Is.True);
        }

        [TestCase("frame")]
        [TestCase("emoji")]
        [TestCase("background")]
        [TestCase("pet")]
        [TestCase("hat")]
        public void ValidateFeatureType_WithValidTypes_ReturnsEmptyErrorMessage(string type)
        {
            // Act
            var result = FeaturesValidator.ValidateFeatureType(type);

            // Assert
            Assert.That(result.errorMessage, Is.Empty);
        }

        [Test]
        public void ValidateFeatureEquip_WithValidParameters_ReturnsTrue()
        {
            // Arrange
            int userId = 1;
            int featureId = 1;
            bool isPurchased = true;

            // Act
            var result = FeaturesValidator.ValidateFeatureEquip(userId, featureId, isPurchased);

            // Assert
            Assert.That(result.isValid, Is.True);
        }

        [Test]
        public void ValidateFeatureEquip_WithValidParameters_ReturnsEmptyErrorMessage()
        {
            // Arrange
            int userId = 1;
            int featureId = 1;
            bool isPurchased = true;

            // Act
            var result = FeaturesValidator.ValidateFeatureEquip(userId, featureId, isPurchased);

            // Assert
            Assert.That(result.errorMessage, Is.Empty);
        }

        [Test]
        public void ValidateFeatureEquip_WithInvalidUserId_ReturnsFalse()
        {
            // Arrange
            int userId = 0;
            int featureId = 1;
            bool isPurchased = true;

            // Act
            var result = FeaturesValidator.ValidateFeatureEquip(userId, featureId, isPurchased);

            // Assert
            Assert.That(result.isValid, Is.False);
        }

        [Test]
        public void ValidateFeatureEquip_WithInvalidUserId_ReturnsInvalidUserIdErrorMessage()
        {
            // Arrange
            int userId = 0;
            int featureId = 1;
            bool isPurchased = true;

            // Act
            var result = FeaturesValidator.ValidateFeatureEquip(userId, featureId, isPurchased);

            // Assert
            Assert.That(result.errorMessage, Is.EqualTo("Invalid user ID."));
        }

        [Test]
        public void ValidateFeatureEquip_WithInvalidFeatureId_ReturnsFalse()
        {
            // Arrange
            int userId = 1;
            int featureId = 0;
            bool isPurchased = true;

            // Act
            var result = FeaturesValidator.ValidateFeatureEquip(userId, featureId, isPurchased);

            // Assert
            Assert.That(result.isValid, Is.False);
        }

        [Test]
        public void ValidateFeatureEquip_WithInvalidFeatureId_ReturnsInvalidFeatureIdErrorMessage()
        {
            // Arrange
            int userId = 1;
            int featureId = 0;
            bool isPurchased = true;

            // Act
            var result = FeaturesValidator.ValidateFeatureEquip(userId, featureId, isPurchased);

            // Assert
            Assert.That(result.errorMessage, Is.EqualTo("Invalid feature ID."));
        }

        [Test]
        public void ValidateFeatureEquip_WithNotPurchasedFeature_ReturnsFalse()
        {
            // Arrange
            int userId = 1;
            int featureId = 1;
            bool isPurchased = false;

            // Act
            var result = FeaturesValidator.ValidateFeatureEquip(userId, featureId, isPurchased);

            // Assert
            Assert.That(result.isValid, Is.False);
        }

        [Test]
        public void ValidateFeatureEquip_WithNotPurchasedFeature_ReturnsFeatureNotPurchasedErrorMessage()
        {
            // Arrange
            int userId = 1;
            int featureId = 1;
            bool isPurchased = false;

            // Act
            var result = FeaturesValidator.ValidateFeatureEquip(userId, featureId, isPurchased);

            // Assert
            Assert.That(result.errorMessage, Is.EqualTo("Feature is not purchased by the user."));
        }
    }
}