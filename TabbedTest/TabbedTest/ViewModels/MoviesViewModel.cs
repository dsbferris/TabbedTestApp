using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TabbedTest.Models;
using TabbedTest.Services;
using TabbedTest.Views;
using Xamarin.Essentials;
using Xamarin.Forms;
using Log = TabbedTest.Services.MyLog;

namespace TabbedTest.ViewModels
{
    public class MoviesViewModel : BaseViewModel
    {
        #region Commands etc.
        private Movie _selectedItem;

        public ObservableCollection<Movie> Movies { get; }

        public Command LoadItemsCommand { get; }
        public Command SendItemsCommand { get; }

        public Command NextCommand { get; }
        public Command BackCommand { get; }

        public Command<Movie> ItemTappedOnce { get; }
        public Command<Movie> ItemTappedTwice { get; }

        #endregion

        //Pages are 1-index
        private int _currentPage;
        public int CurrentPage 
        { 
            get 
            { 
                return _currentPage;
            } 
            set 
            { 

                _currentPage = value;
                Preferences.Set(nameof(CurrentPage), value);
            } 
        }
        private int _maxPage;
        private static readonly int ItemsPerPage = 100;

        private readonly CollectionView _cv;


        public MoviesViewModel(CollectionView cv)
        {
            Log.Trace("Created MoviesViewModel");
            _cv = cv;
            Title = "Browse";
            //_currentPage = Preferences.Get(nameof(_currentPage), 1);
            Movies = new ObservableCollection<Movie>();
            LoadItemsCommand = new Command(async () => await ExecuteLoadItemsCommand());

            ItemTappedOnce = new Command<Movie>(OnSelectMovieFavourite);
            ItemTappedTwice = new Command<Movie>(OnOpenMovieDetail);

            NextCommand = new Command(PageNext);
            BackCommand = new Command(PageBack);

            SendItemsCommand = new Command(async () => await OnSendItem());

        }

        async Task ExecuteLoadItemsCommand()
        {
            Log.Trace("Entering ExecuteLoadItemsCommand");

            IsBusy = true;
            Movies.Clear();
            string jsonFilter = Preferences.Get("filter", String.Empty);
            var filter = String.IsNullOrEmpty(jsonFilter) ? Filter.GetDefault() : JsonConvert.DeserializeObject<Filter>(jsonFilter);

            var items = await GetMoviesFiltered(filter);
            if(items.Count() > 0)
            {
                _maxPage = Math.DivRem(items.Count(), ItemsPerPage, out int itemsForLastPage);

                itemsForLastPage--;
                if (itemsForLastPage > 0) _maxPage++;
                CurrentPage = Preferences.Get(nameof(CurrentPage), 1);
                if (CurrentPage > _maxPage || CurrentPage == 0) CurrentPage = 1;

                int itemsToLoad = _currentPage == _maxPage ? itemsForLastPage : ItemsPerPage;
                var itemlist = items.ToList();
                try
                {
                    itemlist = itemlist.GetRange((_currentPage - 1) * ItemsPerPage, itemsToLoad);
                }
                catch (Exception ex)
                {
                    Log.Error("ExecuteLoadItems GetRange error " + ex.ToString());
                    throw;
                }

                foreach (var item in itemlist)
                {
                    Movies.Add(item);
                }
            }
            else
            {
                _maxPage = 0;
                CurrentPage = 0;
            }

            
            Title = $"Page {_currentPage}/{_maxPage}";
            IsBusy = false;
            Log.Trace("Exiting ExecuteLoadItemsCommand");
        }

        private async Task<IEnumerable<Movie>> GetMoviesFiltered(Filter filter)
        {
            var movies = await DataStore.GetItemsAsync(true);
            if (filter.OnlyFavourites) movies = movies.Where(m => m.Favourite);
            if (!String.IsNullOrEmpty(filter.Namefilter)) movies = movies.Where(m => m.MovieName.ToLower().Contains(filter.Namefilter.ToLower()));
            if (filter.IsAscending)
            {
                return filter.Order switch
                {
                    Filter.OrderMethod.Name => movies.OrderBy(m => m.MovieName),
                    Filter.OrderMethod.Size => movies.OrderBy(m => m.MovieName),
                    Filter.OrderMethod.Duration => movies.OrderBy(m => m.MovieDuration),
                    Filter.OrderMethod.USK => movies.OrderBy(m => m.USK),
                    Filter.OrderMethod.Genre => movies.OrderBy(m => m.Genre),
                    _ => movies,
                };
            }
            else
            {
                return filter.Order switch
                {
                    Filter.OrderMethod.Name => movies.OrderByDescending(m => m.MovieName),
                    Filter.OrderMethod.Size => movies.OrderByDescending(m => m.MovieName),
                    Filter.OrderMethod.Duration => movies.OrderByDescending(m => m.MovieDuration),
                    Filter.OrderMethod.USK => movies.OrderByDescending(m => m.USK),
                    Filter.OrderMethod.Genre => movies.OrderByDescending(m => m.Genre),
                    _ => movies,
                };
            }
        }

        #region working shit
        
        public void OnAppearing()
        {
            IsBusy = true;
            SelectedItem = null;
        }

        public async void OnDisappearing()
        {
            var movies = await DataStore.GetItemsAsync();
            await JsonIO.Save(JsonIO.SavedMoviesAppdataPath, movies.ToList());
        }

        async void PageNext()
        {
            if (CurrentPage < _maxPage) CurrentPage++;
            else if (CurrentPage == _maxPage) CurrentPage = 1;
            else return;
            _cv.ScrollTo(0, -1, ScrollToPosition.Start, true);
            await ExecuteLoadItemsCommand();


        }

        async void PageBack()
        {
            if (CurrentPage > 1) CurrentPage--;
            else if (CurrentPage == 1) CurrentPage = _maxPage;
            else return; 
            _cv.ScrollTo(0, -1, ScrollToPosition.Start, true);
            await ExecuteLoadItemsCommand();
        }

        public Movie SelectedItem
        {
            get => _selectedItem;
            set
            {
                SetProperty(ref _selectedItem, value);
                OnOpenMovieDetail(value);
            }
        }

        #endregion


        async Task OnSendItem()
        {
            Log.Trace("Entering OnSendItem");
            var file = JsonIO.SelectedMoviesCachePath;

            var moviesToSave = await DataStore.GetItemsAsync();
            await JsonIO.Save(file, moviesToSave.ToList());

            ShareFile sf = new ShareFile(file, "application/json");
            await Share.RequestAsync(new ShareFileRequest(sf) { Title = "Send movies selection" });
            Log.Trace("Exiting OnSendItem");
        }




        async void OnOpenMovieDetail(Movie movie)
        {
            if (movie == null)
                return;

            // This will push the MovieDetailPage onto the navigation stack
            await Shell.Current.GoToAsync($"{nameof(MovieDetailPage)}?{nameof(MovieDetailViewModel.ItemId)}={movie.Id}");
        }

        void OnSelectMovieFavourite(Movie movie)
        {

            if (movie == null)
                return;

            movie.ToggleFavourite();
        }

        
    }
}