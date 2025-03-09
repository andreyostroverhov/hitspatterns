using Core.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DAL
{
    public class CoreDbContext : DbContext
    {
        public DbSet<BankAccount> BankAccounts { get; set; }
        public DbSet<ChangeEvent> ChangeEvents { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<BankAccount>()
                .HasIndex(ba => ba.ClientId);

            modelBuilder.Entity<ChangeEvent>()
                .HasIndex(ce => ce.BankAccId);
        }
        public CoreDbContext(DbContextOptions<CoreDbContext> options) : base(options)
        {
            
        }
    }
}
