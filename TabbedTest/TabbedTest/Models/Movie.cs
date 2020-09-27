using Newtonsoft.Json;
using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;

namespace TabbedTest.Models
{
    public class Movie : INotifyPropertyChanged
    {
        private string _id;
        public string Id { get => _id; set => _id = value; }
        private bool _fav;
        public bool Favourite { get => _fav; set { _fav = value; OnPropertyChanged(nameof(Favourite)); } }
        private string _path;
        public string FilePath { get => _path; set => _path = value; }
        [JsonIgnore]
        public string FileName => Path.GetFileName(FilePath);
        private long _size;
        public long FileSize { get => _size; set => _size = value; }
        [JsonIgnore]
        public string SizeReadable => GetBytesReadable(FileSize);


        private string _name;
        public string MovieName { get => _name; set => _name = value; }

        private long _seconds;
        public long MovieSeconds { get => _seconds; set => _seconds = value; }
        [JsonIgnore]
        public TimeSpan MovieDuration => TimeSpan.FromSeconds(MovieSeconds);

        private string _genre;
        public string Genre { get => _genre; set => _genre = value; }

        private int? _usk;
        public int? USK { get => _usk; set => _usk = value; }

        private string _audios;
        public string Audios { get => _audios; set => _audios = value; }

        private string _desc;
        public string Description { get => _desc; set => _desc = value; }

        [JsonIgnore]
        public string FirstChar => Char.IsLetter(MovieName[0]) ? MovieName[0].ToString() : "#";


        public void ToggleFavourite()
        {
            this.Favourite = !this.Favourite;
        }

        public static string GetBytesReadable(long i)
        {
            // Get absolute value
            long absolute_i = (i < 0 ? -i : i);
            // Determine the suffix and readable value
            string suffix;
            double readable;
            if (absolute_i >= 0x1000000000000000) // Exabyte
            {
                suffix = "EB";
                readable = (i >> 50);
            }
            else if (absolute_i >= 0x4000000000000) // Petabyte
            {
                suffix = "PB";
                readable = (i >> 40);
            }
            else if (absolute_i >= 0x10000000000) // Terabyte
            {
                suffix = "TB";
                readable = (i >> 30);
            }
            else if (absolute_i >= 0x40000000) // Gigabyte
            {
                suffix = "GB";
                readable = (i >> 20);
            }
            else if (absolute_i >= 0x100000) // Megabyte
            {
                suffix = "MB";
                readable = (i >> 10);
            }
            else if (absolute_i >= 0x400) // Kilobyte
            {
                suffix = "KB";
                readable = i;
            }
            else
            {
                return i.ToString("0 B"); // Byte
            }
            // Divide by 1024 to get fractional value
            readable /= 1024;
            // Return formatted number with suffix
            return readable.ToString("0.## ") + suffix;
        }



        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            var changed = PropertyChanged;
            if (changed == null)
                return;

            changed.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}