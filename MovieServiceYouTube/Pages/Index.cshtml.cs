using System.Collections.Generic;
using System.Threading.Tasks;
using DomainLayer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace MovieServiceCore3.Pages;

public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;
    private readonly DomainFacade _domainFacade;

    public IEnumerable<Movie> Movies { get; private set; }

    [BindProperty(SupportsGet = true)]
    public string Genre { get; set; }

    public IndexModel(DomainFacade domainFacade, ILogger<IndexModel> logger)
    {
        _domainFacade = domainFacade;
        _logger = logger;
    }

    public async Task OnGetAsync()
    {
        Movies = string.IsNullOrEmpty(Genre) ? await _domainFacade.GetAllMovies() : await _domainFacade.GetMoviesByGenre(GenreParser.Parse(Genre));
    }
}
