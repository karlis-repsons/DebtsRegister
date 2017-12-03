using Microsoft.EntityFrameworkCore;

namespace DebtsRegister.Core
{
    public class TablesDbContextBase<TContext> : DbContext
        where TContext : DbContext
    {
        public TablesDbContextBase(DbContextOptions<TContext> dbOptions)
            : base(dbOptions) { }

        public DbSet<PersonRow> People { get; set; }

        public DbSet<DebtDealRow> DebtDeals { get; set; }

        public DbSet<DebtDealAnalysisRow> DebtDealsAnalysis { get; set; }

        public DbSet<DebtRow> CurrentDebts { get; set; }

        public DbSet<PersonTotalsRow> CurrentTotalsPerPerson { get; set; }

        public DbSet<PersonStatisticsRow> CurrentStatisticsPerPerson
            { get; set; }

        public DbSet<PeopleStatisticsDocumentRow>
            CurrentPeopleStatisticsDocuments { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            {
                var eb = modelBuilder.Entity<PersonRow>();
                eb.HasIndex(e => e.Name);
                eb.HasIndex(e => e.Surname);
            }

            {
                var eb = modelBuilder.Entity<DebtDealRow>();
                eb.HasIndex(e => e.Time);
                eb.HasIndex(e => e.GiverId);
                eb.HasIndex(e => e.TakerId);
            }

            {
                var eb = modelBuilder.Entity<DebtRow>();
                eb.HasKey(e => new { e.CreditorId, e.DebtorId });
                eb.HasIndex(e => e.CreditorId);
                eb.HasIndex(e => e.DebtorId);
            }

            {
                var eb = modelBuilder.Entity<PersonTotalsRow>();
                eb.HasIndex(e => e.DueDebtsTotal);
            }
        }
    }
}