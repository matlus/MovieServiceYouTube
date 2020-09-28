using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using DomainLayer.Managers;
using DomainLayer.Managers.Enums;
using DomainLayer.Managers.Models;
using DomainLayer.Managers.ServiceLocators;

[assembly: InternalsVisibleTo("AcceptanceTests")]
[assembly: InternalsVisibleTo("Testing.Shared")]
[assembly: InternalsVisibleTo("ClassTests")]

namespace DomainLayer
{
    public sealed class DomainFacade : IDisposable
    {
        private bool _disposed;
        private readonly ServiceLocatorBase _serviceLocator;

        private MovieManager _movieManager;

        private MovieManager MovieManager { get { return _movieManager ??= new MovieManager(_serviceLocator); } }

        public DomainFacade()
          : this(new ServiceLocator())
        {
        }

        internal DomainFacade(ServiceLocatorBase serviceLocator)
        {
            _serviceLocator = serviceLocator;
        }

        public Task<Movie> GetMovieById(int id)
        {
            return MovieManager.GetMovieById(id);
        }

        public Task<IEnumerable<Movie>> GetAllMovies()
        {
            return MovieManager.GetAllMovies();
        }

        public Task<IEnumerable<Movie>> GetMoviesByGenre(Genre genre)
        {
            return MovieManager.GetMoviesByGenre(genre);
        }

        public Task<int> CreateMovie(Movie movie)
        {
            return MovieManager.CreateMovie(movie);
        }

        [ExcludeFromCodeCoverage]
        private void Dispose(bool disposing)
        {
            if (disposing && !_disposed && _movieManager != null)
            {
                var localMovieManager = _movieManager;
                localMovieManager.Dispose();
                _movieManager = null;
                _disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
    }
}
