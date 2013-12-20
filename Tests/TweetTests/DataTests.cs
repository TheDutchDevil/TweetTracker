using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TweetTracker;
using TweetTracker.Model;
using System.Collections.Generic;
using TweetTracker.Model.InformationProviders;
using TweetTracker.DependencyInjection;
using System.Timers;
using TweetTracker.ViewModels;
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

            ServiceProvider.TwitterProvider = new DataTests.MockService();

            var session = new CaptureSession(capSetts);

            var model = new SessionViewModel();

            model.StartCapture(session);

            settings.MaxDataPointsPassed += (sender, args) => Assert.AreEqual(5, model.DeltaCount.Count);

            System.Threading.Thread.Sleep(100000);
        }

        internal class MockService : IProvide
        {

            public void SetSearchString(string searchString)
            {                
            }

            public void StartListening(AcceptStatusUpdate listener)
            {
            }

            public void StopListening()
            {
            }
        }
    }
}
