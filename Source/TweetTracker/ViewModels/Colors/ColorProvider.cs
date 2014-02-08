using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace TweetTracker.ViewModels.Colors
{
    class ColorProvider
    {

        private static List<SolidColorBrush> providedColors = new List<SolidColorBrush>();

        public static SolidColorBrush GetNextColor()
        {
            SolidColorBrush graphColor;
            switch (providedColors.Count)
            {
                case 0:
                    graphColor = Brushes.Blue;
                    break;
                case 1:
                    graphColor = Brushes.Red;
                    break;
                case 2:
                    graphColor = Brushes.HotPink;
                    break;
                case 3:
                    graphColor = Brushes.Green;
                    break;
                case 4:
                    graphColor = Brushes.Yellow;
                    break;
                case 5:
                    graphColor = Brushes.Black;
                    break;
                case 6:
                    graphColor = Brushes.MediumSpringGreen;
                    break;
                case 7:
                    graphColor = Brushes.CadetBlue;
                    break;
                case 8:
                    graphColor = Brushes.Crimson;
                    break;
                case 9:
                    graphColor = Brushes.DarkRed;
                    break;
                case 10:
                    graphColor = Brushes.Gold;
                    break;
                case 11:
                    graphColor = Brushes.Lavender;
                    break;
                case 12:
                    graphColor = Brushes.Crimson;
                    break;
                case 13:
                    graphColor = Brushes.Lime;
                    break;
                case 14:
                    graphColor = Brushes.MediumAquamarine;
                    break;
                case 15:
                    graphColor = Brushes.OrangeRed;
                    break;
                default:
                    graphColor = Brushes.BurlyWood;
                    break;
            }
            
            providedColors.Add(graphColor);
            return graphColor;
        }

        public static void Reset()
        {
            providedColors.Clear();
        }
    }
}
