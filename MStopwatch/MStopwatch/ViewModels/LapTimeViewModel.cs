﻿using MStopwatch.Commons;
using MStopwatch.Models;
using Reactive.Bindings;

namespace MStopwatch.ViewModels
{
    public class LapTimeViewModel
    {
        public ReactiveProperty<string> Span { get; }

        public ReactiveProperty<string> Time { get; }

        public LapTimeViewModel(LapTime model)
        {
            this.Time = new ReactiveProperty<string>(model.Time.ToString(Constants.DateTimeFormat));
            this.Span = new ReactiveProperty<string>(model.Span.ToString(Constants.TimeSpanFormat));
        }
    }
}
