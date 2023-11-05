using System.Data.Common;
using System.Diagnostics.CodeAnalysis;

namespace DomainLayer;

internal static class DbTransactionExtensions
{
    [ExcludeFromCodeCoverage]
    public static void RollbackIfNotNull(this DbTransaction dbTransaction)
    {
        dbTransaction?.Rollback();
    }
}
