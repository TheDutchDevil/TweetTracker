using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using TweetTracker.Model;
using TweetTracker.ViewModels.DataModel;

namespace TweetTracker.ViewModels
{
    class SessionViewModel : BaseViewModel
    {
        private CaptureSession _session;

        private ObservableCollection<CaptureSubject> _models;

        private ObservableCollection<KeyValuePair<DateTime, int>> _deltaCount;

        private ObservableCollection<CaptureSubjectMapper> _subjects;

        private int _dataUpdatesDiscarded;

        public SessionViewModel()
        {
            this._models = new ObservableCollection<CaptureSubject>();
            this._deltaCount = new ObservableCollection<KeyValuePair<DateTime, int>>();
            this.Subjects = new ObservableCollection<CaptureSubjectMapper>();
            this.Session = null;
        }

        public int ModelsHeight
        {
            get
            {
                return 200 + this.Models.Count * 35;
            }
        }

        public ObservableCollection<KeyValuePair<DateTime, int>> DeltaCount
        {
            get
            {
                return this._deltaCount;
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
                    int indexOf = this.Session.CountAtInterval.IndexOf((KeyValuePair<int, int>)newItem);

                    if (indexOf == 0)
                    {
                        continue;
                    }

                    int oldCount = this.Session.CountAtInterval[indexOf - 1].Value;

                    var deltaCount = ((KeyValuePair<int, int>)newItem).Value - oldCount;

                    this._dataUpdatesDiscarded++;
                    if (this._dataUpdatesDiscarded % this.Session.Settings.Settings.IgnoreDataUpdateThreshold == 0)
                    {
                        Application.Current.Dispatcher.BeginInvoke(new Action(() => this.DeltaCount.Add(new KeyValuePair<DateTime, int>(DateTime.Now, deltaCount))));
                        this._dataUpdatesDiscarded = 0;
                    }
                }
            }

            this.Models = new ObservableCollection<CaptureSubject>(this.Models.OrderBy(cpSub => cpSub.AllStatusCount));
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

        public ObservableCollection<CaptureSubject> Models
        {
            get
            {
                return this._models;
            }

            set
            {
                this._models = value;
                this.OnPropertyChanged("Models");
            }
        }

        public void UpdateCaptureSettings(CaptureSettingsViewModel settingsModel)
        {
            var settings = new CaptureSettings(settingsModel);
            this.Session.UpdateCaptureSettings(settings);
        }

        public void StartCapture(CaptureSession session)
        {
            if (session == null)
            {
                throw new ArgumentNullException("session");
            }

            this.Session = session;

            this.Models.Clear();
            this.DeltaCount.Clear();
            
            this.Session.Subjects.ToList().ForEach(capSub => this.Models.Add(capSub));
            this._dataUpdatesDiscarded = 0;

            this.Session.CountAtInterval.CollectionChanged += CountAtInterval_CollectionChanged;
            this.Session.Settings.Settings.MaxDataPointsPassed += (sender, e) => this.DeltaCount.RemoveOneInTwoListItems();

            var newSubjects = new ObservableCollection<CaptureSubjectMapper>();

            foreach (var subject in this.Session.Subjects)
            {
                newSubjects.Add(new CaptureSubjectMapper(subject, this.Session.Settings.Settings));
            }

            this.Subjects = newSubjects;

            this.Session.Subjects.CollectionChanged += Subjects_CollectionChanged;

            this.Session.StartCaptureNonBlocking();


            this.OnPropertyChanged("Session");
            this.OnPropertyChanged("Subjects");
            this.OnPropertyChanged("ModelsHeight");
        }

        private void Subjects_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if(e.NewItems != null)
            {
                var newSubjects = new ObservableCollection<CaptureSubjectMapper>(this.Subjects);

                foreach(var subject in e.NewItems)
                {
                    newSubjects.Add(new CaptureSubjectMapper(subject as CaptureSubject, this.Session.Settings.Settings));
                }

                this.Subjects = newSubjects;
            }
        }

        public void StopCapture()
        {
            this.Session.Subjects.CollectionChanged -= this.Subjects_CollectionChanged;
            this.Session.CountAtInterval.CollectionChanged -= CountAtInterval_CollectionChanged;
            this.Session.StopCapture();
        }

        /// <summary>
        /// When the count interval is changed, remove 50% of all the
        /// CaptureSubject count intervals
        /// </summary>
        private void Settings_CountIntervalChanged(object sender, EventArgs e)
        {           
                this.DeltaCount.RemoveOneInTwoListItems();
        }

    }
}
