using DomainLayer.Managers.Enums;

namespace DomainLayer.Managers.Models
{
    public sealed class Movie
    {
        public string Title { get; }
        public string ImageUrl { get; }
        public Genre Genre { get; }
        public int Year { get; }

        public Movie(string title, string imageUrl, Genre genre, int year)
        {
            Title = title;
            ImageUrl = imageUrl;
            Genre = genre;
            Year = year;
        }
    }
}
