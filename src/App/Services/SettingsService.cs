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

        private const string RATING_SUCCESSFUL_BATCH_COUNT = "Rating_SuccessfulBatchCount";
        private const string RATING_HAS_RATED = "Rating_HasRated";
        private const string RATING_OPTED_OUT = "Rating_OptedOut";
        private const string RATING_PROMPT_COUNT = "Rating_PromptCount";
        private const string RATING_LAST_PROMPT_UTC = "Rating_LastPromptUtc";

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

        public static RatingState LoadRatingState()
        {
            var state = new RatingState();
            try
            {
                var localSettings = ApplicationData.Current.LocalSettings;

                if (localSettings.Values.TryGetValue(RATING_SUCCESSFUL_BATCH_COUNT, out object? batchCount))
                    state.SuccessfulBatchCount = (int)batchCount!;

                if (localSettings.Values.TryGetValue(RATING_HAS_RATED, out object? hasRated))
                    state.HasRated = (bool)hasRated!;

                if (localSettings.Values.TryGetValue(RATING_OPTED_OUT, out object? optedOut))
                    state.OptedOut = (bool)optedOut!;

                if (localSettings.Values.TryGetValue(RATING_PROMPT_COUNT, out object? promptCount))
                    state.PromptCount = (int)promptCount!;

                if (localSettings.Values.TryGetValue(RATING_LAST_PROMPT_UTC, out object? lastPromptUtc)
                    && DateTimeOffset.TryParse((string)lastPromptUtc!, null,
                        System.Globalization.DateTimeStyles.RoundtripKind, out var parsed))
                {
                    state.LastPromptUtc = parsed;
                }
            }
            catch (Exception)
            {
                // Ignored, return defaults
            }
            return state;
        }

        public static void IncrementSuccessfulBatchCount()
        {
            try
            {
                var localSettings = ApplicationData.Current.LocalSettings;
                int current = localSettings.Values.TryGetValue(RATING_SUCCESSFUL_BATCH_COUNT, out object? value)
                    ? (int)value!
                    : 0;
                localSettings.Values[RATING_SUCCESSFUL_BATCH_COUNT] = current + 1;
            }
            catch (Exception)
            {
                // Ignored
            }
        }

        public static void SetHasRated()
        {
            try
            {
                ApplicationData.Current.LocalSettings.Values[RATING_HAS_RATED] = true;
            }
            catch (Exception)
            {
                // Ignored
            }
        }

        public static void SetOptedOut()
        {
            try
            {
                ApplicationData.Current.LocalSettings.Values[RATING_OPTED_OUT] = true;
            }
            catch (Exception)
            {
                // Ignored
            }
        }

        public static void RecordPromptShown(DateTimeOffset whenUtc)
        {
            try
            {
                var localSettings = ApplicationData.Current.LocalSettings;
                int current = localSettings.Values.TryGetValue(RATING_PROMPT_COUNT, out object? value)
                    ? (int)value!
                    : 0;
                localSettings.Values[RATING_PROMPT_COUNT] = current + 1;
                localSettings.Values[RATING_LAST_PROMPT_UTC] = whenUtc.ToString("o");
            }
            catch (Exception)
            {
                // Ignored
            }
        }
    }
}