using System;
using System.Collections.Generic;
using System.Data;
using BusinessLayer.Data;
using BusinessLayer.Exceptions;
using BusinessLayer.Models;
using BusinessLayer.Repositories;
using BusinessLayer.Repositories.Interfaces;
using Microsoft.Data.SqlClient;
using Moq;
using NUnit.Framework;

namespace Tests.RepositoryTests
{
    /*
    [TestFixture]
    public class FeaturesRepositoryTests
    {
        private Mock<IDataLink> mockDataLink;
        private IFeaturesRepository featuresRepository;

        [SetUp]
        public void Setup()
        {
            this.mockDataLink = new Mock<IDataLink>();
            this.featuresRepository = new FeaturesRepository(this.mockDataLink.Object);
        }

        [Test]
        public void Constructor_WithNullDataLink_ThrowsArgumentNullException()
        {
            // Act & Assert
            Assert.That(() => new FeaturesRepository(null), Throws.ArgumentNullException);
        }

        [Test]
        public void GetAllFeatures_ReturnsCorrectNumberOfFeatures()
        {
            // Arrange
            int userId = 1;
            var dataTable = CreateFeaturesDataTable();
            dataTable.Rows.Add(1, "Feature1", "Type1", 10, "Description1", "Source1", true);
            dataTable.Rows.Add(2, "Feature2", "Type2", 20, "Description2", "Source2", false);

            this.mockDataLink.Setup(dataLink => dataLink.ExecuteReaderSql(
                It.IsAny<string>(),
                It.Is<SqlParameter[]>(sqlParameter => sqlParameter.Length == 1 && (int)sqlParameter[0].Value == userId))).Returns(dataTable);

            // Act
            var result = this.featuresRepository.GetAllFeatures(userId);

            // Assert
            Assert.That(result.Count, Is.EqualTo(2));
        }

        [Test]
        public void GetAllFeatures_ReturnsFeatureWithCorrectName()
        {
            // Arrange
            int userId = 1;
            var dataTable = CreateFeaturesDataTable();
            dataTable.Rows.Add(1, "Feature1", "Type1", 10, "Description1", "Source1", true);

            this.mockDataLink.Setup(dataLink => dataLink.ExecuteReaderSql(
                It.IsAny<string>(),
                It.Is<SqlParameter[]>(sqlParameter => sqlParameter.Length == 1 && (int)sqlParameter[0].Value == userId))).Returns(dataTable);

            // Act
            var result = this.featuresRepository.GetAllFeatures(userId);

            // Assert
            Assert.That(result[0].Name, Is.EqualTo("Feature1"));
        }

        [Test]
        public void GetAllFeatures_ReturnsFeatureWithCorrectType()
        {
            // Arrange
            int userId = 1;
            var dataTable = CreateFeaturesDataTable();
            dataTable.Rows.Add(1, "Feature1", "Type1", 10, "Description1", "Source1", true);

            this.mockDataLink.Setup(dataLink => dataLink.ExecuteReaderSql(
                It.IsAny<string>(),
                It.Is<SqlParameter[]>(sqlParameter => sqlParameter.Length == 1 && (int)sqlParameter[0].Value == userId))).Returns(dataTable);

            // Act
            var result = this.featuresRepository.GetAllFeatures(userId);

            // Assert
            Assert.That(result[0].Type, Is.EqualTo("Type1"));
        }

        [Test]
        public void GetAllFeatures_ReturnsFeatureWithCorrectValue()
        {
            // Arrange
            int userId = 1;
            var dataTable = CreateFeaturesDataTable();
            dataTable.Rows.Add(1, "Feature1", "Type1", 10, "Description1", "Source1", true);

            this.mockDataLink.Setup(dataLink => dataLink.ExecuteReaderSql(
                It.IsAny<string>(),
                It.Is<SqlParameter[]>(sqlParameter => sqlParameter.Length == 1 && (int)sqlParameter[0].Value == userId))).Returns(dataTable);

            // Act
            var result = this.featuresRepository.GetAllFeatures(userId);

            // Assert
            Assert.That(result[0].Value, Is.EqualTo(10));
        }

        [Test]
        public void GetAllFeatures_ReturnsFeatureWithCorrectDescription()
        {
            // Arrange
            int userId = 1;
            var dataTable = CreateFeaturesDataTable();
            dataTable.Rows.Add(1, "Feature1", "Type1", 10, "Description1", "Source1", true);

            this.mockDataLink.Setup(dataLink => dataLink.ExecuteReaderSql(
                It.IsAny<string>(),
                It.Is<SqlParameter[]>(sqlParameter => sqlParameter.Length == 1 && (int)sqlParameter[0].Value == userId))).Returns(dataTable);

            // Act
            var result = this.featuresRepository.GetAllFeatures(userId);

            // Assert
            Assert.That(result[0].Description, Is.EqualTo("Description1"));
        }

        [Test]
        public void GetAllFeatures_ReturnsFeatureWithCorrectSource()
        {
            // Arrange
            int userId = 1;
            var dataTable = CreateFeaturesDataTable();
            dataTable.Rows.Add(1, "Feature1", "Type1", 10, "Description1", "Source1", true);

            this.mockDataLink.Setup(dataLink => dataLink.ExecuteReaderSql(
                It.IsAny<string>(),
                It.Is<SqlParameter[]>(sqlParameter => sqlParameter.Length == 1 && (int)sqlParameter[0].Value == userId))).Returns(dataTable);

            // Act
            var result = this.featuresRepository.GetAllFeatures(userId);

            // Assert
            Assert.That(result[0].Source, Is.EqualTo("Source1"));
        }

        [Test]
        public void GetAllFeatures_ReturnsFeatureWithCorrectEquippedStatus()
        {
            // Arrange
            int userId = 1;
            var dataTable = CreateFeaturesDataTable();
            dataTable.Rows.Add(1, "Feature1", "Type1", 10, "Description1", "Source1", true);

            this.mockDataLink.Setup(dataLink => dataLink.ExecuteReaderSql(
                It.IsAny<string>(),
                It.Is<SqlParameter[]>(sqlParameter => sqlParameter.Length == 1 && (int)sqlParameter[0].Value == userId))).Returns(dataTable);

            // Act
            var result = this.featuresRepository.GetAllFeatures(userId);

            // Assert
            Assert.That(result[0].Equipped, Is.True);
        }

        [Test]
        public void GetFeaturesByType_ReturnsCorrectNumberOfFeatures()
        {
            // Arrange
            string type = "Type1";
            var dataTable = CreateFeaturesDataTable();
            dataTable.Rows.Add(1, "Feature1", "Type1", 10, "Description1", "Source1", true);
            dataTable.Rows.Add(2, "Feature2", "Type1", 20, "Description2", "Source2", false);

            this.mockDataLink.Setup(dataLink => dataLink.ExecuteReaderSql(
                It.IsAny<string>(),
                It.Is<SqlParameter[]>(sqlParameter => sqlParameter.Length == 1 && (string)sqlParameter[0].Value == type))).Returns(dataTable);

            // Act
            var result = this.featuresRepository.GetFeaturesByType(type);

            // Assert
            Assert.That(result.Count, Is.EqualTo(2));
        }

        [Test]
        public void GetFeaturesByType_ReturnsFeatureWithCorrectType()
        {
            // Arrange
            string type = "Type1";
            var dataTable = CreateFeaturesDataTable();
            dataTable.Rows.Add(1, "Feature1", "Type1", 10, "Description1", "Source1", true);

            this.mockDataLink.Setup(dataLink => dataLink.ExecuteReaderSql(
                It.IsAny<string>(),
                It.Is<SqlParameter[]>(sqlParameter => sqlParameter.Length == 1 && (string)sqlParameter[0].Value == type))).Returns(dataTable);

            // Act
            var result = this.featuresRepository.GetFeaturesByType(type);

            // Assert
            Assert.That(result[0].Type, Is.EqualTo("Type1"));
        }

        [Test]
        public void GetUserFeatures_ReturnsCorrectNumberOfFeatures()
        {
            // Arrange
            int userId = 1;
            var dataTable = CreateFeaturesDataTable();
            dataTable.Rows.Add(1, "Feature1", "Type1", 10, "Description1", "Source1", true);
            dataTable.Rows.Add(2, "Feature2", "Type2", 20, "Description2", "Source2", false);

            this.mockDataLink.Setup(dataLink => dataLink.ExecuteReaderSql(
                It.IsAny<string>(),
                It.Is<SqlParameter[]>(sqlParameter => sqlParameter.Length == 1 && (int)sqlParameter[0].Value == userId))).Returns(dataTable);

            // Act
            var result = this.featuresRepository.GetUserFeatures(userId);

            // Assert
            Assert.That(result.Count, Is.EqualTo(2));
        }

        [Test]
        public void IsFeaturePurchased_WithPurchasedFeature_ReturnsTrue()
        {
            // Arrange
            int userId = 1;
            int featureId = 1;
            var dataTable = new DataTable();
            dataTable.Columns.Add("feature_id", typeof(int));
            dataTable.Rows.Add(featureId);

            this.mockDataLink.Setup(dataLink => dataLink.ExecuteReaderSql(
                It.IsAny<string>(),
                It.Is<SqlParameter[]>(sqlParameter =>
                    sqlParameter.Length == 2 &&
                    (int)sqlParameter[0].Value == userId &&
                    (int)sqlParameter[1].Value == featureId))).Returns(dataTable);

            // Act
            var result = this.featuresRepository.IsFeaturePurchased(userId, featureId);

            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public void IsFeaturePurchased_WithNotPurchasedFeature_ReturnsFalse()
        {
            // Arrange
            int userId = 1;
            int featureId = 1;
            var dataTable = new DataTable();
            dataTable.Columns.Add("feature_id", typeof(int));

            this.mockDataLink.Setup(dataLink => dataLink.ExecuteReaderSql(
                It.IsAny<string>(),
                It.Is<SqlParameter[]>(sqlParameter =>
                    sqlParameter.Length == 2 &&
                    (int)sqlParameter[0].Value == userId &&
                    (int)sqlParameter[1].Value == featureId))).Returns(dataTable);

            // Act
            var result = this.featuresRepository.IsFeaturePurchased(userId, featureId);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void EquipFeature_WithValidParameters_ReturnsTrue()
        {
            // Arrange
            int userId = 1;
            int featureId = 1;
            var relationshipTable = new DataTable();
            relationshipTable.Columns.Add("feature_id", typeof(int));
            relationshipTable.Rows.Add(featureId);

            this.mockDataLink.Setup(dataLink => dataLink.ExecuteReaderSql(
                It.IsAny<string>(),
                It.Is<SqlParameter[]>(sqlParameter =>
                    sqlParameter.Length == 2 &&
                    (int)sqlParameter[0].Value == userId &&
                    (int)sqlParameter[1].Value == featureId))).Returns(relationshipTable);

            this.mockDataLink.Setup(dataLink => dataLink.ExecuteNonQuerySql(
                It.IsAny<string>(),
                It.Is<SqlParameter[]>(sqlParameter =>
                    sqlParameter.Length == 3 &&
                    (int)sqlParameter[0].Value == userId &&
                    (int)sqlParameter[1].Value == featureId &&
                    (int)sqlParameter[2].Value == 1)));

            // Act
            var result = this.featuresRepository.EquipFeature(userId, featureId);

            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public void EquipFeature_WithNotPurchasedFeature_ReturnsFalse()
        {
            // Arrange
            int userId = 1;
            int featureId = 1;
            var relationshipTable = new DataTable();
            relationshipTable.Columns.Add("feature_id", typeof(int));

            this.mockDataLink.Setup(dataLink => dataLink.ExecuteReaderSql(
                It.IsAny<string>(),
                It.Is<SqlParameter[]>(sqlParameter =>
                    sqlParameter.Length == 2 &&
                    (int)sqlParameter[0].Value == userId &&
                    (int)sqlParameter[1].Value == featureId))).Returns(relationshipTable);

            // Act
            var result = this.featuresRepository.EquipFeature(userId, featureId);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void UnequipFeature_WithNotPurchasedFeature_ReturnsFalse()
        {
            // Arrange
            int userId = 1;
            int featureId = 1;
            var relationshipTable = new DataTable();
            relationshipTable.Columns.Add("feature_id", typeof(int));

            this.mockDataLink.Setup(dataLink => dataLink.ExecuteReaderSql(
                It.IsAny<string>(),
                It.Is<SqlParameter[]>(sqlParameter =>
                    sqlParameter.Length == 2 &&
                    (int)sqlParameter[0].Value == userId &&
                    (int)sqlParameter[1].Value == featureId))).Returns(relationshipTable);

            // Act
            var result = this.featuresRepository.UnequipFeature(userId, featureId);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void UnequipFeaturesByType_WithValidParameters_ReturnsTrue()
        {
            // Arrange
            int userId = 1;
            string featureType = "Type1";

            this.mockDataLink.Setup(dataLink => dataLink.ExecuteNonQuerySql(
                It.IsAny<string>(),
                It.Is<SqlParameter[]>(sqlParameter =>
                    sqlParameter.Length == 2 &&
                    (int)sqlParameter[0].Value == userId &&
                    (string)sqlParameter[1].Value == featureType)));

            // Act
            var result = this.featuresRepository.UnequipFeaturesByType(userId, featureType);

            // Assert
            Assert.That(result, Is.True);
        }

        private DataTable CreateFeaturesDataTable()
        {
            var dataTable = new DataTable();
            dataTable.Columns.Add("feature_id", typeof(int));
            dataTable.Columns.Add("name", typeof(string));
            dataTable.Columns.Add("type", typeof(string));
            dataTable.Columns.Add("value", typeof(int));
            dataTable.Columns.Add("description", typeof(string));
            dataTable.Columns.Add("source", typeof(string));
            dataTable.Columns.Add("equipped", typeof(bool));
            return dataTable;
        }
    }
    */
}