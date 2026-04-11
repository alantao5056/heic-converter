namespace Alan.HeicConverter.Models
{
    public class FileItem
    {
        public string Path { get; set; } = string.Empty;
        public string OriginalName { get; set; } = string.Empty;
        public string ConvertedName { get; set; } = string.Empty;
        public FileStatus Status { get; set; } = FileStatus.Ready;
    }
}