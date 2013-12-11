using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using TweetTracker.Model;

namespace TweetTracker.ViewModels
{
    class SessionViewModel : BaseViewModel
    {
        private CaptureSession _session;

        private ObservableCollection<CaptureSubject> _models;

        private ObservableCollection<KeyValuePair<int, int>> _deltaCount;

        public SessionViewModel(CaptureSession session) : this()
        {
            this.Session = session;
        }

        public SessionViewModel()
        {
            this._models = new ObservableCollection<CaptureSubject>();
            this._deltaCount = new ObservableCollection<KeyValuePair<int, int>>();
            this.Session = null;
        }

        public int ModelsHeight
        {
            get
            {
                return 200 + this.Models.Count * 35;
            }
        }

        public ObservableCollection<KeyValuePair<int, int>> DeltaCount
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

            set
            {
                if(this._session != null)
                {
                    this.Session.CountAtInterval.CollectionChanged -= this.CountAtInterval_CollectionChanged;
                }

                this._session = value;

                this.Models.Clear();
                this.DeltaCount.Clear();

                if (this._session != null)
                {

                    this.Session.Subjects.Values.ToList().ForEach(capSub => this.Models.Add(capSub));

                    this.Session.CountAtInterval.CollectionChanged += CountAtInterval_CollectionChanged;
                }

                this.OnPropertyChanged("Session");
                this.OnPropertyChanged("ModelsHeight");
            }
        }

        void CountAtInterval_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
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
                    
                    Application.Current.Dispatcher.BeginInvoke(new Action(() =>this.DeltaCount.Add(new KeyValuePair<int, int>(((KeyValuePair<int, int>)newItem).Key / 1000, deltaCount))));                    
                }
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

    }

     class Model
    {
        public Model(string key, int count)
        {
            this.Key = key;
            this.Count = count;
        }

        public string Key {get; private set;}
        public int Count {get; private set;}
    }
}
