using System;
using System.IO;
using System.Text;
using Xamarin.Essentials;

namespace TabbedTest.Services
{
    public static class MyLog
    {
        private static readonly string file = Path.Combine(FileSystem.AppDataDirectory, "logfile.txt");
        private static readonly string tag = "ferrisebrise.log";

        /// <summary>
        /// Creates log with Debug level
        /// </summary>
        /// <param name="message">Message to be logged</param>
        public static void Debug(string message)
        {
            Write("DEBUG", message);
        }

        /// <summary>
        /// Creates log with Error level
        /// </summary>
        /// <param name="message">Message to be logged</param>
        public static void Error(string message)
        {
            Write("ERROR", message);
        }

        /// <summary>
        /// Creates log with Info level
        /// </summary>
        /// <param name="message">Message to be logged</param>
        public static void Info(string message)
        {
            Write("INFO", message);
        }

        /// <summary>
        /// Creates log with Trace level
        /// </summary>
        /// <param name="message">Message to be logged</param>
        public static void Trace(string message)
        {
            Write("TRACE", message);
        }

        /// <summary>
        /// Creates log with Warn level
        /// </summary>
        /// <param name="message">Message to be logged</param>
        public static void Warn(string message)
        {
            Write("WARN", message);
        }



        /// <summary>
        /// Writes message to console and file
        /// </summary>
        /// <param name="level">Log level</param>
        /// <param name="message">Message to be written</param>
        private static void Write(string level, string message)
        {
            var time = String.Empty;
            var dtime = DateTime.Now;
            try
            {
                time = dtime.ToString("T") + "." + dtime.ToString("fff");
            }
            catch (Exception)
            {
            }
            StringBuilder sb = new StringBuilder();
            sb.Append(tag);
            sb.Append("\t");
            sb.Append(time);
            sb.Append("\t");
            sb.Append(level);
            sb.Append("\t");
            sb.Append(message);
            sb.Append("\n");
            string line = sb.ToString();
            try
            {
                System.Console.WriteLine(line);
                File.AppendAllText(file, line);
            }
            catch (Exception)
            {
            }
        }

        /// <summary>
        /// Read all written Logs from file
        /// </summary>
        /// <returns>Returns Logfile on success, else null</returns>
        public static string ReadAll()
        {
            string text = null;
            try
            {
                text = File.ReadAllText(file);
            }
            catch (Exception)
            {
            }
            return text;
        }
    }
}
