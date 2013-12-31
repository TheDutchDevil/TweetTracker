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
        public static List<Brush> getGraphColors(int amount)
        {
            List<Brush> graphColors = new List<Brush>();
            for (int i = 0; i < amount; i++)
            {             
                graphColors.Add(Brushes.Blue);
            }
            return graphColors;
        }
    }
}
