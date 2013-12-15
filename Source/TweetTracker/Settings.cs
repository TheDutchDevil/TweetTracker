using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace TweetTracker
{
    public static class Settings
    {
        private readonly static Timer _timer;

        static Settings()
        {
            _timer = new Timer();
            _timer.Elapsed += (sender, e) =>
            {
                CountInterval *= 2;
                _timer.Interval = CountInterval * CountIntervalIncrementer;
                if(CountIntervalChanged != null)
                {
                    CountIntervalChanged(new object(), new EventArgs());
                }
            };
            Reset();
        }

        public static int CountIntervalIncrementer { get; set; }

        public static int CountInterval { get; set; }

        public static event EventHandler CountIntervalChanged;

        public static void Reset()
        {
            CountInterval = 5000;
            CountIntervalIncrementer = 10;
            _timer.Stop();
            _timer.Interval = CountInterval * CountIntervalIncrementer;
            _timer.Start();
        }
    }
}
