using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;
using TweetTracker.ViewModels.DataModel;

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

        private static Brush lastGivenBrush;

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is CaptureSubjectMapper)
            {
                var brush = AvailableBrushes[Random.Next(AvailableBrushes.Length)];
                Debug.WriteLine("gave brush {0} to key {1}", brush.ToString(), (value as CaptureSubjectMapper).Key);
                lastGivenBrush = brush;
                return brush;
            }
            else
            {
                if (value == null && lastGivenBrush != null)
                {
                    Debug.WriteLine("returned a last given brush");
                    var brush = lastGivenBrush;
                    lastGivenBrush = null;
                    return brush;
                }
                else
                {
                    Debug.WriteLine(string.Format("gave brush Purple to object {0}", (value != null ? value.GetType().Name : "null")));
                    return Brushes.Purple;
                }
            }

        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value;
        }
    }
}
