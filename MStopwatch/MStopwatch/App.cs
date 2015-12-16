using Microsoft.Practices.Unity;
using MStopwatch.Models;
using MStopwatch.ViewModels;
using MStopwatch.Views;
using Prism.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;

namespace MStopwatch
{
    public class App : PrismApplication
    {
        public App()
        {
        }

        protected override void OnInitialized()
        {
            this.NavigationService.Navigate("MainPage");
        }

        protected override void RegisterTypes()
        {
            this.Container.RegisterType<Stopwatch>(new ContainerControlledLifetimeManager());
            this.Container.RegisterTypeForNavigation<MainPage>();
            this.Container.RegisterTypeForNavigation<ResultPage>();
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
