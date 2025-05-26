using System.ComponentModel.DataAnnotations.Schema;

namespace BusinessLayer.Models
{
    public class Feature
    {
        public int FeatureId { get; set; }
        public string Name { get; set; }
        public int Value { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }
        public string Source { get; set; }

        public bool Equipped { get; set; }
    }
}
