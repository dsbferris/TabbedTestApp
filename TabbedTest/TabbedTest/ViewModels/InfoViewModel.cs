using Newtonsoft.Json;
using System;
using System.Linq;
using System.Threading.Tasks;
using TabbedTest.Models;
using Xamarin.Essentials;
using static TabbedTest.Models.Filter;

namespace TabbedTest.ViewModels
{
    public class InfoViewModel : BaseViewModel
    {
        private int _favCount;
        public int FavouritesCount { get { return _favCount; } set { _favCount = value; OnPropertyChanged(nameof(FavouritesCount)); } }
        private int _movCount;
        public int MoviesCount { get { return _movCount; } set { _movCount = value; OnPropertyChanged(nameof(MoviesCount)); } }

        private string _favSize;
        public string FavouritesSize { get { return _favSize; } set { _favSize = value; OnPropertyChanged(nameof(FavouritesSize)); } }
        private string _movSize;
        public string MoviesSize { get { return _movSize; } set { _movSize = value; OnPropertyChanged(nameof(MoviesSize)); } }

        private string _favDuration;
        public string FavouritesDuration { get { return _favDuration; } set { _favDuration = value; OnPropertyChanged(nameof(FavouritesDuration)); } }
        private string _movDuration;
        public string MoviesDuration { get { return _movDuration; } set { _movDuration = value; OnPropertyChanged(nameof(MoviesDuration)); } }




        private bool _showFav = Preferences.Get(nameof(ShowOnlyFavourites), false);
        public bool ShowOnlyFavourites { get { return _showFav; } set { _showFav = value; Preferences.Set(nameof(ShowOnlyFavourites), value); OnPropertyChanged(nameof(ShowOnlyFavourites)); } }

        private string _nameFilter = Preferences.Get(nameof(NameFilter), String.Empty);
        public string NameFilter { get { return _nameFilter; } set { _nameFilter = value; Preferences.Set(nameof(NameFilter), value); OnPropertyChanged(nameof(NameFilter)); } }

        private OrderMethod _order = (OrderMethod)Preferences.Get(nameof(Order), (int)OrderMethod.Name);
        public OrderMethod Order { get { return _order; } set { _order = value; Preferences.Set(nameof(Order), (int)value); OnPropertyChanged(nameof(Order)); } }

        private bool _isAscending = Preferences.Get(nameof(IsAscendingOrdered), true);
        public bool IsAscendingOrdered { get { return _isAscending; } set { _isAscending = value; Preferences.Set(nameof(IsAscendingOrdered), value); OnPropertyChanged(nameof(IsAscendingOrdered)); OnPropertyChanged(nameof(IsDescendingOrdered)); } }

        public bool IsDescendingOrdered { get { return !_isAscending; } set { _isAscending = !value; Preferences.Set(nameof(IsAscendingOrdered), !value); OnPropertyChanged(nameof(IsDescendingOrdered)); OnPropertyChanged(nameof(IsAscendingOrdered)); } }

        public OrderMethod[] Methods => new OrderMethod[] { OrderMethod.Name, OrderMethod.Size, OrderMethod.Duration, OrderMethod.USK, OrderMethod.Genre };

        public InfoViewModel()
        {
            Title = "Info";
        }

        public async Task OnAppearing()
        {
            var movies = await DataStore.GetItemsAsync();
            var favs = movies.Where(m => m.Favourite);

            MoviesCount = movies.Count();
            FavouritesCount = favs.Count();

            FavouritesSize = Models.Movie.GetBytesReadable(favs.Sum(m => m.FileSize));
            MoviesSize = Models.Movie.GetBytesReadable(movies.Sum(m => m.FileSize));

            FavouritesDuration = TimeSpan.FromSeconds(favs.Sum(m => m.MovieSeconds)).ToString();
            MoviesDuration = TimeSpan.FromSeconds(movies.Sum(m => m.MovieSeconds)).ToString();

        }

        public void OnDisappearing()
        {
            var filter = new Filter()
            {
                OnlyFavourites = _showFav,
                Namefilter = _nameFilter,
                Order = _order,
                IsAscending = _isAscending
            };
            string jsonFilter = JsonConvert.SerializeObject(filter);
            Preferences.Set("filter", jsonFilter);
        }

        public void ToggleShowOnlyFavourites()
        {
            ShowOnlyFavourites = !ShowOnlyFavourites;
        }

        public void ResetFilter()
        {
            ShowOnlyFavourites = false;
            NameFilter = String.Empty;
            Order = OrderMethod.Name;
            IsAscendingOrdered = true;

        }
    }
}