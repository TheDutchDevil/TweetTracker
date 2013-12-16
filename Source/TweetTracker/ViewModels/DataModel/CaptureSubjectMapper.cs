using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TweetTracker.Model;

namespace TweetTracker.ViewModels.DataModel
{
    class CaptureSubjectMapper : BaseViewModel
    {
        private CaptureSubject _subject;

        private int _updatesDiscared;

        private Settings _settings;

        public CaptureSubjectMapper(CaptureSubject subject, Settings settings)
        {
            this._settings = settings;
            this.DataPoints = new ObservableCollection<KeyValuePair<DateTime, int>>();
            this._updatesDiscared = 0;
            subject.StatusCountAtTime.CollectionChanged += StatusCountAtTime_CollectionChanged;
            settings.MaxDataPointsPassed += (sender, e) => this.DataPoints.RemoveOneInTwoListItems();
            this.DataPoints.Add(new KeyValuePair<DateTime, int>(DateTime.Now, 0));
            this.Subject = subject;
        }

        void StatusCountAtTime_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (var newItem in e.NewItems)
                {
                    this._updatesDiscared++;
                    if (this._updatesDiscared % this._settings.IgnoreDataUpdateThreshold == 0)
                    {
                        DataPoints.Add(new KeyValuePair<DateTime, int>(DateTime.Now, ((KeyValuePair<int, int>)newItem).Value));
                        this._updatesDiscared = 0;
                    }
                }
            }
        }

        public ObservableCollection<KeyValuePair<DateTime, int>> DataPoints { get; private set; }

        public CaptureSubject Subject
        {
            get
            {
                return this._subject;
            }

            private set
            {
                this._subject = value;
                this.OnPropertyChanged("Subject");
            }
        }

        public string Key
        {
            get
            {
                return this.Subject.Key;
            }
        }
    }
}
