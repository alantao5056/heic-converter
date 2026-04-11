using Microsoft.UI.Xaml;
using System;

namespace Alan.HeicConverter.Models
{
    public static class FileStatusExtensions
    {
        public static string GetStatusText(this FileStatus status) => status switch
        {
            FileStatus.Ready => "Ready",
            FileStatus.Pending => "Pending",
            FileStatus.Converting => "Converting...",
            FileStatus.Completed => "Completed",
            FileStatus.Error => "Error",
            _ => "Unknown"
        };

        public static string GetStatusGlyph(this FileStatus status) => status switch
        {
            FileStatus.Ready => "\uE768",      // Play button
            FileStatus.Pending => "\uE916",    // History/Timer
            FileStatus.Completed => "\uE73E",  // Checkmark
            FileStatus.Error => "\uE711",      // Error/X
            _ => string.Empty
        };

        public static string GetBackgroundColorResourceKey(this FileStatus status) => status switch
        {
            FileStatus.Completed => "GreenBgColor",
            FileStatus.Error => "RedBgColor",
            FileStatus.Pending => "AmberBgColor",
            _ => "AccentLightColor"
        };

        public static string GetForegroundColorResourceKey(this FileStatus status) => status switch
        {
            FileStatus.Completed => "GreenTextColor",
            FileStatus.Error => "RedTextColor",
            FileStatus.Pending => "AmberTextColor",
            _ => "AccentColor"
        };
    }
}