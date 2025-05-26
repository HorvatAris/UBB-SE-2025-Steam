using BusinessLayer.Models;
using System.Collections.Generic;

namespace SteamProfileWeb.ViewModels
{
    public class FeaturePreviewViewModel
    {
        public string ProfilePicturePath { get; set; }
        public string BioText { get; set; }
        public List<Feature> EquippedFeatures { get; set; }
        public int FeatureId { get; set; }
        public Feature PreviewedFeature { get; set; }
    }
} 