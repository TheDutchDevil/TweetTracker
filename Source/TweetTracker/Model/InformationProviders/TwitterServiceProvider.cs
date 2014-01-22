using LinqToTwitter;
using LitJson;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TweetTracker.Properties;

namespace TweetTracker.Model.InformationProviders
{
    class TwitterServiceProvider : IProvide
    {
        private string _currentSearchString;

        private string _currentCultureString;

        private IEnumerable<string> _cultures;

        private bool _isRunning;

        private TwitterContext _context;

        private AcceptStatusUpdate _listener;

        /// <summary>
        /// Current stream from which this Serviceprovider provides
        /// its updates, is null when isRunning is false
        /// </summary>
        private Streaming _stream;


        public TwitterServiceProvider()
        {
            var auth = new SingleUserAuthorizer
            {
                Credentials = new InMemoryCredentials
                {
                    ConsumerKey = Resources.ConsumerKey,
                    ConsumerSecret = Resources.ConsumerSecret,
                    OAuthToken = Resources.OAuthToken,
                    AccessToken = Resources.OAuthAccessTokenSecret

                }
            };

            auth.Authorize();

            this._context = new TwitterContext(auth);
            this._currentCultureString = string.Empty;
            this._isRunning = false;
        }

        public void SetSearchString(string searchString)
        {
            if(searchString == null)
            {
                throw new ArgumentNullException("searchString");
            }

            if (!searchString.Equals(this._currentSearchString))
            {
                this._currentSearchString = searchString;
                Restart();
            }
        }

        /// <summary>
        /// Will restart the service provider if it is running
        /// </summary>
        private void Restart()
        {
            if (this._isRunning)
            {
                this.StopListening();

                Thread.Sleep(200);

                this.StartRunning();
            }
        }

        public void StartListening(AcceptStatusUpdate listener)
        {
            if(listener == null)
            {
                throw new ArgumentNullException("listener");
            }
            if(string.IsNullOrEmpty(this._currentSearchString))
            {
                throw new InvalidOperationException("Cannot start listening without a search string");
            }

            this._listener = listener;

            if(this._isRunning)
            {
                this.StopListening();
            }

            this.StartRunning();
            this._isRunning = true;
        }

        public void StopListening()
        {
            if(this._isRunning)
            {
                this._stream.CloseStream();
                this._isRunning = false;
            }
        }

        private void StartRunning()
        {
            try
            {
                    this._stream = (from stream in this._context.Streaming
                                    where stream.Type == StreamingType.Filter &&
                                    stream.Track == this._currentSearchString.ToString()
                                    select stream).StreamingCallback(this.HandleTweet).Single();
            }
            catch(WebException ex)
            {
                Debug.WriteLine("Could not connect to the twitter API: '{0}'\n{1}", ex.Message, ex.StackTrace);
                throw;
            }
        }


        public void SetCultureString(string cultureString)
        {
            if(cultureString == null)
            {
                throw new ArgumentNullException("cultureString");
            }

            if (this._currentCultureString != cultureString.Replace(" ", string.Empty))
            {
                this._currentCultureString = cultureString.Replace(" ", string.Empty);
                if (this._currentCultureString == string.Empty)
                {
                    this._cultures = null;
                }
                else
                {
                    this._cultures = this._currentCultureString.Split(',');
                }

                this.Restart();
            }
        }

        private void HandleTweet(StreamContent content)
        {
            if (content.Status != TwitterErrorStatus.Success)
            {
                Debug.WriteLine(content.Error.ToString());
                return;
            }

            JsonData statusJson = JsonMapper.ToObject(content.Content);
            var status = new Status(statusJson);

            if(status.Text == null)
            {
                return;
            }

            if (this._cultures != null)
            {
                foreach (var culture in this._cultures)
                {
                    if(status.Lang.Equals(culture))
                    {
                        this._listener(status);
                        break;
                    }
                }
            }
            else
            {
                this._listener(status);
            }
        }
    }
}
