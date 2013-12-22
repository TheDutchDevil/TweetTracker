using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace TweetTracker.Converters
{
    class RandomColorProvider : IValueConverter
    {
        private static readonly Random Random = new Random();

        private static readonly Brush[] AvailableBrushes = new Brush[] {
                                                    Brushes.Beige,
                                                    Brushes.Black,
                                                    Brushes.Blue,
                                                    Brushes.Cyan,
                                                    Brushes.DarkGoldenrod,
                                                    Brushes.DarkGray,
                                                    Brushes.DarkGreen,
                                                    Brushes.DeepPink,
                                                    Brushes.ForestGreen,
                                                    Brushes.Gold,
                                                    Brushes.Ivory,
                                                    Brushes.LightBlue,
                                                    Brushes.LightSalmon,
                                                    Brushes.LightSeaGreen,
                                                    Brushes.Lime,
                                                    Brushes.Navy
                                                  };

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return AvailableBrushes[Random.Next(AvailableBrushes.Length)];
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value;
        }
    }
}
