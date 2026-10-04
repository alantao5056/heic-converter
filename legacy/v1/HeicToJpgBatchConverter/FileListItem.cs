using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace HeicToJpgBatchConverter
{
    public class FileListItem : INotifyPropertyChanged
    {
        private string _convertedName;
        private Status _result;

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public string FullPath { get; set; }    // key
        public bool IsChecked { get; set; }        
        public string Path { get; set; }
        public DateTime? DateTaken { get; set; }
        public string OriginalName { get; set; }
        public StorageFile OriginalStorageFile { get; set; }
        public string ConvertedName 
        {
            get
            {
                return this._convertedName;
            }
            set
            {
                this._convertedName = value;
                OnPropertyChanged("ConvertedName");
            }
        }
        public Status Status
        {
            get
            {
                return this._result;
            }
            set
            {
                this._result = value;
                OnPropertyChanged("Status");
            }
        }
    }
}
