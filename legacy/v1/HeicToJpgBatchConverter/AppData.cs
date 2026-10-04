using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeicToJpgBatchConverter
{
    internal class AppData : INotifyPropertyChanged
    {
        private string _sourceFolder;
        private string _targetFolder;
        private string _currentStatus;
        private double _progressPercentage;

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        public string SourceFolder
        {
            get
            {
                return _sourceFolder;
            }
            set
            {
                this._sourceFolder = value;
                OnPropertyChanged("SourceFolder");
            }
        }

        public string TargetFolder
        {
            get
            {
                return _targetFolder;
            }
            set
            {
                this._targetFolder = value;
                OnPropertyChanged("TargetFolder");
            }
        }

        public string CurrentStatus
        {
            get
            {
                return _currentStatus;
            }
            set
            {
                this._currentStatus = value;
                OnPropertyChanged("CurrentStatus");
            }
        }

        public double ProgressPercentage
        {
            get
            {
                return _progressPercentage;
            }
            set
            {
                this._progressPercentage = value;
                OnPropertyChanged("ProgressPercentage");
            }
        }
    }
}
