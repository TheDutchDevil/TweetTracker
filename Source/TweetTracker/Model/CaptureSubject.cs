using LinqToTwitter;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using TweetTracker.ViewModels;

namespace TweetTracker.Model
{
    class CaptureSubject : BaseViewModel
    {
        private double _allStatusCount;

        private List<string> _keywords;

        private ObservableCollection<KeyValuePair<int, int>> _statusCountAtTime;

        private string _key;

        private Timer _timer;

        public CaptureSubject(string key, List<string> keywords)
        {
            this._keywords = keywords;
            this._key = key;
            this._statusCountAtTime = new ObservableCollection<KeyValuePair<int, int>>();
            this._timer = new Timer(Settings.CountInterval);
            this._timer.Elapsed += (sender, e) => 
                Application.Current.Dispatcher.Invoke(new Action(() => this._statusCountAtTime.Add(new KeyValuePair<int,int>(this.StatusCountAtTime.Max(kvp => kvp.Key) + Settings.CountInterval / 1000, (int) this.AllStatusCount))));
            this._timer.Start();


            this._statusCountAtTime.Add(new KeyValuePair<int, int>((Settings.CountInterval / 1000) * this._statusCountAtTime.Count, (int)this.AllStatusCount));
        }

        public string Key
        {
            get
            {
                return this._key;
            }
        }

        public ObservableCollection<KeyValuePair<int, int>> StatusCountAtTime
        {
            get { return this._statusCountAtTime; }
        }

        public double AllStatusCount
        {
            get 
            { 
                return this._allStatusCount;            
            }

            set
            { 
                this._allStatusCount = value;
                this.OnPropertyChanged("AllStatusCount");
            }
        }

        public void AddStatus(Status status)
        {
            if (this._timer.Enabled)
            {
                foreach (var keyword in this._keywords)
                {
                    if (status.Text.ToUpperInvariant().Contains(keyword.ToUpperInvariant()))
                    {
                        Console.WriteLine(string.Format(CultureInfo.InvariantCulture, "Added to {0}: '{1}'", this.Key, status.Text));
                        this.AllStatusCount = this._allStatusCount + 1;
                        break;
                    }
                }
            }
        }

        public void StopAccepting()
        {
            this._timer.Stop();
        }

        private void Settings_CountIntervalChanged(object sender, EventArgs e)
        {
            this._timer.Stop();
            this._timer.Interval = Settings.CountInterval;
            this._timer.Start();
        }
    }
}
