using DomainLayer;
using DomainLayer.Managers.Enums;
using DomainLayer.Managers.Models;
using System;
using System.Threading.Tasks;

namespace ConsoleApp3
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var domainFacade = new DomainFacade();
            var id = await domainFacade.CreateMovie(new Movie("dfvbds", "http://www.somedomain.com", (Genre)1, 2021));
            var allMovies = await domainFacade.GetAllMovies();
        }
    }
}
