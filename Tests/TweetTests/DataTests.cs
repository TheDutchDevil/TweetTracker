using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TweetTracker;
using TweetTracker.Model;
using System.Collections.Generic;

namespace TweetTests
{
    [TestClass]
    public class DataTests
    {
        /// <summary>
        /// Test used to verify that no data is thrown away by the model 
        /// </summary>
        [TestMethod]
        public void AssertAllDataIsKeptInView()
        {
            var settings = new Settings(10, 100);
            var capSetts = new CaptureSettings("somt", new Dictionary<string, string>(), settings);



        }
    }
}
