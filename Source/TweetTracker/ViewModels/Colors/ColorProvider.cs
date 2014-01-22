﻿using System;
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
