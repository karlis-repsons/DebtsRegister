using Microsoft.EntityFrameworkCore;

namespace DebtsRegister.Core
{
    public class TablesDbContext : TablesDbContextBase<TablesDbContext>
    {
        public TablesDbContext(DbContextOptions<TablesDbContext> dbOptions)
            : base(dbOptions) { }
    }
}