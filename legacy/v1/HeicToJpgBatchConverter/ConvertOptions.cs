using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeicToJpgBatchConverter
{
    internal class ConvertOptions
    {
        internal ConvertOptions(bool includeSubfolders, CollisionResolution collisionResolution, ImageFormat format, double imageQuality)
        {
            IncludeSubfolders = includeSubfolders;
            CollisionResolution = collisionResolution;
            Format = format;
            ImageQuality = imageQuality;
        }

        internal bool IncludeSubfolders { get; set; }
        internal CollisionResolution CollisionResolution { get; set; }
        internal ImageFormat Format { get; set; }
        internal double ImageQuality { get; set; }
    }
}
