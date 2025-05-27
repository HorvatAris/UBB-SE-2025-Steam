using System.ComponentModel.DataAnnotations.Schema;

namespace SteamHub.Api.Entities
{
    public class Feature
    {
        public int FeatureId { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Value { get; set; }
        public string Description { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string Source { get; set; } = string.Empty;
        public bool Equipped { get; set; }

        // Navigation properties
        public ICollection<FeatureUser> Users { get; set; }
    }
}
