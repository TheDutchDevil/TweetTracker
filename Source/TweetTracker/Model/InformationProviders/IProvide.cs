using LinqToTwitter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TweetTracker.Model.InformationProviders
{
    public delegate void AcceptStatusUpdate(Status status);

    interface IProvide
    {
        /// <summary>
        /// <para>Search string for which the service provider
        /// will return statuses, different search terms
        /// should be seperated by commas (logical OR) or
        /// spaces (logical AND)</para>
        /// <para>If called while a session is active the
        /// provider will adapt to the new search string</para>
        /// </summary>
        /// <param name="searchString"></param>
        void SetSearchString(string searchString);

        void SetCultureString(string cultureString);

        void StartListening(AcceptStatusUpdate listener);

        void StopListening();
    }
}
