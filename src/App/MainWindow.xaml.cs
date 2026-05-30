using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Alan.HeicConverter.Services;
using Alan.HeicConverter.Models;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Alan.HeicConverter
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        public record FileHandlingOption(string DisplayName, OriginalFileHandling Value);

        public IReadOnlyList<FileHandlingOption> FileHandlingOptions { get; } = new[]
        {
            new FileHandlingOption("Keep", OriginalFileHandling.Keep),
            new FileHandlingOption("Delete", OriginalFileHandling.Delete),
            new FileHandlingOption("Move to …", OriginalFileHandling.MoveTo)
        };

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern uint GetDpiForWindow(IntPtr hwnd);

        private Microsoft.UI.Windowing.AppWindow _appWindow;

        private readonly RatingService _ratingService = new();

        public ObservableCollection<FileItem> Files { get; } = new();

        public MainWindow()
        {
            InitializeComponent();
            AppWindow.SetIcon("Assets/app.ico");

            var hWnd = WinRT.Interop.WindowNative.GetWindowHandle(this);
            var windowId = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(hWnd);
            _appWindow = Microsoft.UI.Windowing.AppWindow.GetFromWindowId(windowId);

            FileListView.ItemsSource = Files;

            RestoreWindowState();
            LoadSettings();

            this.Closed += MainWindow_Closed;

            if (this.Content is FrameworkElement rootElement)
            {
                rootElement.Loaded += RootElement_Loaded;
            }
        }

        private async void RootElement_Loaded(object sender, RoutedEventArgs e)
        {
            if (sender is FrameworkElement rootElement)
            {
                rootElement.Loaded -= RootElement_Loaded;
            }

            var appService = new AppService();
            bool hasUpdate = await appService.HasMandatoryUpdateAsync();
            if (hasUpdate)
            {
                var dialog = new ContentDialog
                {
                    Title = "Mandatory Update Required",
                    Content = "There is a mandatory update available on the Microsoft Store. Please update the app to continue using it.",
                    PrimaryButtonText = "Update Now",
                    CloseButtonText = "Exit App",
                    XamlRoot = this.Content.XamlRoot
                };

                var result = await dialog.ShowAsync();

                if (result == ContentDialogResult.Primary)
                {
                    // Launch Microsoft Store app page
                    var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(this);
                    string productId = await appService.GetStoreProductIdAsync(hwnd);
                    
                    if (!string.IsNullOrEmpty(productId))
                    {
                        await Windows.System.Launcher.LaunchUriAsync(new Uri($"ms-windows-store://pdp/?ProductId={productId}"));
                    }
                    else
                    {
                        try
                        {
                            // Fallback to dynamic PFN if Product ID could not be retrieved
                            string pfn = Windows.ApplicationModel.Package.Current.Id.FamilyName;
                            await Windows.System.Launcher.LaunchUriAsync(new Uri($"ms-windows-store://pdp/?PFN={pfn}"));
                        }
                        catch
                        {
                            // Ignore
                        }
                    }
                }

                Application.Current.Exit();
            }
        }

        private void RestoreWindowState()
        {
            var savedSize = SettingsService.GetWindowSize();
            if (savedSize.HasValue)
            {
                _appWindow.Resize(savedSize.Value);
            }
            else
            {
                SetDefaultSize();
            }

            var savedPosition = SettingsService.GetWindowPosition();
            if (savedPosition.HasValue)
            {
                _appWindow.Move(savedPosition.Value);
            }

            if (SettingsService.GetIsMaximized())
            {
                if (_appWindow.Presenter is Microsoft.UI.Windowing.OverlappedPresenter presenter)
                {
                    presenter.Maximize();
                }
            }
        }

        private void LoadSettings()
        {
            var settings = SettingsService.LoadAppSettings();

            IncludeSubfoldersCheckBox.IsChecked = settings.IncludeSubfolders;

            JpgButton.IsChecked = settings.Format == OutputFormat.Jpg;
            PngButton.IsChecked = settings.Format == OutputFormat.Png;
            GifButton.IsChecked = settings.Format == OutputFormat.Gif;
            BmpButton.IsChecked = settings.Format == OutputFormat.Bmp;

            JpgQualitySlider.Value = settings.JpgQuality;
            JpgQualityText.Text = $"{settings.JpgQuality}%";
            JpgQualityPanel.Visibility = (settings.Format == OutputFormat.Jpg) ? Visibility.Visible : Visibility.Collapsed;

            ConflictResolutionComboBox.SelectedIndex = (int)settings.ConflictResolution;
            OriginalFileHandlingComboBox.SelectedItem = FileHandlingOptions.FirstOrDefault(opt => opt.Value == settings.OriginalFileHandling) ?? FileHandlingOptions[0];
            CustomPathTextBox.Text = settings.CustomPath;

            SourceFolderTextBox.Text = string.Empty;
            TargetFolderTextBox.Text = string.Empty;
            
            UpdateStartConversionButtonStatus();
        }

        private void SetDefaultSize()
        {
            var hWnd = WinRT.Interop.WindowNative.GetWindowHandle(this);
            double dpi = GetDpiForWindow(hWnd);
            double scaleFactor = dpi / 96.0;

            int width = (int)(960 * scaleFactor);
            int height = (int)(640 * scaleFactor);

            _appWindow.Resize(new Windows.Graphics.SizeInt32(width, height));
        }

        private void MainWindow_Closed(object sender, WindowEventArgs args)
        {
            bool isMaximized = false;
            if (_appWindow.Presenter is Microsoft.UI.Windowing.OverlappedPresenter presenter)
            {
                isMaximized = presenter.State == Microsoft.UI.Windowing.OverlappedPresenterState.Maximized;
            }

            SettingsService.SaveWindowState(
                _appWindow.Size.Width,
                _appWindow.Size.Height,
                _appWindow.Position.X,
                _appWindow.Position.Y,
                isMaximized);

            SaveSettings();
        }

        private void SaveSettings()
        {
            var settings = new AppSettings
            {
                IncludeSubfolders = IncludeSubfoldersCheckBox.IsChecked ?? false,
                JpgQuality = (int)JpgQualitySlider.Value,
                ConflictResolution = (ConflictResolution)ConflictResolutionComboBox.SelectedIndex,
                OriginalFileHandling = OriginalFileHandlingComboBox.SelectedItem is FileHandlingOption opt ? opt.Value : OriginalFileHandling.Keep,
                CustomPath = CustomPathTextBox.Text
            };

            if (JpgButton.IsChecked == true) settings.Format = OutputFormat.Jpg;
            else if (PngButton.IsChecked == true) settings.Format = OutputFormat.Png;
            else if (GifButton.IsChecked == true) settings.Format = OutputFormat.Gif;
            else if (BmpButton.IsChecked == true) settings.Format = OutputFormat.Bmp;

            SettingsService.SaveAppSettings(settings);
        }

        private void JpgQualitySlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            if (JpgQualityText != null)
            {
                JpgQualityText.Text = $"{e.NewValue:0}%";
            }
        }

        private void FormatButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is ToggleButton clickedButton)
            {
                if (clickedButton.IsChecked == false)
                {
                    clickedButton.IsChecked = true;
                    return;
                }

                if (clickedButton != JpgButton) JpgButton.IsChecked = false;
                if (clickedButton != PngButton) PngButton.IsChecked = false;
                if (clickedButton != GifButton) GifButton.IsChecked = false;
                if (clickedButton != BmpButton) BmpButton.IsChecked = false;

                if (JpgQualityPanel != null)
                {
                    JpgQualityPanel.Visibility = (clickedButton == JpgButton) ? Visibility.Visible : Visibility.Collapsed;
                }

                OutputFormat newFormat = OutputFormat.Jpg;
                if (clickedButton == PngButton) newFormat = OutputFormat.Png;
                else if (clickedButton == GifButton) newFormat = OutputFormat.Gif;
                else if (clickedButton == BmpButton) newFormat = OutputFormat.Bmp;

                foreach (var file in Files)
                {
                    file.CurrentFormat = newFormat;
                }

                int processedCount = Files.Count(f => f.Status != FileStatus.Ready && f.Status != FileStatus.Pending && f.Status != FileStatus.Converting);
                UpdateFooter(processedCount);
            }
        }

        private void OriginalFileHandlingComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (OriginalFileHandlingComboBox.SelectedItem is not FileHandlingOption selectedOption) return;

            if (CustomPathGrid != null && selectedOption.Value == OriginalFileHandling.MoveTo)
            {
                CustomPathGrid.Visibility = Visibility.Visible;
            }
            else if (CustomPathGrid != null)
            {
                CustomPathGrid.Visibility = Visibility.Collapsed;
            }
        }

        private void IncludeSubfoldersCheckBox_CheckedChanged(object sender, RoutedEventArgs e)
        {
            if (SourceFolderTextBox != null)
            {
                ScanFolder(SourceFolderTextBox.Text);
            }
        }

        private async void BrowseCustomPathButton_Click(object sender, RoutedEventArgs e)
        {
            var folderPicker = new Windows.Storage.Pickers.FolderPicker();
            WinRT.Interop.InitializeWithWindow.Initialize(folderPicker, WinRT.Interop.WindowNative.GetWindowHandle(this));
            folderPicker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.PicturesLibrary;
            folderPicker.FileTypeFilter.Add("*");
            
            var folder = await folderPicker.PickSingleFolderAsync();
            if (folder != null)
            {
                CustomPathTextBox.Text = folder.Path;
            }
        }

        private async void BrowseSourceFolderButton_Click(object sender, RoutedEventArgs e)
        {
            var folderPicker = new Windows.Storage.Pickers.FolderPicker();
            WinRT.Interop.InitializeWithWindow.Initialize(folderPicker, WinRT.Interop.WindowNative.GetWindowHandle(this));
            folderPicker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.PicturesLibrary;
            folderPicker.FileTypeFilter.Add("*");

            var folder = await folderPicker.PickSingleFolderAsync();
            if (folder != null)
            {
                SourceFolderTextBox.Text = folder.Path;
                
                if (string.IsNullOrWhiteSpace(TargetFolderTextBox.Text))
                {
                    TargetFolderTextBox.Text = folder.Path;
                }

                ScanFolder(folder.Path);
            }
        }

        private void ResetTableButtonTopbar_Click(object sender, RoutedEventArgs e)
        {
            if (SourceFolderTextBox != null && !string.IsNullOrWhiteSpace(SourceFolderTextBox.Text))
            {
                ScanFolder(SourceFolderTextBox.Text);
            }
        }

        private void ScanFolder(string path)
        {
            Files.Clear();
            
            if (string.IsNullOrWhiteSpace(path) || !Directory.Exists(path))
            {
                UpdateStartConversionButtonStatus();
                return;
            }

            try
            {
                bool includeSubfolders = IncludeSubfoldersCheckBox.IsChecked ?? false;
                var searchOption = includeSubfolders ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
                
                var heicFiles = Directory.EnumerateFiles(path, "*.heic", searchOption);

                foreach (var file in heicFiles)
                {
                    var fileInfo = new FileInfo(file);
                    Files.Add(new FileItem
                    {
                        Path = fileInfo.DirectoryName ?? string.Empty,
                        OriginalName = fileInfo.Name,
                        ConvertedName = string.Empty,
                        Status = FileStatus.Ready
                    });
                }

                UpdateFooter(0);
            }
            catch (Exception ex)
            {
                // In a real app, show a message dialog
                System.Diagnostics.Debug.WriteLine($"Error scanning folder: {ex.Message}");
            }
            finally
            {
                UpdateStartConversionButtonStatus();
            }
        }

        private void UpdateFooter(int processed)
        {
            int total = Files.Count;
            
            FileCountTextBlock.Text = $"{processed} / {total} files";
            
            if (total > 0)
            {
                double percentage = (double)processed / total * 100;
                ConversionProgressBar.Value = percentage;
                PercentageTextBlock.Text = $"{(int)percentage}%";
            }
            else
            {
                ConversionProgressBar.Value = 0;
                PercentageTextBlock.Text = "0%";
            }

            // Update Stats Row
            int doneCount = Files.Count(f => f.Status == FileStatus.Completed || f.Status == FileStatus.Ignored || f.Status == FileStatus.Replaced);
            int failedCount = Files.Count(f => f.Status == FileStatus.Error);

            StatsDoneTextBlock.Text = $"{doneCount} done";
            StatsFailedTextBlock.Text = $"{failedCount} failed";
        }

        private async void BrowseTargetFolderButton_Click(object sender, RoutedEventArgs e)
        {
            var folderPicker = new Windows.Storage.Pickers.FolderPicker();
            WinRT.Interop.InitializeWithWindow.Initialize(folderPicker, WinRT.Interop.WindowNative.GetWindowHandle(this));
            folderPicker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.PicturesLibrary;
            folderPicker.FileTypeFilter.Add("*");

            var folder = await folderPicker.PickSingleFolderAsync();
            if (folder != null)
            {
                TargetFolderTextBox.Text = folder.Path;
            }
        }

        private async void StartConversionButton_Click(object sender, RoutedEventArgs e)
        {
            if (Files.Count == 0) return;

            StartConversionButton.IsEnabled = false;
            SetControlsEnabled(false);

            var targetFolder = TargetFolderTextBox.Text;
            var sourceBasePath = SourceFolderTextBox.Text;
            if (string.IsNullOrWhiteSpace(targetFolder))
            {
                targetFolder = sourceBasePath;
            }

            OutputFormat format = OutputFormat.Jpg;
            if (PngButton.IsChecked == true) format = OutputFormat.Png;
            else if (GifButton.IsChecked == true) format = OutputFormat.Gif;
            else if (BmpButton.IsChecked == true) format = OutputFormat.Bmp;

            var options = new ConversionOptions
            {
                ImageQuality = (int)JpgQualitySlider.Value,
                ConflictResolution = (ConflictResolution)ConflictResolutionComboBox.SelectedIndex,
                OriginalFileHandling = OriginalFileHandlingComboBox.SelectedItem is FileHandlingOption opt ? opt.Value : OriginalFileHandling.Keep,
                CustomMovePath = CustomPathTextBox.Text
            };

            List<FileItem> pendingFiles = new List<FileItem>();

            foreach (var file in Files)
            {
                if (file.Status == FileStatus.Pending
                    || file.Status == FileStatus.Error 
                    || file.Status == FileStatus.Ready
                    || (file.Status == FileStatus.Ignored && options.ConflictResolution != ConflictResolution.Ignore))
                {
                    file.Status = FileStatus.Pending;
                    pendingFiles.Add(file);
                }
            }

            if (pendingFiles.Count == 0)
            {
                SetControlsEnabled(true);
                StartConversionButton.IsEnabled = true;
                return;
            }

            int processedCount = Files.Count - pendingFiles.Count;

            try
            {
                foreach (var file in pendingFiles)
                {
                    file.Status = FileStatus.Converting;

                    string sourcePath = Path.Combine(file.Path, file.OriginalName);

                    try
                    {
                        // Run conversion in background to keep UI responsive
                        var result = await System.Threading.Tasks.Task.Run(async () => 
                            await FileService.ConvertAsync(sourcePath, sourceBasePath, targetFolder, format, options)
                        );

                        if (string.IsNullOrEmpty(result.ErrorMessage))
                        {
                            file.ErrorMessage = null;
                            file.ConvertedName = result.ConvertedFileName;
                            if (result.Ignored)
                            {
                                file.Status = FileStatus.Ignored;
                            }
                            else if (result.Replaced)
                            {
                                file.Status = FileStatus.Replaced;
                            }
                            else
                            {
                                file.Status = FileStatus.Completed;
                            }
                        }
                        else
                        {
                            file.ErrorMessage = result.ErrorMessage;
                            file.Status = FileStatus.Error;
                        }
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"Error converting file: {ex.Message}");
                        file.ErrorMessage = ex.Message;
                        file.Status = FileStatus.Error;
                    }

                    processedCount++;
                    UpdateFooter(processedCount);
                }
            }
            finally
            {
                SetControlsEnabled(true);
                StartConversionButton.IsEnabled = true;
            }

            // A "successful batch" = a run where more than 90% of processed files ended up
            // Completed, Ignored, or Replaced.
            int succeededCount = pendingFiles.Count(f =>
                f.Status == FileStatus.Completed
                || f.Status == FileStatus.Ignored
                || f.Status == FileStatus.Replaced);
            bool isSuccessfulBatch = (double)succeededCount / pendingFiles.Count > 0.9;
            if (isSuccessfulBatch)
            {
                _ratingService.RecordSuccessfulBatch();
                if (_ratingService.ShouldRequestReview())
                {
                    var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(this);
                    await _ratingService.TryRequestReviewAsync(this.Content.XamlRoot, hwnd);
                }
            }
        }

        private async void RateAppLink_Click(object sender, RoutedEventArgs e)
        {
            var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(this);
            bool rated = await new AppService().RequestRateAndReviewAsync(hwnd);
            if (rated)
            {
                SettingsService.SetHasRated();
            }
        }

        private async void ContactSupportLink_Click(object sender, RoutedEventArgs e)
        {
            await Windows.System.Launcher.LaunchUriAsync(new Uri("mailto:heicbatchconverter@gmail.com"));
        }

        private void UpdateStartConversionButtonStatus()
        {
            if (StartConversionButton != null)
            {
                StartConversionButton.IsEnabled = Files.Count > 0;
            }
        }

        private void SetControlsEnabled(bool isEnabled)
        {
            if (IncludeSubfoldersCheckBox != null) IncludeSubfoldersCheckBox.IsEnabled = isEnabled;
            if (JpgButton != null) JpgButton.IsEnabled = isEnabled;
            if (PngButton != null) PngButton.IsEnabled = isEnabled;
            if (GifButton != null) GifButton.IsEnabled = isEnabled;
            if (BmpButton != null) BmpButton.IsEnabled = isEnabled;
            if (JpgQualitySlider != null) JpgQualitySlider.IsEnabled = isEnabled;
            if (ConflictResolutionComboBox != null) ConflictResolutionComboBox.IsEnabled = isEnabled;
            if (OriginalFileHandlingComboBox != null) OriginalFileHandlingComboBox.IsEnabled = isEnabled;
            
            if (BrowseCustomPathButton != null) BrowseCustomPathButton.IsEnabled = isEnabled;
            if (BrowseSourceFolderButton != null) BrowseSourceFolderButton.IsEnabled = isEnabled;
            if (BrowseTargetFolderButton != null) BrowseTargetFolderButton.IsEnabled = isEnabled;
        }
    }
}
