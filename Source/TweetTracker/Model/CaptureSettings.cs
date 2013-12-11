using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TweetTracker.ViewModels;

namespace TweetTracker.Model
{
    class CaptureSettings
    {
        private CaptureSettings()
        {
            this.HashTag = string.Empty;
            this.CompareKeys = new Dictionary<string, List<string>>();
        }

        public CaptureSettings(CaptureSettingsViewModel model) : this()
        {
            if(model.HashTag == null)
            {
                model.HashTag = string.Empty;
            }

            this.HashTag = model.HashTag;

            foreach(var row in model.SettingsRow)
            {
                if(string.IsNullOrWhiteSpace(row.Key))
                {
                    throw new Exception("Key cannot be empty");
                }
                if(string.IsNullOrWhiteSpace(row.Values))
                {
                    throw new Exception("Values field cannot be empty");
                }

                var keywords = row.Values.Split(';').ToList();

                this.CompareKeys.Add(row.Key, keywords);
            }
        }

        public Dictionary<string, List<string>> CompareKeys
        {
            get;
            private set;
        }

        public string HashTag
        {
            get;
            set;
        }

    }
}
