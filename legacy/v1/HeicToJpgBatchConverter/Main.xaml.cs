using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Search;
using Windows.System;
using Windows.UI;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace HeicToJpgBatchConverter
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Main : Page, INotifyPropertyChanged
    {
        private const string PRIMARY_COLOR = "#3379D9";
        private string _sourceFolderPath;
        private string _targetFolderPath;
        private bool _sameAsSourceFolder = true;
        private StorageFolder _sourceFolder;
        private StorageFolder _targetFolder;

        public Main()
        {
            Files = new ObservableCollection<FileListItem>();
            this.InitializeComponent();
        }

        public event PropertyChangedEventHandler PropertyChanged = delegate { };
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public ObservableCollection<FileListItem> Files { get; set; }

        public List<CollisionResolutionSelectionItem> CollisionResolutionSelectionItems
        {
            get
            {
                return new List<CollisionResolutionSelectionItem>()
                {
                    new CollisionResolutionSelectionItem("Generate Unique Name", CollisionResolution.GenerateUniqueName),
                    new CollisionResolutionSelectionItem("Replace", CollisionResolution.Replace),
                    new CollisionResolutionSelectionItem("Ignore", CollisionResolution.Ignore)
                };
            }
        }

        public List<ImageFormat> ImageFormats
        {
            get
            {
                return new List<ImageFormat>()
                {
                    ImageFormat.JPG,
                    ImageFormat.PNG,
                    ImageFormat.GIF,
                    ImageFormat.TIFF,
                    ImageFormat.BMP
                };
            }
        }

        private string _startButtonIconForeground = PRIMARY_COLOR;
        public string StartButtonIconForeground
        {
            get { return _startButtonIconForeground; }
            set
            {
                _startButtonIconForeground = value;
                OnPropertyChanged("StartButtonIconForeground");
            }
        }

        public string SourceFolderPath
        {
            get { return _sourceFolderPath; }
            set { 
                _sourceFolderPath = value;
                OnPropertyChanged(); 
            }
        }

        public string TargetFolderPath
        {
            get { return _targetFolderPath; }
            set
            {
                _targetFolderPath = value;
                OnPropertyChanged();
            }
        }

        public bool SameAsSourceFolder
        {
            get { return _sameAsSourceFolder; }
            set
            {
                _sameAsSourceFolder = value;
                OnPropertyChanged();
            }
        }

        private async void btnSourceFolder_Click(object sender, RoutedEventArgs e)
        {
            FolderPicker folderPicker = new FolderPicker();
            folderPicker.FileTypeFilter.Add("*");

            StorageFolder selectedFolder = await folderPicker.PickSingleFolderAsync();
            if (selectedFolder == null)
            {
                return;
            }

            _sourceFolder = selectedFolder;
            SourceFolderPath = _sourceFolder.Path;
            if (chbSameAsSourceFolder.IsChecked == true)
            {
                _targetFolder = selectedFolder;
                TargetFolderPath = _targetFolder.Path;
            }

            await _UpdateFiles();
        }

        private void btnRemove_Click(object sender, RoutedEventArgs e)
        {
            List<FileListItem> itemsToRemove = Files.Where(f => f.IsChecked).ToList();
            foreach (FileListItem item in itemsToRemove)
            {
                Files.Remove(item);
            }
        }

        private void cmbOutputFormat_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sliderJpgQuality != null)
            {
                sliderJpgQuality.Visibility = cmbOutputFormat.SelectedItem.ToString() == "JPG" ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        private async void btnTargetFolder_Click(object sender, RoutedEventArgs e)
        {
            FolderPicker folderPicker = new FolderPicker();
            folderPicker.FileTypeFilter.Add("*");

            StorageFolder selectedFolder = await folderPicker.PickSingleFolderAsync();
            if (selectedFolder == null)
            {
                return;
            }

            _targetFolder = selectedFolder;
            TargetFolderPath = _targetFolder.Path;
        }

        private void chbSameAsSourceFolder_Changed(object sender, RoutedEventArgs e)
        {
            if (chbSameAsSourceFolder.IsChecked == true)
            {
                _targetFolder = _sourceFolder;
                TargetFolderPath = SourceFolderPath;
            }
        }

        private async void chbIncludeSubfolders_Changed(object sender, RoutedEventArgs e)
        {
            if (_sourceFolder != null)
            {
                await _UpdateFiles();
            }
        }

        private async Task _UpdateFiles()
        {
            bool includeSubfolders = chbIncludeSubfolders.IsChecked == true;
            QueryOptions queryOptions = new QueryOptions();
            queryOptions.FileTypeFilter.Add(".heic");
            queryOptions.FolderDepth = includeSubfolders? FolderDepth.Deep : FolderDepth.Shallow;
            StorageFileQueryResult queryResult = _sourceFolder.CreateFileQueryWithOptions(queryOptions);
            IReadOnlyList<StorageFile> heicFiles = await queryResult.GetFilesAsync();
            
            Dictionary<string, FileListItem> filesDic = new Dictionary<string, FileListItem>();
            foreach (StorageFile file in heicFiles)
            {
                filesDic.Add(file.Path, new FileListItem()
                {
                    FullPath = file.Path,
                    IsChecked = false,
                    OriginalStorageFile = file,
                    OriginalName = file.Name,
                    Path = Path.GetDirectoryName(file.Path),
                    DateTaken = await _GetDateTakenAsync(file),
                });
            }

            List<FileListItem> toRemove = new List<FileListItem>();
            foreach (FileListItem file in Files)
            {
                if (filesDic.ContainsKey(file.FullPath))
                {
                    filesDic.Remove(file.FullPath);
                }
                else
                {
                    toRemove.Add(file);
                }
            }

            foreach (FileListItem file in toRemove)
            {
                Files.Remove(file);
            }

            foreach(FileListItem file in filesDic.Values)
            {
                Files.Add(file);
            }
        }

        private async Task<DateTime?> _GetDateTakenAsync(StorageFile file)
        {
            var imageProperties = await file.Properties.GetImagePropertiesAsync();
            if (imageProperties != null && imageProperties.DateTaken != null)
            {
                return imageProperties.DateTaken.DateTime;
            }
            return null;
        }

        private async void btnStartConvert_Click(object sender, RoutedEventArgs e)
        {
            if (!await _CheckHeicDecoder())
            {
                return;
            }
            _EnableDisableControls(false);
            try
            {
                if (!await _Validate())
                {
                    return;
                }
                await _Convert();
            }
            finally
            {
                _EnableDisableControls(true);
            }
        }

        private async Task<bool> _CheckHeicDecoder()
        {
            if (!Utils.IsHeifSupported())
            {
                var dialog = new MessageDialog(
                   "Your system does not currently support HEIC image decoding.\n\n" +
                   "Please install the Microsoft HEIF Image Extension to continue using this app.",
                   "Missing Plugin");

                dialog.Commands.Add(new UICommand("Go to Microsoft Store")
                {
                    Invoked = async cmd =>
                    {
                        // Microsoft HEIF Image Extension official ProductId
                        var uri = new Uri("ms-windows-store://pdp/?productid=9PMMSR1CGPWG");
                        await Launcher.LaunchUriAsync(uri);
                    }
                });

                dialog.Commands.Add(new UICommand("Cancel"));

                dialog.DefaultCommandIndex = 0;
                dialog.CancelCommandIndex = 1;

                await dialog.ShowAsync();
                return false;
            }
            return true;
        }

        private async Task _Convert()
        {
            ConvertOptions convertOptions = new ConvertOptions(
                chbIncludeSubfolders.IsChecked == true,
                ((CollisionResolutionSelectionItem)cmbFileNameConflict.SelectedItem).Resolution,
                (ImageFormat)cmbOutputFormat.SelectedItem,
                sliderJpgQuality.Value / 100);
            Converter converter = new Converter(Files, _sourceFolder.Path, _targetFolder, convertOptions);
            ConvertSummary summary = await converter.ConvertAsync();

            StringBuilder sb = new StringBuilder();
            sb.Append("Total processed: " + summary.Total);
            sb.AppendLine();
            sb.Append("    Success: " + summary.Success);
            sb.AppendLine();
            sb.Append("    Fail: " + summary.Fail);
            sb.AppendLine();
            sb.Append("    Ignore: " + summary.Ignore);

            MessageDialog dialog = new MessageDialog(sb.ToString(), "Complete!");
            await dialog.ShowAsync();
        }

        private async Task<bool> _Validate()
        {
            if (_sourceFolder == null || _targetFolder == null)
            {
                MessageDialog dialog = new MessageDialog("Please select both Source and Target folders.", "Invalid Input");
                await dialog.ShowAsync();
                return false;
            }
            return true;
        }

        private void _EnableDisableControls(bool isEnabled)
        {
            btnSourceFolder.IsEnabled = isEnabled;
            btnTargetFolder.IsEnabled = isEnabled;
            chbIncludeSubfolders.IsEnabled = isEnabled;
            chbSameAsSourceFolder.IsEnabled = isEnabled;
            btnStartConvert.IsEnabled = isEnabled;
            btnRemove.IsEnabled = isEnabled;
            cmbFileNameConflict.IsEnabled = isEnabled;
            cmbOutputFormat.IsEnabled = isEnabled;
            sliderJpgQuality.IsEnabled = isEnabled;
            StartButtonIconForeground = isEnabled ? PRIMARY_COLOR : "Gray";
        }
    }
}
