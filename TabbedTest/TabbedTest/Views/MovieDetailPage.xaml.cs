using TabbedTest.ViewModels;
using Xamarin.Forms;

namespace TabbedTest.Views
{
    public partial class MovieDetailPage : ContentPage
    {
        private readonly MovieDetailViewModel _viewModel;
        public MovieDetailPage()
        {
            InitializeComponent();
            BindingContext = _viewModel = new MovieDetailViewModel();
        }

        private async void CheckBox_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            await _viewModel.ChangeFavourite();
        }

        private void ContentPage_Disappearing(object sender, System.EventArgs e)
        {
            base.OnDisappearing();
            _viewModel.OnDisapperaing();
        }

        private void TapGestureRecognizer_Tapped(object sender, System.EventArgs e)
        {
            _viewModel.OnNameTapped();
        }
    }
}