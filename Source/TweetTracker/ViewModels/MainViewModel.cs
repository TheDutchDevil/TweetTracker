using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TweetTracker.Model;

namespace TweetTracker.ViewModels
{
    class MainViewModel : BaseViewModel
    {

        private SessionViewModel _session;

        public MainViewModel()
        {
            this.Settings = new CaptureSettingsViewModel();
            this.StartCommand = new RelayCommand(this.PrepareForCapture);
            this.StopCommand = new RelayCommand(this.StopCapture);
            this.Session = new SessionViewModel();
        }


        public CaptureSettingsViewModel Settings { get; private set; }

        public RelayCommand StartCommand
        {
            get;
            private set;
        }

        public RelayCommand StopCommand
        {
            get;
            private set;
        }

        public SessionViewModel Session
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

        public bool SessionStarted { get; private set; }

        public bool SessionNotStarted
        {
            get
            {
                return !this.SessionStarted;
            }
        }

        private void PrepareForCapture()
        {
            var settings = new CaptureSettings(this.Settings);
            var capSession = new CaptureSession(settings);
            this.SessionStarted = true;
            this.Session.StartCapture(capSession);
            this.OnPropertyChanged("SessionStarted");
            this.OnPropertyChanged("SessionNotStarted");
        }

        private void StopCapture()
        {
            this.Session.StopCapture();

            this.SessionStarted = false;
            this.OnPropertyChanged("SessionStarted");
            this.OnPropertyChanged("SessionNotStarted");
        }
    }
}
