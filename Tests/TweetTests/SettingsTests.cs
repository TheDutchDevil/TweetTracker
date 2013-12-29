namespace TweetTests
{
    using System.Collections.Generic;
    using System.Timers;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using TweetTracker;
    using TweetTracker.DependencyInjection;
    using TweetTracker.Model;

    /// <summary>
    /// Class that contains unit tests which can be used to
    /// test the Settings class
    /// </summary>
    [TestClass]
    public class SettingsTests
    {
        /// <summary>
        /// Method that verifies that the Settings.DataLimitPassed event is
        /// correctly fired
        /// </summary>
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

            while (!firstIntervalElapsed)
            {
                System.Threading.Thread.Sleep(5);
            }

            Assert.AreEqual(2, settings.IgnoreDataUpdateThreshold);

            bool secondIntervalElapsed = false;

            settings.MaxDataPointsPassed += (sender, e) => secondIntervalElapsed = true;

            System.Threading.Thread.Sleep(1250);

            Assert.AreEqual(true, secondIntervalElapsed);
        }
        
        /// <summary>
        /// Tests whether the settings object returns the correct status
        /// for several status transitions
        /// </summary>
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
