using Microsoft.Reactive.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MStopwatch.Models;
using MStopwatch.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MStopwatch.Test
{
    [TestClass]
    public class StopwatchViewModelTest
    {
        private Stopwatch model;
        private TestScheduler scheduler;
        private DateTime now;
        private MainPageViewModel target;

        [AssemblyInitialize]
        public static void AssemblyInitialize(TestContext context)
        {
            SynchronizationContext.SetSynchronizationContext(new CurrentThreadSynchronizationContext());
        }

        [TestInitialize]
        public void Initialize()
        {
            this.scheduler = new TestScheduler();
            this.now = DateTime.Now;
            this.scheduler.AdvanceTo(this.now.Ticks);
            this.model = new Stopwatch(this.scheduler);
            this.target = new MainPageViewModel(model);
        }

        [TestCleanup]
        public void Cleanup()
        {
            this.scheduler = null;
            this.model = null;
            this.target = null;
            this.now = default(DateTime);
        }

        [TestMethod]
        public void InitialState()
        {
            Assert.AreEqual(0, this.target.Items.Count);
            Assert.AreEqual("00:00:00", this.target.NowSpan.Value);
            Assert.AreEqual("Start", this.target.StartButtonLabel.Value);
            Assert.IsFalse(this.target.LapCommand.CanExecute());
        }

        [TestMethod]
        public void StartStop()
        {
            this.target.StartCommand.Execute();
            this.scheduler.AdvanceBy(TimeSpan.FromSeconds(1).Ticks);
            Assert.AreEqual(0, this.target.Items.Count);
            Assert.AreEqual("00:00:01", this.target.NowSpan.Value);
            Assert.AreEqual("Stop", this.target.StartButtonLabel.Value);
            this.target.LapCommand.Execute();

            Assert.AreEqual(1, this.target.Items.Count);

            this.target.StartCommand.Execute();
            Assert.AreEqual("Reset", this.target.StartButtonLabel.Value);
            Assert.IsFalse(this.target.LapCommand.CanExecute());

            this.target.StartCommand.Execute();
            Assert.AreEqual("Start", this.target.StartButtonLabel.Value);
        }
    }

    class CurrentThreadSynchronizationContext : SynchronizationContext
    {
        public override void Post(SendOrPostCallback d, object state)
        {
            d(state);
        }
    }
}
