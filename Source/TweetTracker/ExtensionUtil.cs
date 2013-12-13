using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace TweetTracker
{
    static class ExtensionUtil
    {
        public static void RemoveOneInTwoListItems<T>(this ObservableCollection<T> collection)
        {
            for (int i = collection.Count - 1; i >= 0; i--)
            {
                if (collection.IndexOf(collection.ElementAt(i)) % 2 != 0)
                {
                    Application.Current.Dispatcher.Invoke(() => collection.RemoveAt(i));
                }
            }
        }
    }
}
