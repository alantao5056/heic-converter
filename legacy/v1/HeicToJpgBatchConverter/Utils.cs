using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;

namespace HeicToJpgBatchConverter
{
    internal static class Utils
    {
        internal static bool IsHeifSupported()
        {
            try
            {
                var decoders = BitmapDecoder.GetDecoderInformationEnumerator();
                return decoders.Any(d => d.CodecId == BitmapDecoder.HeifDecoderId);
            }
            catch
            {
                return true;
            }
        }
    }
}
