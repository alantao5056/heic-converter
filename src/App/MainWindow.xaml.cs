using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Alan.HeicConverter.Services;

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

        public MainWindow()
        {
            InitializeComponent();

            var hWnd = WinRT.Interop.WindowNative.GetWindowHandle(this);
            var windowId = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(hWnd);
            _appWindow = Microsoft.UI.Windowing.AppWindow.GetFromWindowId(windowId);

            RestoreWindowState();

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
    }
}
