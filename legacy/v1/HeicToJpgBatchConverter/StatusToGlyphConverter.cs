using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace HeicToJpgBatchConverter
{
    public class StatusToGlyphConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value != null)
            {
                Status status = (Status)value;
                switch (status)
                {
                    case Status.InProgress:
                        return "\uF16A";
                    case Status.Success:
                        return "\uEC61";
                    case Status.Fail:
                        return "\uEB90";
                    case Status.Ignore:
                        return "\uE814";
                    default:
                        break;
                }
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new Exception("Internal Error.");
        }
    }
}
