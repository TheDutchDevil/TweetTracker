using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TweetTracker.Model
{
    class TweetCaptureSettings
    {
        private CaptureSettings _settings;

        public TweetCaptureSettings(CaptureSettings settings)
        {
            this._settings = settings;
        }
    }
}
