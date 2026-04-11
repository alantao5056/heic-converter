namespace Alan.HeicConverter.Models
{
    public class AppSettings
    {
        public bool IncludeSubfolders { get; set; } = true;
        public OutputFormat Format { get; set; } = OutputFormat.Jpg;
        public int JpgQuality { get; set; } = 85;
        public ConflictResolution ConflictResolution { get; set; } = ConflictResolution.GenerateUniqueName;
        public OriginalFileHandling OriginalFileHandling { get; set; } = OriginalFileHandling.Keep;
        public string CustomPath { get; set; } = string.Empty;
    }
}
