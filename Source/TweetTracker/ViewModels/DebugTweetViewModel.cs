using LinqToTwitter;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using TweetTracker.Model;
using TweetTracker.Util;

namespace TweetTracker.ViewModels
{
    class DebugTweetViewModel : CaptureSessionBase
    {
        private const int AddThreshold = 1200;

        private const int StatusListSize = 20;

        private ObservableCollection<TweetTrackerAction> _actions;

        private DateTime _lastAddTime;

        private CaptureSession _session;

        public DebugTweetViewModel()
        {
            this.DisplayName = "Debug";
            this._actions = new ObservableCollection<TweetTrackerAction>();
        }

        public ObservableCollection<TweetTrackerAction> Actions
        {
            get
            {
                return this._actions;
            }
        }

        public override void StartListening(Model.CaptureSession session)
        {
            this._session = session;
            this._actions.Clear();
            session.StatusProcessedEvent += session_StatusProcessedEvent;
            this._lastAddTime = new DateTime();
        }

        public override void StopListening()
        {
            this._session.StatusProcessedEvent -= this.session_StatusProcessedEvent;
        }

        private void session_StatusProcessedEvent(TweetTrackerAction action)
        {
            var timeSpanSinceLastAddition = DateTime.Now - this._lastAddTime;

            if (timeSpanSinceLastAddition.TotalMilliseconds > AddThreshold)
            {
                Application.Current.Dispatcher.Invoke(() => this.AddStatus(action));
            }
        }

        private void AddStatus(TweetTrackerAction action)
        {
            if(!this._session.Settings.HashTag.Equals(string.Empty) || action.AddedTo.Count != 0)
            {
                if (this._actions.Count > StatusListSize)
                {
                    this._actions.RemoveAt(this._actions.Count - 1);
                }

                this._actions.Insert(0, action);
                this._lastAddTime = DateTime.Now;
            }
        }
    }
}
