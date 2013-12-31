using LinqToTwitter;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using TweetTracker.Model;
using TweetTracker.Util;

namespace TweetTracker.ViewModels
{
    class DebugTweetViewModel : CaptureSessionBase
    {
        private const int AddThreshold = 1200;

        private const int StatusListSize = 20;

        private ObservableCollection<Status> _statuses;

        private DateTime _lastAddTime;

        private CaptureSession _session;

        public DebugTweetViewModel()
        {
            this.DisplayName = "Debug";
            this._statuses = new ObservableCollection<Status>();
        }

        public ObservableCollection<Status> Statuses
        {
            get
            {
                return this._statuses;
            }
        }

        public override void StartListening(Model.CaptureSession session)
        {
            this._session = session;
            this._statuses.Clear();
            session.StatusProcessedEvent += session_StatusProcessedEvent;
            this._lastAddTime = new DateTime();
        }

        public override void StopListening()
        {
            this._session.StatusProcessedEvent -= this.session_StatusProcessedEvent;
        }

        private void session_StatusProcessedEvent(LinqToTwitter.Status status, string action)
        {
            var timeSpanSinceLastAddition = DateTime.Now - this._lastAddTime;

            if (timeSpanSinceLastAddition.TotalMilliseconds > AddThreshold)
            {
                Application.Current.Dispatcher.Invoke(() => this.AddStatus(status));
            }
        }

        private void AddStatus(Status status)
        {
            if (this._statuses.Count > StatusListSize)
            {
                this._statuses.RemoveAt(this._statuses.Count - 1);
            }

            this._statuses.Insert(0, status);
            this._lastAddTime = DateTime.Now;
        }
    }
}
