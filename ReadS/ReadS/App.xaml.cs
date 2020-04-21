using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using ReadS.Services;
using ReadS.Views;
using System.Diagnostics;

namespace ReadS
{
    public partial class App : Application
    {

        public App()
        {
            InitializeComponent();

            DependencyService.Register<MockDataStore>();
            MainPage = new NavigationPage(new TabbedPage1());
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
            Debug.WriteLine("OnSleep");
        }

        protected override void OnResume()
        {
        }
    }
}
