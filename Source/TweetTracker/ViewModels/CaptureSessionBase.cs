using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TweetTracker.Model;

namespace TweetTracker.ViewModels
{
    abstract class CaptureSessionBase : BaseViewModel
    {
        private string _displayName;

        public CaptureSessionBase()
        {
            this.DisplayName = this.GetType().Name;
        }

        public string DisplayName
        {
            get
            {
                return this._displayName;
            }

            protected   set
            {
                this._displayName = value;
                this.OnPropertyChanged("DisplayName");
            }
        }

        public abstract void StartListening(CaptureSession session);

        public abstract void StopListening();
    }
}
