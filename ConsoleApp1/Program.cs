using System.Threading.Tasks;
using DomainLayer;

namespace ConsoleApp1
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            var domainFacade = new DomainFacade();
            var allMovies = await domainFacade.GetAllMovies();
            var sciFiMovies = await domainFacade.GetMoviesByGenre(DomainLayer.Managers.Enums.Genre.SciFi);
        }
    }
}