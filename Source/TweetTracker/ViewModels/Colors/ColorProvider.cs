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

        private static List<Brush> providedColors = new List<Brush>();

        public static Brush getNextColor()
        {
            Brush graphColor;
            
            graphColor = Brushes.Blue;
            providedColors.Add(graphColor);
            return graphColor;
        }

        public static void reset()
        {
            providedColors.Clear();
        }
    }
}
