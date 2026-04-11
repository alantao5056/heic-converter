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
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern uint GetDpiForWindow(IntPtr hwnd);

        private Microsoft.UI.Windowing.AppWindow _appWindow;

        public ObservableCollection<FileItem> Files { get; } = new();

        public MainWindow()
        {
            InitializeComponent();

            var hWnd = WinRT.Interop.WindowNative.GetWindowHandle(this);
            var windowId = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(hWnd);
            _appWindow = Microsoft.UI.Windowing.AppWindow.GetFromWindowId(windowId);

            FileListView.ItemsSource = Files;

            RestoreWindowState();
            LoadSettings();

            this.Closed += MainWindow_Closed;
        }

        private void RestoreWindowState()
        {
            var savedSize = StorageService.GetWindowSize();
            if (savedSize.HasValue)
            {
                _appWindow.Resize(savedSize.Value);
            }
            else
            {
                SetDefaultSize();
            }

            var savedPosition = StorageService.GetWindowPosition();
            if (savedPosition.HasValue)
            {
                _appWindow.Move(savedPosition.Value);
            }

            if (StorageService.GetIsMaximized())
            {
                if (_appWindow.Presenter is Microsoft.UI.Windowing.OverlappedPresenter presenter)
                {
                    presenter.Maximize();
                }
            }
        }

        private void LoadSettings()
        {
            var settings = StorageService.LoadAppSettings();

            IncludeSubfoldersCheckBox.IsChecked = settings.IncludeSubfolders;

            JpgButton.IsChecked = settings.Format == OutputFormat.Jpg;
            PngButton.IsChecked = settings.Format == OutputFormat.Png;
            GifButton.IsChecked = settings.Format == OutputFormat.Gif;
            BmpButton.IsChecked = settings.Format == OutputFormat.Bmp;

            JpgQualitySlider.Value = settings.JpgQuality;
            JpgQualityText.Text = $"{settings.JpgQuality}%";
            JpgQualityPanel.Visibility = (settings.Format == OutputFormat.Jpg) ? Visibility.Visible : Visibility.Collapsed;

            ConflictResolutionComboBox.SelectedIndex = (int)settings.ConflictResolution;
            OriginalFileHandlingComboBox.SelectedIndex = (int)settings.OriginalFileHandling;
            CustomPathTextBox.Text = settings.CustomPath;

            SourceFolderTextBox.Text = string.Empty;
            TargetFolderTextBox.Text = string.Empty;
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

            StorageService.SaveWindowState(
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
                OriginalFileHandling = (OriginalFileHandling)OriginalFileHandlingComboBox.SelectedIndex,
                CustomPath = CustomPathTextBox.Text
            };

            if (JpgButton.IsChecked == true) settings.Format = OutputFormat.Jpg;
            else if (PngButton.IsChecked == true) settings.Format = OutputFormat.Png;
            else if (GifButton.IsChecked == true) settings.Format = OutputFormat.Gif;
            else if (BmpButton.IsChecked == true) settings.Format = OutputFormat.Bmp;

            StorageService.SaveAppSettings(settings);
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
            }
        }

        private void OriginalFileHandlingComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CustomPathGrid != null && OriginalFileHandlingComboBox.SelectedIndex == 2)
            {
                CustomPathGrid.Visibility = Visibility.Visible;
            }
            else if (CustomPathGrid != null)
            {
                CustomPathGrid.Visibility = Visibility.Collapsed;
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

        private void ScanFolder(string path)
        {
            Files.Clear();
            
            if (string.IsNullOrWhiteSpace(path) || !Directory.Exists(path))
            {
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

                UpdateFooter();
            }
            catch (Exception ex)
            {
                // In a real app, show a message dialog
                System.Diagnostics.Debug.WriteLine($"Error scanning folder: {ex.Message}");
            }
        }

        private void UpdateFooter()
        {
            int total = Files.Count;
            int completed = Files.Count(f => f.Status == FileStatus.Completed);
            
            FileCountTextBlock.Text = $"{completed} / {total} files";
            
            if (total > 0)
            {
                double percentage = (double)completed / total * 100;
                ConversionProgressBar.Value = percentage;
                PercentageTextBlock.Text = $"{(int)percentage}%";
            }
            else
            {
                ConversionProgressBar.Value = 0;
                PercentageTextBlock.Text = "0%";
            }
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
    }
}
