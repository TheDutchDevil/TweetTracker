using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls.DataVisualization.Charting;
using System.Windows.Media;
using TweetTracker.ViewModels.DataModel;

namespace TweetTracker.Util.StyleProviders
{
    class DataPointStyleProvider
    {
        private static readonly Dictionary<CaptureSubjectMapper, Style> AssociatedStyles;

        private static readonly Style BaseStyle;

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

        private static readonly Random RandomBrushPicker;

        static DataPointStyleProvider()
        {
            RandomBrushPicker = new Random(); 
            AssociatedStyles = new Dictionary<CaptureSubjectMapper, Style>();

            ResourceDictionary res = (ResourceDictionary)Application.LoadComponent(new Uri("Views/StyleDictionary/DataPointStyles.xaml", UriKind.Relative));

            BaseStyle = res["BaseStyle"] as Style;
        }

        public static Style StyleForSubjectMapper(CaptureSubjectMapper mapper)
        {
            if (AssociatedStyles.ContainsKey(mapper))
            {
                return AssociatedStyles[mapper];
            }

            var style = new Style(typeof(LineDataPoint), BaseStyle);
            style.Setters.Add(new Setter(LineDataPoint.BackgroundProperty, AvailableBrushes[RandomBrushPicker.Next(AvailableBrushes.Length)]));
            AssociatedStyles.Add(mapper, style);

            return style;
        }
    }
}
