using LinqToTwitter;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Media;
using TweetTracker.ViewModels;

namespace TweetTracker.Model
{
    /// <summary>
    /// One subject that is being tracked in a session, keeps track of
    /// all the tweets that match the search term
    /// </summary>
    class CaptureSubject : BaseViewModel
    {
        /// <summary>
        /// the name of the subject being tracked
        /// </summary>
        private readonly string _key;

        /// <summary>
        /// the amount of tweets counted
        /// </summary>
        private double _allStatusCount;

        /// <summary>
        /// the list of keywords with which provided tweets are matched,
        /// regex matching is used for this
        /// </summary>
        private List<string> _keywords;

        /// <summary>
        /// A list of the amount of a tracked tweets at a point in time after
        /// the start of the session
        /// </summary>
        private ObservableCollection<KeyValuePair<int, int>> _statusCountAtTime;

        /// <summary>
        /// Timer used to maintain the _statusCountAtTime collection
        /// </summary>
        private Timer _timer;

        private Settings _settings;

        private readonly SolidColorBrush _brush;

        public CaptureSubject(string key, List<string> keywords, Settings settings, SolidColorBrush brush)
        {
            this._brush = brush;
            this._settings = settings;
            this._keywords = keywords;
            this._key = key;
            this._statusCountAtTime = new ObservableCollection<KeyValuePair<int, int>>();
            this._timer = new Timer(settings.CountInterval);
            this._timer.Elapsed += (sender, e) => 
                Application.Current.Dispatcher.Invoke(new Action(() => this._statusCountAtTime.Add(new KeyValuePair<int,int>(this.StatusCountAtTime.Max(kvp => kvp.Key) + this._settings.CountInterval / 1000, (int) this.AllStatusCount))));
            this._timer.Start();


            this._statusCountAtTime.Add(new KeyValuePair<int, int>((settings.CountInterval / 1000) * this._statusCountAtTime.Count, (int)this.AllStatusCount));
        }

        public string Key
        {
            get
            {
                return this._key;
            }
        }

        public SolidColorBrush Brush
        {
            get
            {
                return this._brush;
            }
        }

        /// <summary>
        /// Gets a reference to the collection in which the amount
        /// of tracked statutes per interval is tracked
        /// </summary>
        public ObservableCollection<KeyValuePair<int, int>> StatusCountAtTime
        {
            get { return this._statusCountAtTime; }
        }

        /// <summary>
        /// Gets or sets the total amount of tweets tracked for
        /// this subject
        /// </summary>
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

        /// <summary>
        /// Updates the keywords that this capture subjects uses to match
        /// tweets
        /// </summary>
        /// <param name="keywords">The keywords (can include regex strings)
        /// which this subject should use to match incoming statuses</param>
        public void UpdateKeywords(List<string> keywords)
        {
            this._keywords = keywords;
        }

        /// <summary>
        /// Takes the provided status and matches it against the
        /// key words to determine whether it should be added
        /// </summary>
        /// <param name="status">The status whose text is used
        /// to match it against the keywords</param>
        /// <returns>True if the status was added to this subject,
        /// otherwise false</returns>
        public bool AddStatus(Status status)
        {
            if (this._timer.Enabled)
            {
                foreach (var keyword in this._keywords)
                {
                    var match = Regex.Match(status.Text, keyword, RegexOptions.IgnoreCase);

                    if (match.Success)
                    {
                        this.AllStatusCount = this._allStatusCount + 1;
                        return true;
                    }
                }
            }

            return false;
        }

        public void StopAccepting()
        {
            this._timer.Stop();
        }
    }
}
