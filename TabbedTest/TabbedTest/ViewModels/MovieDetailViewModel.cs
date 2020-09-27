using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace TabbedTest.ViewModels
{
    [QueryProperty(nameof(ItemId), nameof(ItemId))]
    public class MovieDetailViewModel : BaseViewModel
    {
        private string itemId;
        public string Id { get; set; }

        private bool _fav;
        private string _moviename;
        private TimeSpan _duration;
        private string _size;
        private int? _usk;
        private string _genre;
        private string _audios;
        private string _description;

        public bool Favourite
        {
            get
            {
                return _fav;
            }
            set
            {
                _fav = value;
                OnPropertyChanged(nameof(Favourite));

            }
        }

        public string MovieName { get { return _moviename; } set { _moviename = value; OnPropertyChanged(nameof(MovieName)); } }
        public TimeSpan MovieDuration { get { return _duration; } set { _duration = value; OnPropertyChanged(nameof(MovieDuration)); } }
        public string SizeReadable { get { return _size; } set { _size = value; OnPropertyChanged(nameof(SizeReadable)); } }
        public int? USK { get { return _usk; } set { _usk = value; OnPropertyChanged(nameof(USK)); } }
        public string Genre { get { return _genre; } set { _genre = value; OnPropertyChanged(nameof(Genre)); } }
        public string Audios { get { return _audios; } set { _audios = value; OnPropertyChanged(nameof(Audios)); } }
        public string Description { get { return _description; } set { _description = value; OnPropertyChanged(nameof(Description)); } }

        public string ItemId
        {
            get
            {
                return itemId;
            }
            set
            {
                itemId = value;
                LoadItemId(value);
            }
        }

        public async void LoadItemId(string itemId)
        {
            try
            {
                var m = await DataStore.GetItemAsync(itemId);
                Id = m.Id;
                Favourite = m.Favourite;
                MovieName = m.MovieName;
                MovieDuration = m.MovieDuration;
                SizeReadable = m.SizeReadable;
                USK = m.USK;
                Genre = m.Genre;
                Audios = m.Audios;
                Description = m.Description;
                Title = MovieName;
            }
            catch (Exception)
            {
                Debug.WriteLine("de.ferris exception: Failed to Load Item");
            }
        }

        public async Task ChangeFavourite()
        {
            var m = await DataStore.GetItemAsync(itemId);
            m.ToggleFavourite();
        }

        public async void OnDisapperaing()
        {
            var m = await DataStore.GetItemAsync(itemId);
            m.Favourite = Favourite;
        }

        public async void OnNameTapped()
        {
            string uri = "https://www.themoviedb.org/search?query=" + MovieName;
            await Browser.OpenAsync(uri, BrowserLaunchMode.SystemPreferred);
        }
    }
}
