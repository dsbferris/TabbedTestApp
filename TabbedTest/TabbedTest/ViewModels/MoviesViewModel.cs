using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using TabbedTest.Models;
using TabbedTest.Services;
using TabbedTest.Views;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace TabbedTest.ViewModels
{
    public class MoviesViewModel : BaseViewModel
    {
        private Movie _selectedItem;

        public ObservableCollection<Movie> Movies { get; }

        public Command LoadItemsCommand { get; }
        public Command SendItemsCommand { get; }

        public Command NextCommand { get; }
        public Command BackCommand { get; }

        public Command<Movie> ItemTappedOnce { get; }
        public Command<Movie> ItemTappedTwice { get; }


        //Pages are 1-index
        private int currentPage;
        private int maxPage;
        private static readonly int itemsPerPage = 100;


        public MoviesViewModel()
        {
            Title = "Browse";
            currentPage = Preferences.Get(nameof(currentPage), 1);
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
            IsBusy = true;

            Movies.Clear();
            string jsonFilter = Preferences.Get("filter", String.Empty);
            var filter = String.IsNullOrEmpty(jsonFilter) ? null : JsonConvert.DeserializeObject<Filter>(jsonFilter);
            var items = filter == null ? await DataStore.GetItemsAsync(true) : await GetMoviesFiltered(filter);

            maxPage = Math.DivRem(items.Count(), itemsPerPage, out int itemsForLastPage);
            if (itemsForLastPage > 0) maxPage++;
            int itemsToLoad = currentPage == maxPage ? itemsForLastPage : itemsPerPage;
            var itemlist = items.ToList();
            itemlist = itemlist.GetRange((currentPage - 1) * itemsPerPage, itemsToLoad);
            foreach (var item in itemlist)
            {
                Movies.Add(item);
            }
            Title = $"Page {currentPage}/{maxPage}";

            IsBusy = false;
        }

        private async Task<IEnumerable<Movie>> GetMoviesFiltered(Filter filter)
        {
            var movies = await DataStore.GetItemsAsync();
            if (filter.OnlyFavourites) movies = movies.Where(m => m.Favourite);
            if (!String.IsNullOrEmpty(filter.Namefilter)) movies = movies.Where(m => m.MovieName.ToLower().Contains(filter.Namefilter.ToLower()));
            if (filter.IsAscending)
            {
                switch (filter.Order)
                {
                    case Filter.OrderMethod.Name: return movies.OrderBy(m => m.MovieName);
                    case Filter.OrderMethod.Size: return movies.OrderBy(m => m.MovieName);
                    case Filter.OrderMethod.Duration: return movies.OrderBy(m => m.MovieDuration);
                    case Filter.OrderMethod.USK: return movies.OrderBy(m => m.USK);
                    case Filter.OrderMethod.Genre: return movies.OrderBy(m => m.Genre);
                    default: return movies;
                }
            }
            else
            {
                switch (filter.Order)
                {
                    case Filter.OrderMethod.Name: return movies.OrderByDescending(m => m.MovieName);
                    case Filter.OrderMethod.Size: return movies.OrderByDescending(m => m.MovieName);
                    case Filter.OrderMethod.Duration: return movies.OrderByDescending(m => m.MovieDuration);
                    case Filter.OrderMethod.USK: return movies.OrderByDescending(m => m.USK);
                    case Filter.OrderMethod.Genre: return movies.OrderByDescending(m => m.Genre);
                    default: return movies;
                }
            }
        }

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

        public Movie SelectedItem
        {
            get => _selectedItem;
            set
            {
                SetProperty(ref _selectedItem, value);
                OnOpenMovieDetail(value);
            }
        }

        async Task OnSendItem()
        {
            var file = JsonIO.SelectedMoviesCachePath;

            var moviesToSave = await DataStore.GetItemsAsync();
            await JsonIO.Save(file, moviesToSave.ToList());

            ShareFile sf = new ShareFile(file, "application/json");
            await Share.RequestAsync(new ShareFileRequest(sf) { Title = "Send movies selection" });
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

        async void PageNext()
        {
            if (currentPage < maxPage)
            {
                currentPage++;
                Preferences.Set(nameof(currentPage), currentPage);
                await ExecuteLoadItemsCommand();
            }
            else if (currentPage == maxPage)
            {
                currentPage = 1;
                Preferences.Set(nameof(currentPage), currentPage);
                await ExecuteLoadItemsCommand();
            }
        }

        async void PageBack()
        {
            if (currentPage > 1)
            {
                currentPage--;
                Preferences.Set(nameof(currentPage), currentPage);
                await ExecuteLoadItemsCommand();
            }
            else if (currentPage == 1)
            {
                currentPage = maxPage;
                Preferences.Set(nameof(currentPage), currentPage);
                await ExecuteLoadItemsCommand();
            }
        }
    }
}