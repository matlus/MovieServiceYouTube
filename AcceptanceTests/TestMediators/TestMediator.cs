using System.Collections.Generic;
using DomainLayer;

namespace AcceptanceTests.TestMediators;

internal sealed class TestMediator
{
    public IEnumerable<Movie>? MoviesForGetAllMovies { get; set; }

    public ExceptionInformation? ExceptionInformation { get; set; }
}

internal sealed class ExceptionInformation
{
    public ExceptionReason ExceptionReason { get; set; }
}

internal enum ExceptionReason
{
    NotFond = 404,
    BadRequest = 400,
    ProxyAuthenticationRequired = 407,
    BadGateway = 502,
    ServiceUnavailable = 503,
}
