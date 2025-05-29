using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SteamHub.ApiContract.Repositories;
using SteamHub.ApiContract.Models;
using FeatureUser=SteamHub.Api.Entities.FeatureUser;

namespace SteamHub.Api.Context.Repositories
{
    public class FeaturesRepository : IFeaturesRepository
    {
        private readonly DataContext context;

        public FeaturesRepository(DataContext context)
        {
            System.Diagnostics.Debug.WriteLine($"[DEBUG] FeaturesRepository constructor called. context is null: {context == null}");
            this.context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<List<Feature>> GetAllFeaturesAsync(int userId)
        {
                        return await context.Features
                .Select(feature => new Feature
                {
                    FeatureId = feature.FeatureId,
                    Name = feature.Name,
                    Value = feature.Value,
                    Description = feature.Description,
                    Type = feature.Type,
                    Source = feature.Source,
                    Equipped = feature.Equipped
                })
                .ToListAsync();
        }

        public async Task<List<Feature>> GetFeaturesByTypeAsync(string type)
        {
            return await context.Features
                .Where(feature => feature.Type == type)
                .OrderByDescending(feature => feature.Value)
                .Select(feature => new Feature
                {
                    FeatureId = feature.FeatureId,
                    Name = feature.Name,
                    Value = feature.Value,
                    Description = feature.Description,
                    Type = feature.Type,
                    Source = feature.Source,
                    Equipped = feature.Equipped
                })
                .ToListAsync();
        }

        public async Task<List<Feature>> GetUserFeaturesAsync(int userIdentifier)
        {
            return await context.Features
                .Join(
                    context.FeatureUsers.Where(featureUser => featureUser.UserId == userIdentifier),
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
                .OrderBy(feature => feature.Type)
                .ThenByDescending(feature => feature.Value)
                .ToListAsync();
        }

        public async Task<bool> IsFeaturePurchasedAsync(int userId, int featureId)
        {
            return await context.FeatureUsers
                .AnyAsync(featureUser => featureUser.UserId == userId && featureUser.FeatureId == featureId);
        }

        public async Task<bool> EquipFeatureAsync(int userId, int featureId)
        {
            var featureUser = await context.FeatureUsers
                .FirstOrDefaultAsync(featureUser => featureUser.UserId == userId && featureUser.FeatureId == featureId);

            if (featureUser == null)
            {
                featureUser = new FeatureUser
                {
                    UserId = userId,
                    FeatureId = featureId,
                    Equipped = true
                };
                await context.FeatureUsers.AddAsync(featureUser);
            }
            else
            {
                featureUser.Equipped = true;
            }

            await context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UnequipFeatureAsync(int userIdentifier, int featureIdentifier)
        {
            var featureUser = await context.FeatureUsers
                .FirstOrDefaultAsync(featureUser => featureUser.UserId == userIdentifier && featureUser.FeatureId == featureIdentifier);

            if (featureUser == null)
            {
                return false;
            }

            featureUser.Equipped = false;
            await context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UnequipFeaturesByTypeAsync(int userIdentifier, string featureType)
        {
            var featuresToUnequip = await context.FeatureUsers
                .Where(featureUser => featureUser.UserId == userIdentifier && featureUser.Feature.Type == featureType)
                .ToListAsync();

            foreach (var feature in featuresToUnequip)
            {
                feature.Equipped = false;
            }

            await context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> AddUserFeatureAsync(int userIdentifier, int featureIdentifier)
        {
            if (await IsFeaturePurchasedAsync(userIdentifier, featureIdentifier))
            {
                return false;
            }

            await context.FeatureUsers.AddAsync(new FeatureUser
            {
                UserId = userIdentifier,
                FeatureId = featureIdentifier,
                Equipped = false
            });

            await context.SaveChangesAsync();
            return true;
        }

        public async Task<List<Feature>> GetEquippedFeaturesAsync(int userId)
        {
            return await context.Features
                .Join(
                    context.FeatureUsers.Where(featureUser => featureUser.UserId == userId && featureUser.Equipped),
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
                .OrderBy(feature => feature.Type)
                .ThenByDescending(feature => feature.Value)
                .ToListAsync();
        }

        public async Task<Feature> GetFeatureByIdAsync(int featureId)
        {
            var feature = await context.Features
                .Where(feature => feature.FeatureId == featureId)
                .Select(feature => new Feature
                {
                    FeatureId = feature.FeatureId,
                    Name = feature.Name,
                    Value = feature.Value,
                    Description = feature.Description,
                    Type = feature.Type,
                    Source = feature.Source,
                    Equipped = feature.Equipped
                })
                .FirstOrDefaultAsync();

            return feature;
        }
    }
} 