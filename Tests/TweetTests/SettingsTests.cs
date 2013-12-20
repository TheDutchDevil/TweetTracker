using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using TweetTracker;
using TweetTracker.DependencyInjection;
using TweetTracker.Model;

namespace TweetTests
{
    [TestClass]
    public class SettingsTests
    {
        [TestMethod]
        public void AssertDataLimitPassedIsFired()
        {
            var settings = new Settings(10, 100);
            var capSetts = new CaptureSettings("somt", new Dictionary<string, string>(), settings);

            ServiceProvider.TwitterProvider = new DataTests.MockService();

            var session = new CaptureSession(capSetts);

            bool firstIntervalElapsed = false;

            settings.MaxDataPointsPassed += (sender, e) => firstIntervalElapsed = true;

            Timer timer = new System.Timers.Timer(1010);
            timer.Elapsed += (sender, e) => Assert.AreEqual(true, firstIntervalElapsed);

            session.StartCapture();
            timer.Start();

            while(!firstIntervalElapsed)
            {
                System.Threading.Thread.Sleep(5);
            }
            Assert.AreEqual(2, settings.IgnoreDataUpdateThreshold);

            bool secondIntervalElapsed = false;

            settings.MaxDataPointsPassed += (sender, e) => secondIntervalElapsed = true;

            System.Threading.Thread.Sleep(1190);

            Assert.AreEqual(true, secondIntervalElapsed);
        }



        [TestMethod]
        public void AssertSettingsAreCorrectlyStarted()
        {
            var settings = new Settings(10, 100);
            var capSetts = new CaptureSettings("somt", new Dictionary<string, string>(), settings);

            ServiceProvider.TwitterProvider = new DataTests.MockService();

            var session = new CaptureSession(capSetts);

            Assert.AreEqual(false, settings.Started);

            session.StartCapture();

            Assert.AreEqual(true, settings.Started);

            session.StopCapture();

            Assert.AreEqual(false, settings.Started);
        }
    }
}
