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
        private Settings _settings;

        private CaptureSettings()
        {
            this.HashTag = string.Empty;
            this.CompareKeys = new Dictionary<string, List<string>>();
            this._settings = new Settings(50, 2500);
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
                    throw new ArgumentException("Key cannot be empty");
                }
                if(string.IsNullOrWhiteSpace(row.Values))
                {
                    throw new ArgumentException("Values field cannot be empty");
                }

                var keywords = row.Values.Split(';').ToList();

                this.CompareKeys.Add(row.Key, keywords);
            }
        }

        public CaptureSettings(string hashtag, Dictionary<string, string> values, Settings settings) : this()
        {
            this.HashTag = hashtag;

            foreach(var kvp in values)
            {
                this.CompareKeys.Add(kvp.Key, kvp.Value.Split(';').ToList());
            }

            this._settings = settings;
        }

        public Settings Settings
        {
            get
            {
                return this._settings;
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
