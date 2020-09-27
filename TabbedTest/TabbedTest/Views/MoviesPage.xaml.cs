using TabbedTest.ViewModels;
using Xamarin.Forms;

namespace TabbedTest.Views
{
    public partial class MoviesPage : ContentPage
    {
        private readonly MoviesViewModel _viewModel;

        public MoviesPage()
        {
            InitializeComponent();
            BindingContext = _viewModel = new MoviesViewModel(MoviesListView);
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            _viewModel.OnAppearing();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            _viewModel.OnDisappearing();
        }
    }
}