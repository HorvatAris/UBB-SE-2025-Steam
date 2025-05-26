using System.Data;
using BusinessLayer.Data;
using BusinessLayer.Models;
using Microsoft.Data.SqlClient;
using BusinessLayer.Exceptions;
using BusinessLayer.Repositories.Interfaces;
using BusinessLayer.DataContext;

namespace BusinessLayer.Repositories
{
    public class FeaturesRepository : IFeaturesRepository
    {
        private readonly ApplicationDbContext context;

        public FeaturesRepository(ApplicationDbContext newContext)
        {
            context = newContext ?? throw new ArgumentNullException(nameof(newContext));
        }

        public List<Feature> GetAllFeatures(int userId)
        {
            return context.Features
                .Select(f => new Feature
                {
                    FeatureId = f.FeatureId,
                    Name = f.Name,
                    Value = f.Value,
                    Description = f.Description,
                    Type = f.Type,
                    Source = f.Source,
                })
                .ToList();
        }

        public List<Feature> GetFeaturesByType(string type)
        {
            return context.Features
                .Where(f => f.Type == type)
                .OrderByDescending(f => f.Value)
                .ToList();
        }

        public List<Feature> GetUserFeatures(int userIdentifier)
        {
            return context.Features
        .Join(
            context.FeatureUsers.Where(fu => fu.UserId == userIdentifier),
            f => f.FeatureId,
            fu => fu.FeatureId,
            (f, fu) => new Feature
            {
                FeatureId = f.FeatureId,
                Name = f.Name,
                Value = f.Value,
                Description = f.Description,
                Type = f.Type,
                Source = f.Source,
                Equipped = fu != null ? fu.Equipped : false
            })
        .OrderBy(f => f.Type)
        .ThenByDescending(f => f.Value)
        .ToList();
        }

        public bool EquipFeature(int userId, int featureId)
        {
            // 1. Check if the feature exists
            var feature = context.Features.FirstOrDefault(f => f.FeatureId == featureId);
            if (feature == null)
            {
                return false;
            }
            var userExists = context.Users.Any(u => u.UserId == userId);
            if (!userExists)
            {
                return false;
            }
            var featureUser = context.FeatureUsers
                .FirstOrDefault(fu => fu.UserId == userId && fu.FeatureId == featureId);

            if (featureUser == null)
            {
                featureUser = new FeatureUser
                {
                    UserId = userId,
                    FeatureId = featureId,
                    Equipped = true
                };
                context.FeatureUsers.Add(featureUser);
            }
            else
            {
                featureUser.Equipped = true;
            }

            context.SaveChanges();
            return true;
        }

        public bool UnequipFeature(int userIdentifier, int featureIdentifier)
        {
            var featureUser = context.FeatureUsers
                                        .FirstOrDefault(fu => fu.UserId == userIdentifier && fu.FeatureId == featureIdentifier);
            if (featureUser == null)
            {
                return false; // Feature not purchased
            }
            featureUser.Equipped = false;
            context.SaveChanges();
            return true;
        }

        public bool UnequipFeaturesByType(int userIdentifier, string featureType)
        {
            var all = context.FeatureUsers
                                .Where(fu => fu.UserId == userIdentifier && fu.Feature.Type == featureType);
            foreach (var fu in all)
            {
                fu.Equipped = false;
            }
            context.SaveChanges();
            return true;
        }

        public bool IsFeaturePurchased(int userIdentifier, int featureIdentifier)
        {
            return context.FeatureUsers.Any(fu => fu.UserId == userIdentifier && fu.FeatureId == featureIdentifier);
        }

        public bool AddUserFeature(int userIdentifier, int featureIdentifier)
        {
            if (IsFeaturePurchased(userIdentifier, featureIdentifier))
            {
                return false;
            }
            context.FeatureUsers.Add(new FeatureUser
            {
                UserId = userIdentifier,
                FeatureId = featureIdentifier,
                Equipped = false
            });
            context.SaveChanges();
            return true;
        }

        public List<Feature> GetEquippedFeatures(int userId)
        {
            return context.Features
                .Join(
                    context.FeatureUsers.Where(fu => fu.UserId == userId && fu.Equipped),
                    feature => feature.FeatureId,
                    featureUser => featureUser.FeatureId,
                    (feature, featureUser) => new Feature
                    {
                        FeatureId = feature.FeatureId,
                        Name = feature.Name,
                        Value = feature.Value,
                        Description = feature.Description,
                        Type = feature.Type,
                        Source = feature.Source,
                        Equipped = featureUser.Equipped
                    })
                .OrderBy(f => f.Type)
                .ThenByDescending(f => f.Value)
                .ToList();
        }

        public Feature GetFeatureById(int featureId)
        {
            return context.Features
                .Where(f => f.FeatureId == featureId)
                .Select(f => new Feature
                {
                    FeatureId = f.FeatureId,
                    Name = f.Name,
                    Value = f.Value,
                    Description = f.Description,
                    Type = f.Type,
                    Source = f.Source,
                    Equipped = f.Equipped
                })
                .FirstOrDefault();
        }
    }
}
