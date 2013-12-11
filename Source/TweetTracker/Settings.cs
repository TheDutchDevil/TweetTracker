using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TweetTracker
{
    public static class Settings
    {
        static Settings()
        {
            CountInterval = 30000;
        }

        public static int CountInterval { get; set; }
    }
}
