using Alan.HeicConverter.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alan.HeicConverter.Services
{
    internal static class FileService
    {
        internal static ConversionResult Convert(string heicFile, string outputPath, OutputFormat format, ConversionOptions options)
        {
            var random = new Random();
            
            // Simulate random conversion time between 500ms and 2000ms
            System.Threading.Thread.Sleep(random.Next(500, 2000));

            // Simulate random failure (e.g., 20% chance of failure)
            if (random.NextDouble() < 0.2)
            {
                return new ConversionResult
                {
                    ErrorMessage = "Simulated conversion failure."
                };
            }

            return new ConversionResult
            {
                ConvertedFileName = System.IO.Path.GetFileNameWithoutExtension(heicFile) + ".jpg"
            };
        }
    }
}
