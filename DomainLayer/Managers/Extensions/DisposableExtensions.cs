using System;
using System.Diagnostics.CodeAnalysis;

namespace DomainLayer.Managers.Extensions
{
    [ExcludeFromCodeCoverage]
    internal static class DisposableExtensions
    {
        public static void DisposeIfNotNull(this IDisposable disposable)
        {
            if (disposable == null)
            {
                return;
            }

            disposable.Dispose();
        }
    }
}
