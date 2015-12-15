using MStopwatch.Models;
using Prism.Navigation;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MStopwatch.ViewModels
{
    public class MainPageViewModel : INavigationAware
    {
        private CompositeDisposable Disposable { get; } = new CompositeDisposable();

        private Stopwatch Model { get; }

        public ReadOnlyReactiveProperty<string> StartButtonLabel { get; }

        public ReactiveCommand StartCommand { get; }

        public ReactiveCommand LapCommand { get; }

        public ReadOnlyReactiveCollection<LapTimeViewModel> Items { get; }

        public ReadOnlyReactiveProperty<string> NowSpan { get; }

        public MainPageViewModel(Stopwatch model)
        {
            this.Model = model;
            this.StartButtonLabel = this.Model
                .ObserveProperty(x => x.Mode)
                .Select(x =>
                {
                    switch (x)
                    {
                        case StopwatchMode.Init:
                            return "Start";
                        case StopwatchMode.Start:
                            return "Stop";
                        case StopwatchMode.Stop:
                            return "Reset";
                        default:
                            throw new InvalidOperationException();
                    }
                })
                .ToReadOnlyReactiveProperty()
                .AddTo(this.Disposable);

            this.StartCommand = new ReactiveCommand();
            this.StartCommand.Subscribe(_ =>
            {
                switch (this.Model.Mode)
                {
                    case StopwatchMode.Init:
                        this.Model.Start();
                        break;
                    case StopwatchMode.Start:
                        this.Model.Stop();
                        break;
                    case StopwatchMode.Stop:
                        this.Model.Reset();
                        break;
                    default:
                        throw new InvalidOperationException();
                }
            });

            this.NowSpan = this.Model
                .ObserveProperty(x => x.NowSpan)
                .Select(x => x.ToString())
                .ToReadOnlyReactiveProperty();

            this.LapCommand = this.Model
                .ObserveProperty(x => x.Mode)
                .Select(x => x == StopwatchMode.Start)
                .ToReactiveCommand()
                .AddTo(this.Disposable);
            this.LapCommand.Subscribe(_ => this.Model.Lap());

            this.Items = this.Model
                .Items
                .ToReadOnlyReactiveCollection(x => new LapTimeViewModel(x));
        }

        public void OnNavigatedFrom(NavigationParameters parameters)
        {
            this.Disposable.Dispose();
        }

        public void OnNavigatedTo(NavigationParameters parameters)
        {
        }
    }
}
