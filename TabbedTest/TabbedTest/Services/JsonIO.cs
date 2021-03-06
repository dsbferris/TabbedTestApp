﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using TabbedTest.Models;
using Xamarin.Essentials;

namespace TabbedTest.Services
{
    public static class JsonIO
    {
        public static string SelectedMoviesCachePath = Path.Combine(FileSystem.CacheDirectory, "selected_movies.json");
        public static string SavedMoviesAppdataPath = Path.Combine(FileSystem.AppDataDirectory, "saved_movies.json");

        private static readonly JsonSerializerSettings settings = new JsonSerializerSettings()
        {
            NullValueHandling = NullValueHandling.Include,
            Error = new EventHandler<Newtonsoft.Json.Serialization.ErrorEventArgs>((sender, e) => MyLog.Error("JsonIO seri error:\n" + e.ToString()))
        };


        /// <summary>
        /// Saves movie list to location
        /// </summary>
        /// <param name="filepath">Either appdata or cache location</param>
        /// <param name="movies">List of movies</param>
        /// <returns></returns>
        /// <exception cref="Exception">Something went wrong. Use try/catch block!</exception>
        public static Task Save(string filepath, List<Movie> movies)
        {
            /*
            var json = JsonConvert.SerializeObject(movies);

            using var writer = File.CreateText(filepath);
            await writer.WriteAsync(json);
            */

            //var file = Path.Combine(FileSystem.CacheDirectory, "selected_movies.json");
            
            using var stream = new StreamWriter(filepath);
            using var writer = new JsonTextWriter(stream);
            JsonSerializer.Create(settings).Serialize(writer, movies);

            return Task.CompletedTask;
        }

        /// <summary>
        /// Loads default list of movies from assets
        /// </summary>
        /// <returns>List of movies</returns>
        /// <exception cref="Exception">Something went wrong. Use try/catch block!</exception>
        public static async Task<List<Movie>> LoadFromAssets()
        {
            using var stream = await FileSystem.OpenAppPackageFileAsync("movies.json");
            using var sr = new StreamReader(stream);
            using JsonReader reader = new JsonTextReader(sr);
            var movies = JsonSerializer.Create(settings).Deserialize<List<Movie>>(reader);
            return movies;

        }

        /// <summary>
        /// Loads saved list from location
        /// </summary>
        /// <param name="filepath">Most likely Appdata location</param>
        /// <returns></returns>
        /// <exception cref="Exception">Something went wrong. Use try/catch block!</exception>
        public static List<Movie> LoadFromFile(string filepath)
        {
            /*
            var json = File.ReadAllText(filepath);
            var movies = JsonConvert.DeserializeObject<List<Movie>>(json);
            return movies;
            */

            using var stream = new StreamReader(filepath);
            using var reader = new JsonTextReader(stream);
            var movies = JsonSerializer.Create(settings).Deserialize<List<Movie>>(reader);
            return movies;
        }


    }
}
