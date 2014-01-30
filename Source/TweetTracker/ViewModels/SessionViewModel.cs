using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using TweetTracker.Model;
using TweetTracker.ViewModels.Colors;

namespace TweetTracker.ViewModels
{
    class SessionViewModel : BaseViewModel
    {
        private ObservableCollection<CaptureSessionBase> _tabpages;

        private CaptureSession _session;

        private int _currentInterval;

        private Timer _intervalTimer; 

        public SessionViewModel()
        {
            this._tabpages = new ObservableCollection<CaptureSessionBase>();
        }

        public int UpdateInterval
        {
            get
            {
                return this.Session.Settings.Settings.CountInterval / 1000 * this.Session.Settings.Settings.IgnoreDataUpdateThreshold;
            }
        }

        public string UpdateName
        {
            get
            {
                return "Seconds";
            }
        }

        public CaptureSession Session
        {
            get
            {
                return this._session;
            }

            set
            {
                this._session = value;
                this.OnPropertyChanged("Session");
            }
        }

        public ObservableCollection<CaptureSessionBase> Pages
        {
            get
            {
                return this._tabpages;
            }

            private set
            {
                this._tabpages = value;
                this.OnPropertyChanged("Pages");
            }
        }

        public int CurrentInterval
        {
            get
            {
                return this._currentInterval;
            }

            private set
            {
                this._currentInterval = value;
                this.OnPropertyChanged("CurrentInterval");
            }
        }


        public void StartSession(CaptureSession session)
        {    
            if (session == null)
            {
                throw new ArgumentNullException("session");
            }

            this.Pages.Clear();

            this.Session = session;
            this.Session.Settings.Settings.MaxDataPointsPassed += Settings_MaxDataPointsPassed;
            this.Session.StartCaptureNonBlocking();

            this.Pages.Add(new StaticSessionViewModel());
#if DEBUG 
            this.Pages.Add(new DebugTweetViewModel());
#endif 

            foreach(var model in this.Pages)
            {
                model.StartListening(session);
            }

            this._intervalTimer = new Timer();
            this._intervalTimer.Interval = this.Session.Settings.Settings.CountInterval;
            this._intervalTimer.Elapsed += _intervalTimer_Elapsed;
            this._intervalTimer.Start();

            this.OnPropertyChanged("updateInterval");
        }

        void _intervalTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            this.CurrentInterval++;

            if(this.CurrentInterval == this.UpdateInterval)
            {
                Task.Delay(200).ContinueWith((b) => Application.Current.Dispatcher.Invoke(() => this.CurrentInterval = 0));
            }
        }

        private void Settings_MaxDataPointsPassed(object sender, EventArgs e)
        {
            this.OnPropertyChanged("UpdateInterval");
        }

        public void StopSession()
        {
            this.Session.StopCapture();

            this.Session.Settings.Settings.MaxDataPointsPassed -= this.Settings_MaxDataPointsPassed;
            this._intervalTimer.Stop();

            foreach (var model in this.Pages)
            {
                model.StopListening();
            }

            ColorProvider.Reset();
        }

        public void UpdateCaptureSettings(CaptureSettingsViewModel settingsModel)
        {
            var settings = new CaptureSettings(settingsModel);
            this.Session.UpdateCaptureSettings(settings);
        }        
    }
}
