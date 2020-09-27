using System.Diagnostics;
using TabbedTest.Services;
using Xamarin.Forms;
using Log = TabbedTest.Services.MyLog;

namespace TabbedTest
{
    public partial class App : Application
    {

        public App()
        {
            InitializeComponent();
            DependencyService.Register<MockDataStore>();
            //Register Log
            MainPage = new AppShell();
        }

        protected override void OnStart()
        {
            Log.Trace("App.xaml.cs.OnStart() called");
        }

        protected override void OnSleep()
        {
            Log.Trace("App.xaml.cs.OnSleep() called");
        }

        protected override void OnResume()
        {
            Log.Trace("App.xaml.cs.OnResume() called");
        }


    }
}
