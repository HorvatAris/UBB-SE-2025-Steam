using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Models
{
    public class FeatureUser
    {
        public int UserId { get; set; }
        public int FeatureId { get; set; }

        public bool Equipped { get; set; } = false;

        public Feature Feature { get; set; }
    }
}
