using System;
using System.ComponentModel;
using TabbedTest.Models;
using TabbedTest.ViewModels;
using Xamarin.Forms;
using static TabbedTest.Models.Filter;
using Log = TabbedTest.Services.MyLog;

namespace TabbedTest.Views
{
    [DesignTimeVisible]
    public partial class InfoPage : ContentPage
    {
        private readonly InfoViewModel vm;

        public InfoPage()
        {
            InitializeComponent();
            this.BindingContext = vm = new InfoViewModel();
        }

        #region Working stuff

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await vm.OnAppearing();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            vm.OnDisappearing();
        }

        private void Picker_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (sender is Picker p)
            {
                vm.Order = (OrderMethod)p.SelectedItem;
            }
        }

        private void IsAscending_Tapped(object sender, EventArgs e)
        {
            vm.IsAscendingOrdered = true;
        }

        private void IsDescending_Tapped(object sender, EventArgs e)
        {
            vm.IsAscendingOrdered = false;
        }

        private void OnlyFavourites_Tapped(object sender, EventArgs e)
        {
            vm.ToggleShowOnlyFavourites();
        }

        private void ButtonReset_Clicked(object sender, EventArgs e)
        {
            vm.ResetFilter();
        }

        #endregion

        private async void ButtonSendLogs_Clicked(object sender, EventArgs e)
        {
            await vm.SendLogs();
        }

        private async void ButtonClearLogs_Clicked(object sender, EventArgs e)
        {
            var answer = await DisplayAlert("Delete logs?", "Are you sure to delete logs?", "Yes", "No");
            if(answer) Log.Clear();
        }

        private async void ButtonFun_Clicked(object sender, EventArgs e)
        {
            await DisplayAlert("Screw you!", "How dare you support smoking?!", "I'm sorry");
        }

        private async void ButtonResetFavourites_Clicked(object sender, EventArgs e)
        {
            if (await DisplayAlert("Delete favourites?", "Are you sure you want to delete all marked favourites?", "Yes", "No"))
            await vm.ResetFavourites();
        }
    }
}