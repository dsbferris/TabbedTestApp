namespace TabbedTest.Models
{
    public class Filter
    {
        public bool OnlyFavourites { get; set; }
        public string Namefilter { get; set; }
        public OrderMethod Order { get; set; }
        public bool IsAscending { get; set; }

        public enum OrderMethod
        {
            Name,
            Size,
            USK,
            Duration,
            Genre
        }
    }
}
