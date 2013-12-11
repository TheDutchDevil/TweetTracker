using LinqToTwitter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TweetTracker.Model
{
    class TweetTracker
    {
        private List<Status> _trackedTweets;

        private readonly object _trackedTweetsLock = new Object();

        public TweetTracker()
        {
            this._trackedTweets = new List<Status>();
        }

        public Status AttemptToAdd(Status status)
        {
            lock (this._trackedTweetsLock)
            {
                if (!this._trackedTweets.Any(stat => stat.ID == status.ID))
                {
                    this._trackedTweets.Add(status);
                    return status;
                }
                else
                {
                    return null;
                }
            }
        }
    }
}
