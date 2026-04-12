using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Alan.HeicConverter.Models
{
    public class FileItem : INotifyPropertyChanged
    {
        public string Path { get; set; } = string.Empty;
        public string OriginalName { get; set; } = string.Empty;

        private OutputFormat _currentFormat = OutputFormat.Jpg;
        public OutputFormat CurrentFormat
        {
            get => _currentFormat;
            set
            {
                if (SetProperty(ref _currentFormat, value))
                {
                    OnPropertyChanged(nameof(ConvertedName));
                    OnPropertyChanged(nameof(Status));
                }
            }
        }

        private readonly Dictionary<OutputFormat, string> _convertedNames = new();
        public string ConvertedName
        {
            get => _convertedNames.TryGetValue(_currentFormat, out var name) ? name : string.Empty;
            set
            {
                _convertedNames[_currentFormat] = value;
                OnPropertyChanged();
            }
        }

        private readonly Dictionary<OutputFormat, FileStatus> _statuses = new();
        public FileStatus Status
        {
            get => _statuses.TryGetValue(_currentFormat, out var status) ? status : FileStatus.Ready;
            set
            {
                _statuses[_currentFormat] = value;
                OnPropertyChanged();
            }
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