using ImageMagick;

namespace Alan.HeicConverter.Models
{
    public enum OutputFormat
    {
        Jpg,
        Png,
        Gif,
        Bmp
    }

    public static class OutputFormatExtensions
    {
        public static string GetFileExtension(this OutputFormat format)
        {
            return format switch
            {
                OutputFormat.Jpg => ".jpg",
                OutputFormat.Png => ".png",
                OutputFormat.Gif => ".gif",
                OutputFormat.Bmp => ".bmp",
                _ => ".jpg"
            };
        }

        public static MagickFormat ToMagickFormat(this OutputFormat format)
        {
            return format switch
            {
                OutputFormat.Jpg => MagickFormat.Jpeg,
                OutputFormat.Png => MagickFormat.Png,
                OutputFormat.Gif => MagickFormat.Gif,
                OutputFormat.Bmp => MagickFormat.Bmp,
                _ => MagickFormat.Jpeg
            };
        }
    }
}
