using System;
using TabbedTest.Models;
using TabbedTest.ViewModels;
using Xamarin.Forms;

namespace TabbedTest.Views
{
    public partial class InfoPage : ContentPage
    {
        private readonly InfoViewModel infoViewModel;
        public InfoPage()
        {
            InitializeComponent();
            this.BindingContext = infoViewModel = new InfoViewModel();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await infoViewModel.OnAppearing();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            infoViewModel.OnDisappearing();
        }

        private void Picker_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (sender is Picker p)
            {
                infoViewModel.Order = (Filter.OrderMethod)p.SelectedItem;
            }
        }

        private async void Button_Clicked(object sender, EventArgs e)
        {
            await DisplayAlert("Screw you!", "How dare you support smoking?!", "I'm sorry");
        }

        private void IsAscending_Tapped(object sender, EventArgs e)
        {
            infoViewModel.IsAscendingOrdered = true;
        }

        private void IsDescending_Tapped(object sender, EventArgs e)
        {
            infoViewModel.IsAscendingOrdered = false;
        }

        private void OnlyFavourites_Tapped(object sender, EventArgs e)
        {
            infoViewModel.ToggleShowOnlyFavourites();
        }

        private void ButtonReset_Clicked(object sender, EventArgs e)
        {
            infoViewModel.ResetFilter();
        }
    }
}