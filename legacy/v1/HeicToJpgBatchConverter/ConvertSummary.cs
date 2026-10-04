using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeicToJpgBatchConverter
{
    internal class ConvertSummary
    {
        internal uint Total { get; set; }
        internal int Success { get; set; }
        internal int Fail { get; set; }
        internal int Ignore { get; set; }
    }
}
