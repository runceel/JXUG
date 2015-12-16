using MStopwatch.Commons;
using MStopwatch.Models;
using Prism.Navigation;
using Prism.Services;
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
    public class MainPageViewModel
    {
        private IPageDialogService DialogService { get; }

        private INavigationService NavigationService { get; }

        private Stopwatch Model { get; }

        public ReadOnlyReactiveProperty<string> StartButtonLabel { get; }

        public ReactiveCommand StartCommand { get; }

        public ReactiveCommand LapCommand { get; }

        public ReadOnlyReactiveCollection<LapTimeViewModel> Items { get; }

        public ReadOnlyReactiveProperty<string> NowSpan { get; }

        public MainPageViewModel(Stopwatch model, IPageDialogService dialogService, INavigationService navigationService)
        {
            this.Model = model;
            this.DialogService = dialogService;
            this.NavigationService = navigationService;
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
               .ToReadOnlyReactiveProperty();

            this.StartCommand = new ReactiveCommand();
            this.StartCommand.Subscribe(async _ =>
            {
                switch (this.Model.Mode)
                {
                    case StopwatchMode.Init:
                        this.Model.Start();
                        break;
                    case StopwatchMode.Start:
                        this.Model.Stop();
                        var result = await this.DialogService.DisplayAlert(
                            $"All time: {this.Model.NowSpan.ToString(Constants.TimeSpanFormat)}",
                            $"Max laptime: {this.Model.MaxLapTime.TotalMilliseconds}ms\nMin laptime: {this.Model.MinLapTime.TotalMilliseconds}ms\n\nShow all lap result?",
                            "Yes",
                            "No");
                        if (result)
                        {
                            await this.NavigationService.Navigate("ResultPage");
                        }
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
                .Select(x => x.ToString(Constants.TimeSpanFormat))
                .ToReadOnlyReactiveProperty();

            this.LapCommand = this.Model
                .ObserveProperty(x => x.Mode)
                .Select(x => x == StopwatchMode.Start)
                .ToReactiveCommand();
            this.LapCommand.Subscribe(_ => this.Model.Lap());

            this.Items = this.Model
                .Items
                .ToReadOnlyReactiveCollection(x => new LapTimeViewModel(x));
        }
    }
}
