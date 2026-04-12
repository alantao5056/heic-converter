using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Alan.HeicConverter.Models;

namespace Alan.HeicConverter.Controls
{
    public sealed partial class FileStatusBadge : UserControl
    {
        public FileStatusBadge()
        {
            this.InitializeComponent();
            UpdateUI();
        }

        public static readonly DependencyProperty StatusProperty =
            DependencyProperty.Register(
                nameof(Status),
                typeof(FileStatus),
                typeof(FileStatusBadge),
                new PropertyMetadata(FileStatus.Ready, OnStatusChanged));

        public FileStatus Status
        {
            get => (FileStatus)GetValue(StatusProperty);
            set => SetValue(StatusProperty, value);
        }

        public static readonly DependencyProperty ErrorMessageProperty =
            DependencyProperty.Register(
                nameof(ErrorMessage),
                typeof(string),
                typeof(FileStatusBadge),
                new PropertyMetadata(null, OnErrorMessageChanged));

        public string? ErrorMessage
        {
            get => (string?)GetValue(ErrorMessageProperty);
            set => SetValue(ErrorMessageProperty, value);
        }

        private static void OnStatusChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is FileStatusBadge control)
            {
                control.UpdateUI();
            }
        }

        private static void OnErrorMessageChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is FileStatusBadge control)
            {
                control.UpdateUI();
            }
        }

        private void UpdateUI()
        {
            // Set Text
            StatusTextBlock.Text = Status.GetStatusText();

            // Handle Tooltip for Errors
            if (Status == FileStatus.Error && !string.IsNullOrEmpty(ErrorMessage))
            {
                ToolTipService.SetToolTip(RootBorder, ErrorMessage);
            }
            else
            {
                ToolTipService.SetToolTip(RootBorder, null);
            }

            // Set Visibility and Glyph
            if (Status == FileStatus.Converting)
            {
                StatusProgressRing.Visibility = Visibility.Visible;
                StatusProgressRing.IsActive = true;
                StatusIcon.Visibility = Visibility.Collapsed;
            }
            else
            {
                StatusProgressRing.Visibility = Visibility.Collapsed;
                StatusProgressRing.IsActive = false;
                StatusIcon.Visibility = Visibility.Visible;
                StatusIcon.Glyph = Status.GetStatusGlyph();
            }

            // Set Colors
            var bgKey = Status.GetBackgroundColorResourceKey();
            var fgKey = Status.GetForegroundColorResourceKey();

            if (Application.Current.Resources.TryGetValue(bgKey, out object bgObj) && bgObj is Brush bgBrush)
            {
                RootBorder.Background = bgBrush;
            }
            
            if (Application.Current.Resources.TryGetValue(fgKey, out object fgObj) && fgObj is Brush fgBrush)
            {
                StatusProgressRing.Foreground = fgBrush;
                StatusIcon.Foreground = fgBrush;
                StatusTextBlock.Foreground = fgBrush;
            }
        }
    }
}