using System;
using System.Collections.Generic;
using System.Linq;
using Windows.Foundation.Collections;
using Windows.Graphics.Imaging;
using Windows.Storage.FileProperties;

namespace HeicToJpgBatchConverter
{
    /// <summary>
    /// Convert Windows.Storage.FileProperties.ImageProperties to BitmapPropertySet
    /// for BitmapEncoder.BitmapProperties.SetPropertiesAsync(...)
    /// </summary>
    public static class ImagePropertiesToBitmapPropertySet
    {
        public sealed class ConvertResult
        {
            public BitmapPropertySet PropertySet { get; } = new BitmapPropertySet();
            public IReadOnlyList<string> Errors => _errors;
            internal List<string> _errors { get; } = new List<string>();
            public int Count => PropertySet.Count;
        }

        /// <summary>
        /// Convert ImageProperties into a BitmapPropertySet with best-effort / fault-tolerant behavior.
        /// </summary>
        /// <param name="src">Source ImageProperties (already loaded from file).</param>
        /// <param name="includeGps">Whether to include GPS fields if available.</param>
        /// <param name="preferStringArrayForKeywords">
        /// True: write keywords as string[] (preferred). False: write as semicolon-delimited string.
        /// </param>
        public static ConvertResult Convert(
            ImageProperties src,
            bool includeGps = true,
            bool preferStringArrayForKeywords = true)
        {
            var result = new ConvertResult();
            if (src == null) return result;

            // ---- Title ----
            // System.Title accepts string. :contentReference[oaicite:2]{index=2}
            AddString(result, "System.Title", src.Title);

            // ---- Rating ----
            // System.Rating range 0..99; ImageProperties.Rating maps to System.Rating. :contentReference[oaicite:3]{index=3}
            if (src.Rating > 0 && src.Rating <= 99)
            {
                AddUInt32(result, "System.Rating", src.Rating);
            }

            // ---- Date taken ----
            // System.Photo.DateTaken is the canonical Windows property for EXIF DateTimeOriginal. :contentReference[oaicite:4]{index=4}
            if (src.DateTaken != default)
            {
                AddDateTime(result, "System.Photo.DateTaken", src.DateTaken);
            }

            // ---- Camera manufacturer & model ----
            // These exist on ImageProperties and map to System.Photo.* keys. :contentReference[oaicite:5]{index=5}
            AddString(result, "System.Photo.CameraManufacturer", src.CameraManufacturer);
            AddString(result, "System.Photo.CameraModel", src.CameraModel);

            // ---- Orientation (EXIF) ----
            // Policy indicates input type is UShort. :contentReference[oaicite:6]{index=6}
            if (TryMapOrientationToExifUShort(src.Orientation, out var exifOrientation))
            {
                AddUInt16(result, "System.Photo.Orientation", exifOrientation);
            }

            // ---- PeopleNames (tagged people) ----
            // PeopleNames policy exists for JPEG/TIFF. :contentReference[oaicite:7]{index=7}
            var people = SafeToStringArray(src.PeopleNames);
            if (people.Length > 0)
            {
                AddStringArray(result, "System.Photo.PeopleNames", people);
            }

            // ---- Keywords / Tags ----
            // System.Keywords is writable; prefers VT_VECTOR|VT_LPWSTR or accepts semicolon string. :contentReference[oaicite:8]{index=8}
            var keywords = SafeToStringArray(src.Keywords);
            if (keywords.Length > 0)
            {
                if (preferStringArrayForKeywords)
                {
                    AddStringArray(result, "System.Keywords", keywords);
                }
                else
                {
                    // Windows UI often shows tags separated by ';'
                    AddString(result, "System.Keywords", string.Join("; ", keywords));
                }
            }

            // ---- GPS ----
            // System.GPS.Latitude/Longitude cannot be written directly; must write Numerator/Denominator/Ref. :contentReference[oaicite:9]{index=9}
            if (includeGps && src.Latitude.HasValue && src.Longitude.HasValue)
            {
                AddGps(result, src.Latitude.Value, src.Longitude.Value);
            }

            return result;
        }

        #region Helpers (fault tolerant)

        private static void AddString(ConvertResult r, string key, string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return;
            TryAdd(r, key, new BitmapTypedValue(value.Trim(), Windows.Foundation.PropertyType.String));
        }

        private static void AddDateTime(ConvertResult r, string key, DateTimeOffset dto)
        {
            // Use DateTime type; the platform will store it as appropriate (FILETIME) for the property policy.
            TryAdd(r, key, new BitmapTypedValue(dto, Windows.Foundation.PropertyType.DateTime));
        }

        private static void AddUInt32(ConvertResult r, string key, uint value)
        {
            TryAdd(r, key, new BitmapTypedValue(value, Windows.Foundation.PropertyType.UInt32));
        }

        private static void AddUInt16(ConvertResult r, string key, ushort value)
        {
            TryAdd(r, key, new BitmapTypedValue(value, Windows.Foundation.PropertyType.UInt16));
        }

        private static void AddStringArray(ConvertResult r, string key, string[] arr)
        {
            if (arr == null || arr.Length == 0) return;
            TryAdd(r, key, new BitmapTypedValue(arr, Windows.Foundation.PropertyType.StringArray));
        }

        private static void TryAdd(ConvertResult r, string key, BitmapTypedValue v)
        {
            try
            {
                // Avoid duplicate key exceptions
                if (r.PropertySet.ContainsKey(key))
                    r.PropertySet[key] = v;
                else
                    r.PropertySet.Add(key, v);
            }
            catch (Exception ex)
            {
                r._errors.Add($"Add failed: {key} ({v?.Type}) - {ex.GetType().Name}: {ex.Message}");
            }
        }

        private static string[] SafeToStringArray(IEnumerable<string> list)
        {
            if (list == null) return Array.Empty<string>();
            return list.Where(s => !string.IsNullOrWhiteSpace(s))
                       .Select(s => s.Trim())
                       .Distinct(StringComparer.OrdinalIgnoreCase)
                       .ToArray();
        }

        #endregion

        #region GPS (decimal degrees -> DMS rational arrays)

        private static void AddGps(ConvertResult r, double latitude, double longitude)
        {
            try
            {
                var latRef = latitude >= 0 ? "N" : "S";
                var lonRef = longitude >= 0 ? "E" : "W";

                var (latNum, latDen) = ToDmsRationals(Math.Abs(latitude));
                var (lonNum, lonDen) = ToDmsRationals(Math.Abs(longitude));

                AddString(r, "System.GPS.LatitudeRef", latRef);
                AddString(r, "System.GPS.LongitudeRef", lonRef);

                TryAdd(r, "System.GPS.LatitudeNumerator",
                    new BitmapTypedValue(latNum, Windows.Foundation.PropertyType.UInt32Array));
                TryAdd(r, "System.GPS.LatitudeDenominator",
                    new BitmapTypedValue(latDen, Windows.Foundation.PropertyType.UInt32Array));

                TryAdd(r, "System.GPS.LongitudeNumerator",
                    new BitmapTypedValue(lonNum, Windows.Foundation.PropertyType.UInt32Array));
                TryAdd(r, "System.GPS.LongitudeDenominator",
                    new BitmapTypedValue(lonDen, Windows.Foundation.PropertyType.UInt32Array));
            }
            catch (Exception ex)
            {
                r._errors.Add($"GPS failed: {ex.GetType().Name}: {ex.Message}");
            }
        }

        private static (uint[] num, uint[] den) ToDmsRationals(double decimalDegrees)
        {
            // Convert decimal degrees to D/M/S:
            // degrees = floor(x)
            // minutes = floor((x - deg)*60)
            // seconds = ((x - deg)*60 - min)*60
            var deg = (uint)Math.Floor(decimalDegrees);
            var minutesFull = (decimalDegrees - deg) * 60.0;
            var min = (uint)Math.Floor(minutesFull);
            var sec = (minutesFull - min) * 60.0;

            // Store seconds as rational for precision: secNum/secDen
            const uint secDen = 10000;
            var secNum = (uint)Math.Round(sec * secDen);

            return (
                num: new[] { deg, min, secNum },
                den: new[] { 1u, 1u, secDen }
            );
        }

        private static bool TryMapOrientationToExifUShort(PhotoOrientation ori, out ushort exif)
        {
            // EXIF orientation:
            // 1 = Normal
            // 2 = Flip horizontal
            // 3 = Rotate 180
            // 4 = Flip vertical
            // 5 = Transpose (flip horizontal + rotate 270 CW)
            // 6 = Rotate 90 CW
            // 7 = Transverse (flip horizontal + rotate 90 CW)
            // 8 = Rotate 270 CW (or 90 CCW)

            switch (ori)
            {
                case PhotoOrientation.Normal: exif = 1; return true;
                case PhotoOrientation.FlipHorizontal: exif = 2; return true;
                case PhotoOrientation.Rotate180: exif = 3; return true;
                case PhotoOrientation.FlipVertical: exif = 4; return true;
                case PhotoOrientation.Transpose: exif = 5; return true;
                case PhotoOrientation.Rotate90: exif = 6; return true;
                case PhotoOrientation.Transverse: exif = 7; return true;
                case PhotoOrientation.Rotate270: exif = 8; return true;

                // PhotoOrientation.Unspecified
                default:
                    exif = 0;
                    return false;
            }
        }


        #endregion
    }
}

