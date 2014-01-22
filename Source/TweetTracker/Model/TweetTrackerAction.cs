using LinqToTwitter;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace TweetTracker.Model
{
    /// <summary>
    /// Represents an action taken by TweetTracker
    /// </summary>
    class TweetTrackerAction
    {
        private readonly Status _status;

        private readonly ObservableCollection<CaptureSubject> _addedTo;

        private readonly GradientStopCollection _colors;

        public TweetTrackerAction(Status status, IEnumerable<CaptureSubject> addedTo)
        {
            this._addedTo = new ObservableCollection<CaptureSubject>(addedTo);
            this._colors = new GradientStopCollection();
            this._status = status;

            this.MakeGradients();
            this._colors.Freeze();
        }

        public Status Status
        {
            get
            {
                return this._status;
            }
        }

        public GradientStopCollection Colors
        {
            get
            {
                return this._colors;
            }
        }

        private void MakeGradients()
        {
            if(this._addedTo.Count == 0)
            {
                this.Colors.Add(this.MakeGradientStop(System.Windows.Media.Colors.Gray, 0));
                this.Colors.Add(this.MakeGradientStop(System.Windows.Media.Colors.Gray, 1));
            }
            else if(this._addedTo.Count == 1)
            {
                this.Colors.Add(this.MakeGradientStop(this._addedTo[0].Brush.Color, 0));
                this.Colors.Add(this.MakeGradientStop(this._addedTo[0].Brush.Color, 1));
            }
            else
            {
                var step = 1 / (double)this._addedTo.Count;

                for(int i = 0; i < this._addedTo.Count; i++)
                {
                    this.Colors.Add(this.MakeGradientStop(this._addedTo[i].Brush.Color, step * i));
                }
            }
        }

        private GradientStop MakeGradientStop(Color color, double offset)
        {
            var gradStop = new GradientStop(color, offset);
            gradStop.Freeze();
            return gradStop;
        }
    }
}
