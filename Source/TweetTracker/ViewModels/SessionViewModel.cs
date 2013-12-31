using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TweetTracker.Model;
using TweetTracker.ViewModels.Colors;

namespace TweetTracker.ViewModels
{
    class SessionViewModel : BaseViewModel
    {
        private ObservableCollection<CaptureSessionBase> _tabpages;

        private CaptureSession _session;

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


            this.OnPropertyChanged("updateInterval");
        }

        private void Settings_MaxDataPointsPassed(object sender, EventArgs e)
        {
            this.OnPropertyChanged("UpdateInterval");
        }

        public void StopSession()
        {
            this.Session.StopCapture();

            this.Session.Settings.Settings.MaxDataPointsPassed -= this.Settings_MaxDataPointsPassed;

            foreach (var model in this.Pages)
            {
                model.StopListening();
            }

            ColorProvider.reset();
        }

        public void UpdateCaptureSettings(CaptureSettingsViewModel settingsModel)
        {
            var settings = new CaptureSettings(settingsModel);
            this.Session.UpdateCaptureSettings(settings);
        }
        
    }
}
