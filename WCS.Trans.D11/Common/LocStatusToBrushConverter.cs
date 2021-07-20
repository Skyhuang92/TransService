using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;
using WCS.MyControl;

namespace WCS.Trans
{
    public class LocStatusToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int input = System.Convert.ToInt32(value);
            switch (input)
            {
                case 0:
                    return CustomSolidBrush.LightGray;
                case 1:
                    return CustomSolidBrush.LimeGreen;
                case 2:
                    return CustomSolidBrush.Red;
                case 3:
                    return Brushes.Yellow;
                default:
                    return CustomSolidBrush.LightGray;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
