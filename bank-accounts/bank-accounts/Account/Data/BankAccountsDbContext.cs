using Microsoft.EntityFrameworkCore;

namespace bank_accounts.Account.Data;

public class BankAccountsDbContext(DbContextOptions<BankAccountsDbContext> options) : DbContext(options)
{
    public DbSet<Models.Account> Accounts { get; set; }
    public DbSet<Models.Transaction> Transactions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Models.Account>()
            .HasMany(a => a.Transactions)
            .WithOne()
            .HasForeignKey(t => t.AccountId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}