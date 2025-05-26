using BusinessLayer.Models;
using System.Collections.Generic;

namespace SteamProfileWeb.ViewModels
{
    public class UserFeaturesViewModel
    {
        public List<Feature> UserFeatures { get; set; }
        public List<Feature> EquippedFeatures { get; set; }
        public int CurrentUserId { get; set; }
    }
} 