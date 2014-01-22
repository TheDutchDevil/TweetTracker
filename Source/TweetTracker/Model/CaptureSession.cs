using LinqToTwitter;
using LitJson;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TweetTracker.DependencyInjection;
using TweetTracker.Model.InformationProviders;
using TweetTracker.Properties;
using TweetTracker.ViewModels;
using TweetTracker.ViewModels.Colors;

namespace TweetTracker.Model
{
    
    internal delegate void StatusProcessed(TweetTrackerAction action);

    sealed class CaptureSession : BaseViewModel
    {
        private IProvide _provider;

        private CaptureSettings _settings;

        private ObservableCollection<CaptureSubject> _captureSubjects;

        private bool _isRunning = false;

        private int _tweetCount = 0;

        private DateTime _startedAt;

        private ObservableCollection<KeyValuePair<int, int>> _countAtInterval;

        private System.Timers.Timer _timer;
        
        public event StatusProcessed StatusProcessedEvent;

        public CaptureSession(CaptureSettings settings)
        {
            this._settings = settings;
            this._provider = ServiceProvider.TwitterProvider;
            this._timer = new System.Timers.Timer(this._settings.Settings.CountInterval);
            this._countAtInterval = new ObservableCollection<KeyValuePair<int, int>>();

            this._captureSubjects = new ObservableCollection<CaptureSubject>();

            foreach(var subjectKey in this._settings.CompareKeys.Keys)
            {

                this._captureSubjects.Add(new CaptureSubject(subjectKey, this._settings.CompareKeys[subjectKey], this.Settings.Settings, ColorProvider.GetNextColor())); 
            }
        }

        public CaptureSettings Settings
        {
            get
            {
                return this._settings;
            }
        }

        public ObservableCollection<CaptureSubject> Subjects
        {
            get
            {
                return this._captureSubjects;
            }
        }

        public int AllTweetsCount
        {
            get
            {
                return this._tweetCount;
            }

            set
            {
                this._tweetCount = value;
                this.OnPropertyChanged("AllTweetsCount");
            }
        }

        public ObservableCollection<KeyValuePair<int, int>> CountAtInterval
        {
            get
            {
                return this._countAtInterval;
            }
        }

        public void StartCaptureNonBlocking()
        {
            var thread = this.GetStartThread();
            thread.Start();
            this._isRunning = true;
        }

        public void StartCapture()
        {
            var thread = this.GetStartThread();
            thread.Start();
            this._isRunning = true;
            thread.Join();
        }

        private Thread GetStartThread()
        {
            if (this._isRunning)
            {
                throw new InvalidOperationException("Cannot start a session that has already been started");
            }

            var thread = new Thread(this.AppendToAllTweets);
            return thread;
        }

        public void StopCapture()
        {
            if(this._isRunning)
            {
                this._provider.StopListening();
                
                foreach(var subject in this._captureSubjects)
                {
                    subject.StopAccepting();
                }

                this._timer.Stop();
                this._settings.Settings.StopCounting();
            }
        }

        public void UpdateCaptureSettings(CaptureSettings settings)
        {
            settings.Settings = this._settings.Settings;

            foreach(var settingsRow in settings.CompareKeys)
            {
                var subject = this._captureSubjects.Where(kvp => kvp.Key == settingsRow.Key).FirstOrDefault();
                if(subject != null)
                {
                    subject.UpdateKeywords(settingsRow.Value);
                }
                else
                {
                    this._captureSubjects.Add(new CaptureSubject(settingsRow.Key, settingsRow.Value, this._settings.Settings, ColorProvider.GetNextColor()));
                }
            }

            var subjectsNoLongerTracked = this._captureSubjects.Where(sub => !settings.CompareKeys.Keys.Contains(sub.Key));

            foreach(var subject in subjectsNoLongerTracked)
            {
                subject.StopAccepting();
            }

            this._settings = settings;

            var searchString = this.MakeSearchString();
            this._provider.SetSearchString(searchString);
            this._provider.SetCultureString(settings.Culture);
        }

        private void AppendToAllTweets()
        {
            var searchString = this.MakeSearchString();

            this._provider.StopListening();

            this._countAtInterval.Clear();

            this._provider.SetSearchString(searchString);
            this._provider.SetCultureString(this._settings.Culture);
            this._provider.StartListening(this.HandleTweet);
            this._settings.Settings.StartCounting();

            this._startedAt = DateTime.Now;
            this._timer = new System.Timers.Timer(this._settings.Settings.CountInterval);
            this._timer.Elapsed += (sender, e) => this._countAtInterval.Add(
                new KeyValuePair<int, int>(this._countAtInterval.Max(kvp => kvp.Key) + this._settings.Settings.CountInterval / 1000, this.AllTweetsCount));
            this._countAtInterval.Add(
                new KeyValuePair<int, int>(this._countAtInterval.Count * this._settings.Settings.CountInterval, 0));
            this._timer.Start();
        }

        private string MakeSearchString()
        {
            var trackstringBuilder = new StringBuilder();

            if (string.IsNullOrEmpty(this._settings.HashTag))
            {
                foreach (var keywords in this._settings.CompareKeys.Values)
                {
                    foreach (var keyword in keywords)
                    {
                        trackstringBuilder.Append(string.Format(CultureInfo.InvariantCulture, "{0},", keyword));
                    }
                }

                trackstringBuilder.Remove(trackstringBuilder.Length - 1, 1);
            }
            else
            {
                trackstringBuilder.Append(this._settings.HashTag.Replace("#", string.Empty));
            }

            return trackstringBuilder.ToString();
        }

        private void Settings_CountIntervalChanged(object sender, EventArgs e)
        {
            _timer.Stop();
            _timer.Interval = this._settings.Settings.CountInterval;
            _timer.Start();
        }

        private void HandleTweet(StreamContent stream)
        {
            if(stream.Status != TwitterErrorStatus.Success)
            {
                Debug.WriteLine(stream.Error.ToString());
                return;
            }

            JsonData statusJson = JsonMapper.ToObject(stream.Content);
            var status = new Status(statusJson);

            if(status.Text == null)
            {
                Debug.WriteLine("status text is null for status with content: " + stream.Content);
                return;
            }

            var addedTo = new List<CaptureSubject>();

            if (status.Text.Contains(this._settings.HashTag.Replace("#", string.Empty)))
            {
                foreach (var subject in this._captureSubjects)
                {
                    if(subject.AddStatus(status))
                    {
                        addedTo.Add(subject);
                    }
                }
            }

            this.AllTweetsCount++;

            if(this.StatusProcessedEvent != null)
            {
                this.StatusProcessedEvent(new TweetTrackerAction(status, addedTo));
            }
        }
    }
}
