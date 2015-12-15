using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MStopwatch.Models;
using Microsoft.Reactive.Testing;

namespace MStopwatch.Test
{
    [TestClass]
    public class StopwatchTest
    {
        private Stopwatch target;
        private TestScheduler scheduler;
        private DateTime now;

        [TestInitialize]
        public void Initialize()
        {
            this.now = DateTime.Now;
            this.scheduler = new TestScheduler();
            this.scheduler.AdvanceTo(now.Ticks);
            this.target = new Stopwatch(this.scheduler);
        }

        [TestCleanup]
        public void Cleanup()
        {
            this.now = default(DateTime);
            this.target = null;
            this.scheduler = null;
        }

        [TestMethod]
        public void InitialState()
        {
            Assert.AreEqual(StopwatchMode.Init, this.target.Mode);
        }

        [TestMethod]
        public void StartStop()
        {
            this.target.Start();
            Assert.AreEqual(StopwatchMode.Start, this.target.Mode);
            this.scheduler.AdvanceBy(TimeSpan.FromSeconds(1).Ticks);
            Assert.AreEqual(now + TimeSpan.FromSeconds(1), this.target.Now);
            Assert.AreEqual(TimeSpan.FromSeconds(1), this.target.NowSpan);

            this.target.Lap();
            Assert.AreEqual(1, this.target.Items.Count);
            Assert.AreEqual(now + TimeSpan.FromSeconds(1), this.target.Items[0].Time);
            Assert.AreEqual(TimeSpan.FromSeconds(1), this.target.Items[0].Span);

            this.target.Stop();
            Assert.AreEqual(StopwatchMode.Stop, this.target.Mode);

            this.target.Reset();
            Assert.AreEqual(StopwatchMode.Init, this.target.Mode);
        }
    }
}
