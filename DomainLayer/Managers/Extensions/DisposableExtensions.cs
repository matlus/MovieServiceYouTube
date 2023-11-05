using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace DomainLayer;

[ExcludeFromCodeCoverage]
internal static class DisposableExtensions
{
    public static async ValueTask DisposeIfNotNullAsync(this IAsyncDisposable disposable)
    {
        if (disposable != null)
        {
            await disposable.DisposeAsync();
        }
    }
}
