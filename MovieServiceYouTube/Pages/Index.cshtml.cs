using System.Collections.Generic;
using System.Threading.Tasks;
using DomainLayer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MovieServiceCore3.Pages;

public class IndexModel(DomainFacade domainFacade) : PageModel
{
    private readonly DomainFacade _domainFacade = domainFacade;

    public IEnumerable<Movie> Movies { get; private set; } = default!;

    [BindProperty(SupportsGet = true)]
    public string? Genre { get; set; }

    public async Task OnGetAsync()
    {
        Movies = string.IsNullOrEmpty(Genre) ? await _domainFacade.GetAllMovies() : await _domainFacade.GetMoviesByGenre(GenreParser.Parse(Genre));
    }
}
