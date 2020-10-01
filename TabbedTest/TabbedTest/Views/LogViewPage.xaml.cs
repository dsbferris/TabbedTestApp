using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TabbedTest.Services;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Xaml;

namespace TabbedTest.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LogViewPage : ContentPage
    {
        private string _logs;
        public string Logs { get { return _logs; } set { _logs = value; OnPropertyChanged(nameof(Logs)); } }
        public LogViewPage()
        {
            Logs = "\t" + MyLog.ReadAll().Replace("ferrisebrise.log", "").Trim();
            Title = "Log";
            this.BindingContext = this;
            InitializeComponent();
        }
    }
}