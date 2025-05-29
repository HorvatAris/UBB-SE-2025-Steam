using SteamHub.ApiContract.Models;
using System.Collections.Generic;

namespace SteamHub.Web.ViewModels
{
    public class FeaturesViewModel
    {
        public Dictionary<string, List<Feature>> FeaturesByCategories { get; set; }
        public int CurrentUserId { get; set; }
    }
} 