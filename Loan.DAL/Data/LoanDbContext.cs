using Loan.DAL.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Loan.DAL.Data;

/// <summary>
/// Loan database context
/// </summary>
public class LoanDbContext : DbContext
{
    /// <summary>
    /// Loan tables
    /// </summary>
    public DbSet<Tariff> Tariffs { get; set; }
    public DbSet<Entities.Loan> Loans { get; set; }
    public DbSet<LoanPayment> LoanPayments { get; set; }
    public DbSet<LoanSchedule> LoanSchedules { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Entities.Loan>()
            .HasOne(l => l.Tariff) // У Loan есть один Tariff
            .WithMany() // У Tariff может быть много Loans
            .HasForeignKey(l => l.TariffId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<LoanPayment>()
            .HasOne(p => p.Loan) // У LoanPayment есть один Loan
            .WithMany() // У Loan может быть много LoanPayments
            .HasForeignKey(p => p.LoanId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<LoanSchedule>()
            .HasOne(s => s.Loan) // У LoanSchedule есть один Loan
            .WithMany() // У Loan может быть много LoanSchedules
            .HasForeignKey(s => s.LoanId) 
            .OnDelete(DeleteBehavior.Cascade);


        modelBuilder.Entity<Entities.Loan>()
            .HasIndex(l => l.ClientId); 

        modelBuilder.Entity<LoanPayment>()
            .HasIndex(p => p.LoanId);

        modelBuilder.Entity<LoanSchedule>()
            .HasIndex(s => s.LoanId);
    }

    /// <inheritdoc />
    public LoanDbContext(DbContextOptions<LoanDbContext> options) : base(options)
    {
    }
}