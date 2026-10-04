using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace HeicToJpgBatchConverter
{
    public class StatusToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value != null)
            {
                Status status = (Status) value;
                switch (status)
                {
                    case Status.InProgress:
                        return "#3379D9";
                    case Status.Success:
                        return "Green";
                    case Status.Fail:
                        return "Red";
                    case Status.Ignore:
                        return "Orange";
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
