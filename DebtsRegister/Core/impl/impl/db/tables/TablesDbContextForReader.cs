using Microsoft.EntityFrameworkCore;

namespace DebtsRegister.Core
{
    /// <summary>
    /// Provides access to tables database for reading purposes.
    /// Changed entities will not be saved back to database.
    /// </summary>
    public class TablesDbContextForReader
        : TablesDbContextBase<TablesDbContextForReader>
    {
        public TablesDbContextForReader(
            DbContextOptions<TablesDbContextForReader> dbOptions
        )
            : base(dbOptions)
        {
            this.ChangeTracker.QueryTrackingBehavior
                = QueryTrackingBehavior.NoTracking;
        }
    }
}