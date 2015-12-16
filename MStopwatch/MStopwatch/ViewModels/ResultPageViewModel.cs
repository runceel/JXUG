using MStopwatch.Models;
using Prism.Navigation;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Text;
using System.Threading.Tasks;

namespace MStopwatch.ViewModels
{
    public class ResultPageViewModel : INavigationAware
    {
        private CompositeDisposable Disposable { get; } = new CompositeDisposable();

        public ReadOnlyReactiveCollection<LapTimeViewModel> LapTimes { get; }

        public ResultPageViewModel(Stopwatch model)
        {
            this.LapTimes = model.Items
                .ToReadOnlyReactiveCollection(x => new LapTimeViewModel(x));
        }

        public void OnNavigatedFrom(NavigationParameters parameters)
        {
        }

        public void OnNavigatedTo(NavigationParameters parameters)
        {
        }
    }
}
