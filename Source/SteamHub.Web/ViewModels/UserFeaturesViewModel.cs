using SteamHub.ApiContract.Models;
using System.Collections.Generic;

namespace SteamHub.Web.ViewModels
{
    public class UserFeaturesViewModel
    {
        public List<Feature> UserFeatures { get; set; }
        public List<Feature> EquippedFeatures { get; set; }
        public int CurrentUserId { get; set; }
    }
} 