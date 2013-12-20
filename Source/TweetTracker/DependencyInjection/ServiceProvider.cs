using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TweetTracker.Model.InformationProviders;

namespace TweetTracker.DependencyInjection
{
    static class ServiceProvider
    {
        static ServiceProvider()
        {
            TwitterProvider = new TwitterServiceProvider();
        }

        public static IProvide TwitterProvider        { get; set; }
    }
}
