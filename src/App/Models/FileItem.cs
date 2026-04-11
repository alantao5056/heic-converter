using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Alan.HeicConverter.Models
{
    public class FileItem : INotifyPropertyChanged
    {
        public string Path { get; set; } = string.Empty;
        public string OriginalName { get; set; } = string.Empty;

        private string _convertedName = string.Empty;
        public string ConvertedName
        {
            get => _convertedName;
            set => SetProperty(ref _convertedName, value);
        }

        private FileStatus _status = FileStatus.Ready;
        public FileStatus Status
        {
            get => _status;
            set => SetProperty(ref _status, value);
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string? propertyName = null)
        {
            if (Equals(storage, value)) return false;

            storage = value;
            OnPropertyChanged(propertyName);
            return true;
        }
    }
}