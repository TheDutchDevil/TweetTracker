using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

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

        public static void InvokeIn(this Dispatcher dispatcher, int time, Action action)
        {
            var timer = new Timer(e => action(), null, time, Timeout.Infinite);
        }
    }
}
