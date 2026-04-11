using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alan.HeicConverter.Models
{
    public class ConversionResult
    {
        public string ConvertedFileName { get; set; } = string.Empty;
        public string? ErrorMessage { get; set; }
    }
}
