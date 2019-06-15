using System.Data;
using System.Data.Common;
using System.Threading.Tasks;

namespace Crumbs.EFCore.Session
{
    public class DatabaseConnectionWrapper : IDataStoreConnection
    {
        private readonly IFrameworkContext _context;

        public DatabaseConnectionWrapper(IFrameworkContext context)
        {
            _context = context;
        }

        public async Task OpenConnection()
        {
            Connection = await _context.OpenDbConnection();
        }

        public DbConnection Connection { get; private set; }

        public IDataStoreTransaction BeginTransaction(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted)
        {
            return new DatabaseTransactionWrapper(Connection.BeginTransaction(isolationLevel));
        }

        public void Dispose()
        {
            Connection.Dispose();
            _context.Dispose();
        }
    }
}