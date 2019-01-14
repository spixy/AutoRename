using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;

namespace AutoRename
{
    public class ShowGridLinesConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool) value ? DataGridGridLinesVisibility.All : DataGridGridLinesVisibility.None;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch ((DataGridGridLinesVisibility) value)
            {
                case DataGridGridLinesVisibility.All:
                    return true;

                case DataGridGridLinesVisibility.None:
                    return false;

                default:
                    throw new ArgumentOutOfRangeException(nameof(value), value, null);
            }
        }
    }
}
