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
using TweetTracker.Model.InformationProviders;
using TweetTracker.Properties;
using TweetTracker.ViewModels;

namespace TweetTracker.Model
{
    sealed class CaptureSession : BaseViewModel
    {
        private IProvide _provider;

        private CaptureSettings _settings;

        private Dictionary<string, CaptureSubject> _captureSubjects;

        private bool _isRunning = false;

        private int _tweetCount = 0;

        private DateTime _startedAt;

        private ObservableCollection<KeyValuePair<int, int>> _countAtInterval;

        private System.Timers.Timer _timer;

        public CaptureSession(CaptureSettings settings)
        {
            // TODO: Inject a tweet serivce here

            this._provider = new TwitterServiceProvider();
            this._timer = new System.Timers.Timer(Settings.CountInterval);

            this._settings = settings;
            this._countAtInterval = new ObservableCollection<KeyValuePair<int, int>>();

            this._captureSubjects = new Dictionary<string, CaptureSubject>();

            foreach(var subjectKey in this._settings.CompareKeys.Keys)
            {
                this._captureSubjects.Add(subjectKey, new CaptureSubject(subjectKey, this._settings.CompareKeys[subjectKey]));
            }
        }

        public Dictionary<string, CaptureSubject> Subjects
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

        public void StartCapture()
        {
         
            if(!this._isRunning)
            {
                var thread = new Thread(this.Update);
                this._isRunning = true;
                thread.Start();
            }
        }

        public void StopCapture()
        {
            if(this._isRunning)
            {
                this._provider.StopListening();
                
                foreach(var subject in this._captureSubjects)
                {
                    subject.Value.StopAccepting();
                }

                this._timer.Stop();
                Settings.CountIntervalChanged -= this.Settings_CountIntervalChanged;
            }
        }

        private void Update()
        {
            this.AppendToAllTweets();
        }

        private void AppendToAllTweets()
        {
            var trackstringBuilder = new StringBuilder();

            if(string.IsNullOrEmpty(this._settings.HashTag))
            {
                foreach(var keywords in this._settings.CompareKeys.Values)
                {
                    foreach(var keyword in keywords)
                    {
                        trackstringBuilder.Append(string.Format(CultureInfo.InvariantCulture,"{0},", keyword));
                    }
                }

                trackstringBuilder.Remove(trackstringBuilder.Length - 1, 1);
            }
            else
            {
                trackstringBuilder.Append(this._settings.HashTag.Replace("#", string.Empty));
            }

            this._provider.StopListening();

            this._countAtInterval.Clear();

            this._provider.SetSearchString(trackstringBuilder.ToString());
            this._provider.StartListening(this.HandleTweet);
            Settings.Reset();

            this._startedAt = DateTime.Now;
            this._timer = new System.Timers.Timer(Settings.CountInterval);
            this._timer.Elapsed += (sender, e) => this._countAtInterval.Add(
                new KeyValuePair<int, int>(this._countAtInterval.Max(kvp => kvp.Key) + Settings.CountInterval / 1000, this.AllTweetsCount));
            this._countAtInterval.Add(
                new KeyValuePair<int, int>(this._countAtInterval.Count * Settings.CountInterval, 0));
            this._timer.Start();
            Settings.CountIntervalChanged +=  Settings_CountIntervalChanged;
        }

        private void Settings_CountIntervalChanged(object sender, EventArgs e)
        {
            _timer.Stop();
            _timer.Interval = Settings.CountInterval;
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

            if (status.Text.Contains(this._settings.HashTag.Replace("#", string.Empty)))
            {
                foreach (var key in this._captureSubjects.Keys)
                {
                    this._captureSubjects[key].AddStatus(status);
                }
            }

            this.AllTweetsCount++;
        }
    }
}
