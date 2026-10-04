using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeicToJpgBatchConverter
{
    public class CollisionResolutionSelectionItem
    {
        public string Name { get; set; }
        public CollisionResolution Resolution { get; set; }

        public CollisionResolutionSelectionItem(string name, CollisionResolution resolution)
        {
            Name = name;
            Resolution = resolution;
        }
    }
}
