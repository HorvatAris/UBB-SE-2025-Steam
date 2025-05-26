using BusinessLayer.Models;
using System.Collections.Generic;

namespace SteamProfileWeb.ViewModels
{
    public class FeaturesViewModel
    {
        public Dictionary<string, List<Feature>> FeaturesByCategories { get; set; }
        public int CurrentUserId { get; set; }
    }
} 