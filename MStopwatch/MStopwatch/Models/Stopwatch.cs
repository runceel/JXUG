using Microsoft.Practices.Unity;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;

namespace MStopwatch.Models
{
    public class Stopwatch : BindableBase
    {
        private DateTime startTime;

        public DateTime StartTime
        {
            get { return this.startTime; }
            set { this.SetProperty(ref this.startTime, value); this.OnPropertyChanged(nameof(this.NowSpan)); }
        }

        private DateTime now;

        public DateTime Now
        {
            get { return this.now; }
            set { this.SetProperty(ref this.now, value); this.OnPropertyChanged(nameof(this.NowSpan)); }
        }

        public TimeSpan NowSpan
        {
            get { return this.Now - this.StartTime; }
        }

        private ObservableCollection<LapTime> ItemsSource { get; } = new ObservableCollection<LapTime>();
        private ReadOnlyObservableCollection<LapTime> items;
        public ReadOnlyObservableCollection<LapTime> Items =>
            items ?? (items = new ReadOnlyObservableCollection<LapTime>(this.ItemsSource));

        private IScheduler TimerScheduler { get; }

        private IDisposable TimerSubscription { get; set; }

        private StopwatchMode mode = StopwatchMode.Init;

        public StopwatchMode Mode
        {
            get { return this.mode; }
            set { this.SetProperty(ref this.mode, value); }
        }


        [InjectionConstructor]
        public Stopwatch() : this(Scheduler.Default)
        {
        }

        public Stopwatch(IScheduler timerScheduler)
        {
            if (timerScheduler == null) { throw new ArgumentNullException(nameof(timerScheduler)); }
            this.TimerScheduler = timerScheduler;
        }

        public void Start()
        {
            if (this.Mode == StopwatchMode.Start) { throw new InvalidOperationException(); }

            this.StartTime = this.TimerScheduler.Now.DateTime;
            this.ItemsSource.Clear();
            this.TimerSubscription = Observable.Interval(TimeSpan.FromMilliseconds(10), this.TimerScheduler)
                .Subscribe(_ =>
                {
                    this.Now = this.TimerScheduler.Now.DateTime;
                });

            this.Mode = StopwatchMode.Start;
        }

        public void Stop()
        {
            if (this.Mode == StopwatchMode.Stop) { throw new InvalidOperationException(); }
            this.TimerSubscription.Dispose();
            this.TimerSubscription = null;

            this.Lap();
            this.Mode = StopwatchMode.Stop;
        }

        public void Reset()
        {
            var nowDateTime = this.TimerScheduler.Now.DateTime;
            this.StartTime = nowDateTime;
            this.Now = nowDateTime;
            this.ItemsSource.Clear();
            this.Mode = StopwatchMode.Init;
        }

        public void Lap()
        {
            this.Now = this.TimerScheduler.Now.DateTime;
            this.ItemsSource.Add(new LapTime(this.Now, this.Now - this.StartTime));
        }
    }
}
