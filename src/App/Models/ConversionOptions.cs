using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alan.HeicConverter.Models
{
    internal class ConversionOptions
    {
        public int ImageQuality { get; set; } = 85;
        public ConflictResolution ConflictResolution { get; set; } = ConflictResolution.GenerateUniqueName;
        public OriginalFileHandling OriginalFileHandling { get; set; } = OriginalFileHandling.Keep;
        public string CustomMovePath { get; set; } = string.Empty;
    }
}
