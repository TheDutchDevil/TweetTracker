using LinqToTwitter;
using LitJson;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TweetTracker.Properties;
using TweetTracker.ViewModels;

namespace TweetTracker.Model
{
    class CaptureSession : BaseViewModel
    {
        private TwitterContext _context;

        private CaptureSettings _settings;

        private Dictionary<string, CaptureSubject> _captureSubjects;

        private bool _isRunning = false;

        private int _tweetCount = 0;

        private Streaming _allTweetsStreaming;

        private DateTime _startedAt;

        private ObservableCollection<KeyValuePair<int, int>> _countAtInterval;

        private System.Timers.Timer _timer;

        public CaptureSession(CaptureSettings settings)
        {
            // TODO: Inject a tweet serivce here

            this._settings = settings;
            this._countAtInterval = new ObservableCollection<KeyValuePair<int, int>>();

            var auth = new SingleUserAuthorizer
            {
                Credentials = new InMemoryCredentials
                {
                    ConsumerKey = Resources.ConsumerKey,
                    ConsumerSecret = Resources.ConsumerSecret,
                    OAuthToken = Resources.OAuthToken,
                    AccessToken = Resources.OAuthAccessToken
                    
                }
            };

            auth.Authorize();

            this._context = new TwitterContext(auth);

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

        public void StartUpdate()
        {
         
            if(!this._isRunning)
            {
                var thread = new Thread(this.Update);
                this._isRunning = true;
                thread.Start();
            }
        }

        private void Update()
        {
            this.AppendToAllTweets();

            this._isRunning = false;
        }

        private void AppendToAllTweets()
        {
            var trackstringBuilder = new StringBuilder();

            if(this._settings.HashTag == string.Empty)
            {
                foreach(var keywords in this._settings.CompareKeys.Values)
                {
                    foreach(var keyword in keywords)
                    {
                        trackstringBuilder.Append(string.Format("{0},", keyword));
                    }
                }

                trackstringBuilder.Remove(trackstringBuilder.Length - 1, 1);
            }
            else
            {
                trackstringBuilder.Append(this._settings.HashTag.Replace("#", string.Empty));
            }

            this._allTweetsStreaming = (from stream in this._context.Streaming
                                        where stream.Type == StreamingType.Filter &&
                                        stream.Track == trackstringBuilder.ToString()
                                        select stream).StreamingCallback(this.HandleTweet).SingleOrDefault();

            this._startedAt = DateTime.Now;
            this._countAtInterval.Clear();
            this._timer = new System.Timers.Timer(Settings.CountInterval);
            this._timer.Elapsed += (sedner, e) => this._countAtInterval.Add(
                new KeyValuePair<int, int>(this._countAtInterval.Count * Settings.CountInterval, this.AllTweetsCount));
            this._countAtInterval.Add(
                new KeyValuePair<int, int>(this._countAtInterval.Count * Settings.CountInterval, 0));
            this._timer.Start();
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
