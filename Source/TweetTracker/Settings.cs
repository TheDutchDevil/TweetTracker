using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace TweetTracker
{
    public class Settings
    {
        private int _maxDataPoints;
        private int _dataPoints;
        private int _recordDataInterval;
        private int _countTimerTickThreshold;
        private int _timerTicksIgnored;

        private bool _started;

        private Timer _timer;

        public Settings(int maxDataPoints, int interval)
        {
            this._maxDataPoints = maxDataPoints;
            this._recordDataInterval = interval;
            this._countTimerTickThreshold = 1;
            this._started = false;

            this._timer = new Timer(this._recordDataInterval);
            this._timer.Elapsed +=_timer_Elapsed;
        }

        public event EventHandler MaxDataPointsPassed;

        public int IgnoreDataUpdateThreshold
        {
            get { return this._countTimerTickThreshold; }
        }

        public int CountInterval
        {
            get { return this._recordDataInterval; }
        }

        public bool Started
        {
            get
            {
                return this._timer.Enabled;
            }
        }

        public void StartCounting()
        {
            if(this._started)
            {
                throw new InvalidOperationException("Session has already been started");
            }
            this._timer.Start();
            this._started = true;
        }

        public void StopCounting()
        {
            if(!this._started)
            {
                throw new InvalidOperationException("Counter cannot be stopped if it has not been started");
            }
            this._timer.Stop();
        }

        private void _timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            this._timerTicksIgnored++;
            if (this._timerTicksIgnored % this._countTimerTickThreshold == 0)
            {
                this._dataPoints++;
                this._timerTicksIgnored = 0;
            }

            if (this._dataPoints == this._maxDataPoints)
            {
                this._dataPoints = this._dataPoints / 2;
                this._countTimerTickThreshold *= 2;

                if (MaxDataPointsPassed != null)
                {
                    try
                    {
                        MaxDataPointsPassed(this, new EventArgs());
                    }
                    catch(Exception ex)
                    {
                        Debug.WriteLine(ex.Message);
                    }
                }
            }
        }
    }
}
