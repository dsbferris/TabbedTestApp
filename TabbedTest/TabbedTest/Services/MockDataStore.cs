using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TabbedTest.Models;
using Xamarin.Forms;

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
            try
            {
                if (File.Exists(JsonIO.SavedMoviesAppdataPath))
                {
                    return JsonIO.LoadFromFile(JsonIO.SavedMoviesAppdataPath);
                }
                else
                {
                    return JsonIO.LoadFromAssets().Result;
                }
            }
            catch (System.Exception ex)
            {
                Debug.WriteLine("de.ferris exception: " + ex.ToString());
            }
            return new List<Movie>();

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
            return await Task.FromResult(items.OrderBy(m => m.MovieName));
        }


    }
}