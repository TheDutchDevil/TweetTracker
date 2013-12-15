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

        private static int _maximumDataPoints;

        private static int _dataCount;

        private static int _threshold;

        private static int _dataCountTimer;

        static Settings()
        {
            _timer = new Timer();
            _timer.Elapsed += _timer_Elapsed;
            Reset();

            _maximumDataPoints = 10;
        }

        static void _timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            _dataCountTimer++;
            if (_dataCountTimer % _threshold == 0)
            {
                _dataCount++;
                _dataCountTimer = 0;
            }

            if (_dataCount == _maximumDataPoints)
            {
                _dataCount = _dataCount / 2;
                _threshold *= 2;

                if (DataPointsPassedMax != null)
                {
                    DataPointsPassedMax(new object(), new EventArgs());
                }

            }
        }

        public static int CountInterval { get; set; }

        public static int AcceptThreshold
        {
            get
            {
                return _threshold;
            }
        }

        public static event EventHandler DataPointsPassedMax;

        public static void Reset()
        {
            CountInterval = 2500;
            _dataCount = 0;
            _threshold = 1;
            _dataCountTimer = 0;
            _timer.Stop();
            _timer.Interval = CountInterval;
            _timer.Start();
        }
    }
}
