using System.Data;
using BusinessLayer.Data;
using BusinessLayer.Models;
using Microsoft.Data.SqlClient;
using BusinessLayer.Exceptions;
using BusinessLayer.Repositories.Interfaces;

namespace BusinessLayer.Repositories
{
    public class FeaturesRepository : IFeaturesRepository
    {
        private readonly IDataLink dataLink;

        private const string FeatureIdentifierString = "feature_id";
        private const string NameString = "name";
        private const string ValueString = "value";
        private const string DescriptionString = "description";
        private const string TypeString = "type";
        private const string SourceString = "source";

        public FeaturesRepository(IDataLink datalink)
        {
            dataLink = datalink ?? throw new ArgumentNullException(nameof(datalink));
        }

        public List<Feature> GetAllFeatures(int userId)
        {
            try
            {
                const string sqlCommand = @"
            SELECT 
                f.feature_id, 
                f.name, 
                f.value, 
                f.description, 
                f.type, 
                f.source,
                CASE WHEN fu.equipped = 1 THEN 1 ELSE 0 END AS equipped
            FROM Features f
            LEFT JOIN Feature_User fu ON f.feature_id = fu.feature_id AND fu.user_id = @userId
            ORDER BY f.type, f.value DESC;";

                var parameters = new SqlParameter[]
                {
            new SqlParameter("@userId", userId)
                };

                var features = new List<Feature>();
                var dataTable = dataLink.ExecuteReaderSql(sqlCommand, parameters);

                foreach (DataRow row in dataTable.Rows)
                {
                    features.Add(new Feature
                    {
                        FeatureId = Convert.ToInt32(row[FeatureIdentifierString]),
                        Name = row[NameString].ToString(),
                        Value = Convert.ToInt32(row[ValueString]),
                        Description = row[DescriptionString].ToString(),
                        Type = row[TypeString].ToString(),
                        Source = row[SourceString].ToString(),
                        Equipped = Convert.ToInt32(row["equipped"]) == 1
                    });
                }

                return features;
            }
            catch (DatabaseOperationException exception)
            {
                throw new DatabaseOperationException("Failed to retrieve features.", exception);
            }
        }

        public List<Feature> GetFeaturesByType(string type)
        {
            try
            {
                const string sqlCommand = @"
            SELECT feature_id, name, value, description, type, source
            FROM Features
            WHERE type = @type
            ORDER BY value DESC;";

                var parameters = new[]
                {
            new SqlParameter("@type", type)
        };

                var dataTable = dataLink.ExecuteReaderSql(sqlCommand, parameters);
                var features = new List<Feature>();

                foreach (DataRow row in dataTable.Rows)
                {
                    features.Add(new Feature
                    {
                        FeatureId = Convert.ToInt32(row[FeatureIdentifierString]),
                        Name = row[NameString].ToString(),
                        Value = Convert.ToInt32(row[ValueString]),
                        Description = row[DescriptionString].ToString(),
                        Type = row[TypeString].ToString(),
                        Source = row[SourceString].ToString()
                    });
                }

                return features;
            }
            catch (DatabaseOperationException exception)
            {
                throw new DatabaseOperationException($"Failed to retrieve features of type {type}.", exception);
            }
        }

        public List<Feature> GetUserFeatures(int userIdentifier)
        {
            try
            {
                const string sqlCommand = @"
            SELECT f.feature_id, f.name, f.value, f.description, f.type, f.source,
                   CASE WHEN fu.feature_id IS NOT NULL THEN 1 ELSE 0 END AS is_owned,
                   ISNULL(fu.equipped, 0) AS equipped
            FROM Features f
            LEFT JOIN Feature_User fu ON f.feature_id = fu.feature_id AND fu.user_id = @userId
            ORDER BY f.type, f.value DESC;";

                var parameters = new[]
                {
            new SqlParameter("@userId", userIdentifier)
        };

                var dataTable = dataLink.ExecuteReaderSql(sqlCommand, parameters);
                var features = new List<Feature>();

                foreach (DataRow row in dataTable.Rows)
                {
                    features.Add(new Feature
                    {
                        FeatureId = Convert.ToInt32(row[FeatureIdentifierString]),
                        Name = row[NameString].ToString(),
                        Value = Convert.ToInt32(row[ValueString]),
                        Description = row[DescriptionString].ToString(),
                        Type = row[TypeString].ToString(),
                        Source = row[SourceString].ToString(),
                        Equipped = Convert.ToInt32(row["equipped"]) == 1
                    });
                }

                return features;
            }
            catch (DatabaseOperationException exception)
            {
                throw new DatabaseOperationException($"Failed to retrieve features for user {userIdentifier}.", exception);
            }
        }

        public bool EquipFeature(int userIdentifier, int featureIdentifier)
        {
            try
            {
                const string checkSql = @"
            SELECT * FROM Feature_User 
            WHERE user_id = @userId AND feature_id = @featureId;";

                var checkParameters = new[]
                {
            new SqlParameter("@userId", userIdentifier),
            new SqlParameter("@featureId", featureIdentifier)
        };

                var relationshipTable = dataLink.ExecuteReaderSql(checkSql, checkParameters);

                if (relationshipTable.Rows.Count > 0)
                {
                    const string updateSql = @"
                UPDATE Feature_User
                SET equipped = 1
                WHERE user_id = @userId AND feature_id = @featureId;";

                    dataLink.ExecuteNonQuerySql(updateSql, checkParameters);
                    return true;
                }

                return false; // Not purchased
            }
            catch (DatabaseOperationException exception)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to equip feature {featureIdentifier} for user {userIdentifier}: {exception.Message}");
                return false;
            }
        }

        public bool UnequipFeature(int userIdentifier, int featureIdentifier)
        {
            try
            {
                const string checkSql = @"
            SELECT * FROM Feature_User 
            WHERE user_id = @userId AND feature_id = @featureId;";

                var checkParameters = new[]
                {
            new SqlParameter("@userId", userIdentifier),
            new SqlParameter("@featureId", featureIdentifier)
        };

                var relationshipTable = dataLink.ExecuteReaderSql(checkSql, checkParameters);

                if (relationshipTable.Rows.Count > 0)
                {
                    const string updateSql = @"
                UPDATE Feature_User
                SET equipped = 0
                WHERE user_id = @userId AND feature_id = @featureId;";

                    dataLink.ExecuteNonQuerySql(updateSql, checkParameters);
                    return true;
                }

                return false; // Not purchased
            }
            catch (DatabaseOperationException exception)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to unequip feature {featureIdentifier} for user {userIdentifier}: {exception.Message}");
                return false;
            }
        }

        public bool UnequipFeaturesByType(int userIdentifier, string featureType)
        {
            try
            {
                const string sqlCommand = @"
            UPDATE fu
            SET equipped = 0
            FROM Feature_User fu
            JOIN Features f ON fu.feature_id = f.feature_id
            WHERE fu.user_id = @userId AND f.type = @featureType;";

                var parameters = new[]
                {
            new SqlParameter("@userId", userIdentifier),
            new SqlParameter("@featureType", featureType)
        };

                dataLink.ExecuteNonQuerySql(sqlCommand, parameters);
                return true;
            }
            catch (DatabaseOperationException)
            {
                return false;
            }
        }

        public bool IsFeaturePurchased(int userIdentifier, int featureIdentifier)
        {
            try
            {
                const string sqlCommand = @"
            SELECT * FROM Feature_User 
            WHERE user_id = @userId AND feature_id = @featureId;";

                var parameters = new[]
                {
            new SqlParameter("@userId", userIdentifier),
            new SqlParameter("@featureId", featureIdentifier)
        };

                var relationshipTable = dataLink.ExecuteReaderSql(sqlCommand, parameters);
                return relationshipTable.Rows.Count > 0;
            }
            catch (DatabaseOperationException exception)
            {
                throw new DatabaseOperationException("Failed to check feature purchase status.", exception);
            }
        }

        public bool AddUserFeature(int userIdentifier, int featureIdentifier)
        {
            try
            {
                if (IsFeaturePurchased(userIdentifier, featureIdentifier))
                {
                    return false;
                }

                const string insertSql = @"
            INSERT INTO Feature_User (user_id, feature_id, equipped)
            VALUES (@userId, @featureId, 0);";

                var parameters = new[]
                {
            new SqlParameter("@userId", userIdentifier),
            new SqlParameter("@featureId", featureIdentifier)
        };

                dataLink.ExecuteNonQuerySql(insertSql, parameters);
                return true;
            }
            catch (DatabaseOperationException)
            {
                return false;
            }
        }
    }
}
