using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using TweetTracker.Model;
using TweetTracker.ViewModels.DataModel;

namespace TweetTracker.ViewModels
{
    /// <summary>
    /// ViewModel used to display long term information for one capture
    /// session
    /// </summary>
    class StaticSessionViewModel : CaptureSessionBase
    {
        private CaptureSession _session;

        private ObservableCollection<KeyValuePair<DateTime, int>> _deltaCount;

        private ObservableCollection<CaptureSubjectMapper> _subjects;

        private int _dataUpdatesDiscarded;

        private readonly object _deltaCountLock = new object();

        public StaticSessionViewModel()
        {
            this._deltaCount = new ObservableCollection<KeyValuePair<DateTime, int>>();
            this.Subjects = new ObservableCollection<CaptureSubjectMapper>();
            this.Session = null;
            this.DisplayName = "Long term";
        }

        public int ModelsHeight
        {
            get
            {
                return 200 + this.Session.Subjects.Count * 35;
            }
        }

        public ObservableCollection<KeyValuePair<DateTime, int>> DeltaCount
        {
            get
            {
                return this._deltaCount;
            }

            set
            {
                this._deltaCount = value;
                this.OnPropertyChanged("DeltaCount");
            }
        }

        public CaptureSession Session
        {
            get
            {
                return this._session;
            }

            private set
            {
                this._session = value;
                this.OnPropertyChanged("Session");
            }
        }

        private void CountAtInterval_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (var newItem in e.NewItems)
                {
                    lock (this._deltaCountLock)
                    {
                        var countItem = (KeyValuePair<int, int>)newItem;

                        int indexOf = this.Session.CountAtInterval.IndexOf(countItem);

                        if (indexOf == 0)
                        {
                            continue;
                        }

                        this._dataUpdatesDiscarded++;
                        if (this._dataUpdatesDiscarded % this.Session.Settings.Settings.IgnoreDataUpdateThreshold == 0)
                        {
                            int oldCount = this.DeltaCount.Sum(kvp => kvp.Value);

                            var deltaCount = countItem.Value - oldCount;
                            var timestamp = DateTime.Now;

                            Application.Current.Dispatcher.BeginInvoke(new Action(() => this.DeltaCount.Add(new KeyValuePair<DateTime, int>(timestamp, deltaCount))));
                            this._dataUpdatesDiscarded = 0;
                        }
                    }
                }
            }

        }

        public ObservableCollection<CaptureSubjectMapper> Subjects
        {
            get
            {
                return this._subjects;
            }

            set
            {
                this._subjects = value;
                this.OnPropertyChanged("Subjects");
            }
        }

        public override void StartListening(CaptureSession session)
        {
            if (session == null)
            {
                throw new ArgumentNullException("session");
            }

            this.Session = session;

            this._dataUpdatesDiscarded = 0;

            this.DeltaCount.Add(new KeyValuePair<DateTime, int>(DateTime.Now, 0));
            this.Session.CountAtInterval.CollectionChanged += CountAtInterval_CollectionChanged;
            this.Session.Settings.Settings.MaxDataPointsPassed += (sender, e) => Application.Current.Dispatcher.Invoke(() => this.RemakeDeltaCountList());
            MakeSubjects();

            this.Session.Subjects.CollectionChanged += Subjects_CollectionChanged;

            this.OnPropertyChanged("Session");
            this.OnPropertyChanged("Subjects");
            this.OnPropertyChanged("ModelsHeight");
        }

        private void MakeSubjects()
        {
            var newSubjects = new ObservableCollection<CaptureSubjectMapper>();

            foreach (var subject in this.Session.Subjects)
            {
                newSubjects.Add(new CaptureSubjectMapper(subject, this.Session.Settings.Settings));
            }

            this.Subjects = newSubjects;
        }

        private void Subjects_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                var newSubjects = new ObservableCollection<CaptureSubjectMapper>(this.Subjects);

                foreach (var subject in e.NewItems)
                {
                    newSubjects.Add(new CaptureSubjectMapper(subject as CaptureSubject, this.Session.Settings.Settings));
                }

                this.Subjects = newSubjects;
            }
        }

        public override void StopListening()
        {
            this.Session.Subjects.CollectionChanged -= this.Subjects_CollectionChanged;
            this.Session.CountAtInterval.CollectionChanged -= CountAtInterval_CollectionChanged;
        }

        private void RemakeDeltaCountList()
        {
            lock (this._deltaCountLock)
            {
                var oldList = new ObservableCollection<KeyValuePair<DateTime, int>>(this.DeltaCount);

                var newList = new ObservableCollection<KeyValuePair<DateTime, int>>();

                newList.Add(oldList[0]);

                for (var i = 2; i < oldList.Count; i += 2)
                {
                    var newValue = oldList[i - 1].Value + oldList[i].Value;

                    var time = oldList[i].Key;

                    newList.Add(new KeyValuePair<DateTime, int>(time, newValue));
                }

                this.DeltaCount = newList;
            }
        }
    }
}
