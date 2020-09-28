using System.Data.Common;
using System.Diagnostics.CodeAnalysis;

namespace DomainLayer.Managers.Extensions
{
    internal static class DbTransactionExtensions
    {
        [ExcludeFromCodeCoverage]
        public static void RollbackIfNotNull(this DbTransaction dbTransaction)
        {
            if (dbTransaction != null)
            {
                dbTransaction.Rollback();
            }
        }
    }
}
