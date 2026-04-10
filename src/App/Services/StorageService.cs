using System;
using Windows.Graphics;
using Windows.Storage;

namespace Alan.HeicConverter.Services
{
    public static class StorageService
    {
        private const string WINDOW_WIDTH_KEY = "WindowWidth";
        private const string WINDOW_HEIGHT_KEY = "WindowHeight";
        private const string WINDOW_X_KEY = "WindowX";
        private const string WINDOW_Y_KEY = "WindowY";
        private const string WINDOW_MAXIMIZED_KEY = "WindowMaximized";

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
    }
}