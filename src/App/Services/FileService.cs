using Alan.HeicConverter.Models;
using ImageMagick;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Alan.HeicConverter.Services
{
    internal static class FileService
    {
        internal static async Task<ConversionResult> ConvertAsync(string heicFile, string sourceBasePath, string outputPath, OutputFormat format, ConversionOptions options)
        {
            try
            {
                if (!File.Exists(heicFile))
                {
                    return new ConversionResult { ErrorMessage = "Source file does not exist." };
                }

                string targetDir = GetTargetDirectory(heicFile, sourceBasePath, outputPath);

                if (!Directory.Exists(targetDir))
                {
                    Directory.CreateDirectory(targetDir);
                }

                string extension = format.GetFileExtension();

                string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(heicFile);
                string targetFileName = fileNameWithoutExtension + extension;
                string targetFilePath = Path.Combine(targetDir, targetFileName);

                bool replaced = false;

                if (File.Exists(targetFilePath))
                {
                    switch (options.ConflictResolution)
                    {
                        case ConflictResolution.Ignore:
                            return new ConversionResult { Ignored = true, ConvertedFileName = targetFileName };
                        case ConflictResolution.Replace:
                            replaced = true;
                            break;
                        case ConflictResolution.GenerateUniqueName:
                            int counter = 1;
                            while (File.Exists(targetFilePath))
                            {
                                targetFileName = $"{fileNameWithoutExtension} ({counter}){extension}";
                                targetFilePath = Path.Combine(targetDir, targetFileName);
                                counter++;
                            }
                            break;
                    }
                }

                using (var image = new MagickImage())
                {
                    await image.ReadAsync(heicFile);
                    
                    image.Format = format.ToMagickFormat();

                    if (format == OutputFormat.Jpg)
                    {
                        image.Quality = (uint)options.ImageQuality;
                    }

                    await image.WriteAsync(targetFilePath);
                }

                if (options.OriginalFileHandling == OriginalFileHandling.Delete)
                {
                    try { File.Delete(heicFile); } catch { /* Ignore cleanup errors */ }
                }
                else if (options.OriginalFileHandling == OriginalFileHandling.MoveTo && !string.IsNullOrWhiteSpace(options.CustomMovePath))
                {
                    try
                    {
                        string moveDir = GetTargetDirectory(heicFile, sourceBasePath, options.CustomMovePath);
                        if (!Directory.Exists(moveDir))
                        {
                            Directory.CreateDirectory(moveDir);
                        }
                        
                        string moveFileName = Path.GetFileName(heicFile);
                        string moveFilePath = Path.Combine(moveDir, moveFileName);
                        
                        if (File.Exists(moveFilePath))
                        {
                            int counter = 1;
                            while (File.Exists(moveFilePath))
                            {
                                moveFileName = $"{Path.GetFileNameWithoutExtension(heicFile)} ({counter}){Path.GetExtension(heicFile)}";
                                moveFilePath = Path.Combine(moveDir, moveFileName);
                                counter++;
                            }
                        }
                        File.Move(heicFile, moveFilePath);
                    }
                    catch { /* Ignore cleanup errors */ }
                }

                return new ConversionResult
                {
                    ConvertedFileName = targetFileName,
                    Replaced = replaced,
                    Ignored = false
                };
            }
            catch (Exception ex)
            {
                return new ConversionResult { ErrorMessage = ex.Message };
            }
        }

        private static string GetTargetDirectory(string heicFile, string sourceBasePath, string targetBasePath)
        {
            string relativeDir = string.Empty;
            if (!string.IsNullOrWhiteSpace(sourceBasePath))
            {
                string fileDir = Path.GetDirectoryName(heicFile)!;
                if (fileDir.StartsWith(sourceBasePath, StringComparison.OrdinalIgnoreCase))
                {
                    relativeDir = Path.GetRelativePath(sourceBasePath, fileDir);
                    if (relativeDir == ".")
                    {
                        relativeDir = string.Empty;
                    }
                }
            }

            return string.IsNullOrEmpty(relativeDir) ? targetBasePath : Path.Combine(targetBasePath, relativeDir);
        }
    }
}