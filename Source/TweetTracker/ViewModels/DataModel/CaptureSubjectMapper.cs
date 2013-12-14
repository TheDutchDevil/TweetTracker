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

        public CaptureSubjectMapper(CaptureSubject subject)
        {
            this.DataPoints = new ObservableCollection<KeyValuePair<DateTime, int>>();
            subject.StatusCountAtTime.CollectionChanged += StatusCountAtTime_CollectionChanged;
            Settings.CountIntervalChanged += (sender, e) => this.DataPoints.RemoveOneInTwoListItems();
            this.DataPoints.Add(new KeyValuePair<DateTime, int>(DateTime.Now, 0));
            this.Subject = subject;
        }

        void StatusCountAtTime_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (var newItem in e.NewItems)
                {
                    DataPoints.Add(new KeyValuePair<DateTime, int>(DateTime.Now, ((KeyValuePair<int, int>)newItem).Value));
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
