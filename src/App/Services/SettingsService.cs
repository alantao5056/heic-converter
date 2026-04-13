using System;
using Windows.Graphics;
using Windows.Storage;
using Alan.HeicConverter.Models;

namespace Alan.HeicConverter.Services
{
    public static class SettingsService
    {
        private const string WINDOW_WIDTH_KEY = "WindowWidth";
        private const string WINDOW_HEIGHT_KEY = "WindowHeight";
        private const string WINDOW_X_KEY = "WindowX";
        private const string WINDOW_Y_KEY = "WindowY";
        private const string WINDOW_MAXIMIZED_KEY = "WindowMaximized";

        private const string SETTINGS_INCLUDE_SUBFOLDERS = "Settings_IncludeSubfolders";
        private const string SETTINGS_FORMAT = "Settings_Format";
        private const string SETTINGS_JPG_QUALITY = "Settings_JpgQuality";
        private const string SETTINGS_CONFLICT_RESOLUTION = "Settings_ConflictResolution";
        private const string SETTINGS_ORIGINAL_FILE_HANDLING = "Settings_OriginalFileHandling";
        private const string SETTINGS_CUSTOM_PATH = "Settings_CustomPath";

        public static void SaveWindowState(int width, int height, int x, int y, bool isMaximized)
        {
            try
            {
                var localSettings = ApplicationData.Current.LocalSettings;
                localSettings.Values[WINDOW_MAXIMIZED_KEY] = isMaximized;

                if (!isMaximized)
                {
                    localSettings.Values[WINDOW_WIDTH_KEY] = width;
                    localSettings.Values[WINDOW_HEIGHT_KEY] = height;
                    localSettings.Values[WINDOW_X_KEY] = x;
                    localSettings.Values[WINDOW_Y_KEY] = y;
                }
            }
            catch (Exception)
            {
                // Ignored
            }
        }

        public static SizeInt32? GetWindowSize()
        {
            try
            {
                var localSettings = ApplicationData.Current.LocalSettings;
                if (localSettings.Values.TryGetValue(WINDOW_WIDTH_KEY, out object? widthObj) &&
                    localSettings.Values.TryGetValue(WINDOW_HEIGHT_KEY, out object? heightObj))
                {
                    return new SizeInt32((int)widthObj!, (int)heightObj!);
                }
            }
            catch (Exception)
            {
                // Ignored
            }
            return null;
        }

        public static PointInt32? GetWindowPosition()
        {
            try
            {
                var localSettings = ApplicationData.Current.LocalSettings;
                if (localSettings.Values.TryGetValue(WINDOW_X_KEY, out object? xObj) &&
                    localSettings.Values.TryGetValue(WINDOW_Y_KEY, out object? yObj))
                {
                    return new PointInt32((int)xObj!, (int)yObj!);
                }
            }
            catch (Exception)
            {
                // Ignored
            }
            return null;
        }

        public static bool GetIsMaximized()
        {
            try
            {
                var localSettings = ApplicationData.Current.LocalSettings;
                if (localSettings.Values.TryGetValue(WINDOW_MAXIMIZED_KEY, out object? isMaximizedObj))
                {
                    return (bool)isMaximizedObj!;
                }
            }
            catch (Exception)
            {
                // Ignored
            }
            return false;
        }

        public static void SaveAppSettings(AppSettings settings)
        {
            try
            {
                var localSettings = ApplicationData.Current.LocalSettings;
                localSettings.Values[SETTINGS_INCLUDE_SUBFOLDERS] = settings.IncludeSubfolders;
                localSettings.Values[SETTINGS_FORMAT] = (int)settings.Format;
                localSettings.Values[SETTINGS_JPG_QUALITY] = settings.JpgQuality;
                localSettings.Values[SETTINGS_CONFLICT_RESOLUTION] = (int)settings.ConflictResolution;
                localSettings.Values[SETTINGS_ORIGINAL_FILE_HANDLING] = (int)settings.OriginalFileHandling;
                localSettings.Values[SETTINGS_CUSTOM_PATH] = settings.CustomPath;
            }
            catch (Exception)
            {
                // Ignored
            }
        }

        public static AppSettings LoadAppSettings()
        {
            var settings = new AppSettings();
            try
            {
                var localSettings = ApplicationData.Current.LocalSettings;

                if (localSettings.Values.TryGetValue(SETTINGS_INCLUDE_SUBFOLDERS, out object? includeSubfolders))
                    settings.IncludeSubfolders = (bool)includeSubfolders!;

                if (localSettings.Values.TryGetValue(SETTINGS_FORMAT, out object? format))
                    settings.Format = (OutputFormat)(int)format!;

                if (localSettings.Values.TryGetValue(SETTINGS_JPG_QUALITY, out object? jpgQuality))
                    settings.JpgQuality = (int)jpgQuality!;

                if (localSettings.Values.TryGetValue(SETTINGS_CONFLICT_RESOLUTION, out object? conflictResolution))
                    settings.ConflictResolution = (ConflictResolution)(int)conflictResolution!;

                if (localSettings.Values.TryGetValue(SETTINGS_ORIGINAL_FILE_HANDLING, out object? originalFileHandling))
                    settings.OriginalFileHandling = (OriginalFileHandling)(int)originalFileHandling!;

                if (localSettings.Values.TryGetValue(SETTINGS_CUSTOM_PATH, out object? customPath))
                    settings.CustomPath = (string)customPath!;
            }
            catch (Exception)
            {
                // Ignored, return defaults
            }
            return settings;
        }
    }
}