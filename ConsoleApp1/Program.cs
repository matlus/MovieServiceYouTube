using System;
using System.Threading.Tasks;
using DomainLayer;

namespace ConsoleApp1
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            Console.WriteLine("Please enter you name");
            var name = Console.ReadLine();

            if (name == "Reva")
            {
                Console.WriteLine("Heelo Buttloose");
            }
            else Console.WriteLine("Hello: " + name);

            Console.ReadLine();
            var domainFacade = new DomainFacade();
            var allMovies = await domainFacade.GetAllMovies();
            var sciFiMovies = await domainFacade.GetMoviesByGenre(DomainLayer.Managers.Enums.Genre.SciFi);
        }
    }
}