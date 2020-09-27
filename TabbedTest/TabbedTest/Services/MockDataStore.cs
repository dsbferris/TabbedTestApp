using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TabbedTest.Models;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace TabbedTest.Services
{
    public class MockDataStore : IDataStore<Movie>
    {
        readonly List<Movie> items;

        public MockDataStore()
        {
            items = Load();
        }

        private List<Movie> Load()
        {
            if (File.Exists(JsonIO.SavedMoviesAppdataPath))
            {
                try
                {
                    var movies = JsonIO.LoadFromFile(JsonIO.SavedMoviesAppdataPath);
                    if (movies.Count > 0) return movies;
                }
                catch (System.Exception ex)
                {
                    MyLog.Error("Could not load items in MockDataStore" + ex.ToString());
                }
            }

            return LoadDefault();
        }

        private List<Movie> LoadDefault()
        {
            return JsonIO.LoadFromAssets().Result;
        }

        public async Task<bool> AddItemAsync(Movie item)
        {
            items.Add(item);

            return await Task.FromResult(true);
        }

        public async Task<bool> UpdateItemAsync(Movie item)
        {
            var oldItem = items.Where((Movie arg) => arg.Id == item.Id).FirstOrDefault();
            items.Remove(oldItem);
            items.Add(item);

            return await Task.FromResult(true);
        }

        public async Task<bool> DeleteItemAsync(string id)
        {
            var oldItem = items.Where((Movie arg) => arg.Id == id).FirstOrDefault();
            items.Remove(oldItem);

            return await Task.FromResult(true);
        }

        public async Task<bool> DeleteItemsAsync(bool foreceRefresh = true)
        {
            items.Clear();
            return await Task.FromResult(true);
        }

        public async Task<Movie> GetItemAsync(string id)
        {
            return await Task.FromResult(items.FirstOrDefault(s => s.Id == id));
        }

        public async Task<IEnumerable<Movie>> GetItemsAsync(bool forceRefresh = false)
        {
            return await Task.FromResult(items);
        }

        public async Task<bool> ResetItems()
        {
            try
            {
                File.Delete(JsonIO.SavedMoviesAppdataPath);
            }
            catch (System.Exception)
            {
                MyLog.Error("Could not delete file for ResetItems");
            }
            items.ForEach(m => m.Favourite = false);
            return await Task.FromResult(true);
        }

    }
}