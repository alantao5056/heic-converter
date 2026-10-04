using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation.Collections;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.FileProperties;
using Windows.Storage.Search;

namespace HeicToJpgBatchConverter
{
    internal class Converter
    {
        private Collection<FileListItem> _files;
        private ConvertOptions _options;
        private HashSet<string> _existingRelativePaths = new HashSet<string>() { "." };
        private StorageFolder _targetFolder;
        private string _sourceFolderPath;
        private BitmapPropertySet _bitmapPropertySet;
        private Guid _encoderGuid;
        private string _extension;
        private static readonly Dictionary<ImageFormat, Guid> _formatToEncodeGuidMapper = new Dictionary<ImageFormat, Guid>()
        {
            { ImageFormat.JPG, BitmapEncoder.JpegEncoderId },
            { ImageFormat.PNG, BitmapEncoder.PngEncoderId },
            { ImageFormat.GIF, BitmapEncoder.GifEncoderId },
            { ImageFormat.BMP, BitmapEncoder.BmpEncoderId },
            { ImageFormat.TIFF, BitmapEncoder.TiffEncoderId }
        };

        internal Converter(Collection<FileListItem> files, string sourceFolderPath, StorageFolder targetFolder, ConvertOptions options)
        {
            this._files = files;
            this._options = options;
            this._targetFolder = targetFolder;
            this._sourceFolderPath = sourceFolderPath;
            if (_options.Format == ImageFormat.JPG)
            {
                _bitmapPropertySet = new BitmapPropertySet();
                BitmapTypedValue qualityValue = new BitmapTypedValue(Math.Round(options.ImageQuality, 2), Windows.Foundation.PropertyType.Single);
                _bitmapPropertySet.Add("ImageQuality", qualityValue);
            }
            this._encoderGuid = _formatToEncodeGuidMapper[options.Format];
            this._extension = "." + options.Format.ToString();
        }

        internal async Task<ConvertSummary> ConvertAsync()
        {
            ConvertSummary convertSummary = new ConvertSummary();
            convertSummary.Total = (uint)_files.Count;

            int i = 1;
            foreach (FileListItem file in _files)
            {
                await _ConvertFile(file);
                switch (file.Status)
                {
                    case Status.Success:
                        convertSummary.Success++;
                        break;
                    case Status.Fail:
                        convertSummary.Fail++;
                        break;
                    case Status.Ignore:
                        convertSummary.Ignore++;
                        break;
                    default:
                        break;
                }
                i++;
            }
            return convertSummary;
        }

        internal async Task _ConvertFile(FileListItem file)
        {
            file.Status = Status.InProgress;
            try
            {
                using (var inputStream = await file.OriginalStorageFile.OpenReadAsync())
                {
                    BitmapDecoder decoder = await BitmapDecoder.CreateAsync(inputStream);
                    SoftwareBitmap softwareBitmap = await decoder.GetSoftwareBitmapAsync();
                    if (softwareBitmap.BitmapPixelFormat != BitmapPixelFormat.Bgra8 ||
                        softwareBitmap.BitmapAlphaMode == BitmapAlphaMode.Straight)
                    {
                        softwareBitmap = SoftwareBitmap.Convert(softwareBitmap, BitmapPixelFormat.Bgra8, BitmapAlphaMode.Premultiplied);
                    }

                    // Prepair target folder
                    StorageFolder targetFolder = await _GetFinalTargetFolder(file.FullPath);
                    string targetFileName = Path.GetFileNameWithoutExtension(file.OriginalName) + _extension;
                    StorageFile targetFile = await _GenerateTargetFile(targetFolder, targetFileName);
                    if (targetFile == null)
                    {
                        file.ConvertedName = string.Empty;
                        file.Status = Status.Ignore;
                        return;
                    }

                    // Convert image properties
                    var imageProperties = await file.OriginalStorageFile.Properties.GetImagePropertiesAsync();
                    var imagePropertiesConvertResult = ImagePropertiesToBitmapPropertySet.Convert(imageProperties);
                    BitmapPropertySet bitmapPropertySet = imagePropertiesConvertResult.PropertySet;

                    using (var stream = await targetFile.OpenAsync(FileAccessMode.ReadWrite))
                    {
                        BitmapEncoder encoder = _bitmapPropertySet != null
                            ? await BitmapEncoder.CreateAsync(_encoderGuid, stream, _bitmapPropertySet)
                            : await BitmapEncoder.CreateAsync(_encoderGuid, stream);

                        
                        encoder.SetSoftwareBitmap(softwareBitmap);                        
                        await encoder.BitmapProperties.SetPropertiesAsync(bitmapPropertySet);
                        await encoder.FlushAsync();
                    }

                    file.ConvertedName = targetFile.Name;
                    file.Status = Status.Success;
                }
            }
            catch (Exception ex)
            {
                file.ConvertedName = string.Empty;
                file.Status = Status.Fail;
            }
        }

        private async Task<StorageFile> _GenerateTargetFile(StorageFolder targetFolder, string targetFileName)
        {
            switch (_options.CollisionResolution)
            {
                case CollisionResolution.GenerateUniqueName:
                    return await targetFolder.CreateFileAsync(targetFileName, CreationCollisionOption.GenerateUniqueName);
                case CollisionResolution.Replace:
                    return await targetFolder.CreateFileAsync(targetFileName, CreationCollisionOption.ReplaceExisting);
                case CollisionResolution.Ignore:
                    try
                    {
                        return await targetFolder.CreateFileAsync(targetFileName, CreationCollisionOption.FailIfExists);
                    }
                    catch (Exception)
                    {
                        return null;
                    }
                default:
                    return null;
            }
        }

        private async Task<StorageFolder> _GetFinalTargetFolder(string sourceFileFullPath)
        {
            StorageFolder finalTargetFolder;
            string relativePath = Path.GetRelativePath(_sourceFolderPath, Path.GetDirectoryName(sourceFileFullPath));
            if (!_existingRelativePaths.Contains(relativePath))
            {
                // First time dealing with this relative path, need to find and create if not exists.
                string[] subfolders = relativePath.Split('/');
                finalTargetFolder = await _GetOrCreateTargetSubfolder(subfolders);
                _existingRelativePaths.Add(relativePath);
            }
            else
            {
                if (relativePath == ".")
                {
                    // root file, return root target folder directly.
                    finalTargetFolder = _targetFolder;
                } else
                {
                    // Assuming the relative path exists. And get it directly.
                    finalTargetFolder = await _targetFolder.GetFolderAsync(relativePath);
                }
            }
            return finalTargetFolder;
        }

        private async Task<StorageFolder> _GetOrCreateTargetSubfolder(string[] subfolderNames)
        {
            StorageFolder currentFolder = _targetFolder;
            foreach (string subfolderName in subfolderNames)
            {
                currentFolder = await currentFolder.CreateFolderAsync(subfolderName, CreationCollisionOption.OpenIfExists);
            }
            return currentFolder;
        }
    }
}
