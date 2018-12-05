using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PrettigLokaalBackend.DomainModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PrettigLokaalBackend.Data
{
    public class PrettigLokaalDataContext : DbContext
    {
        public DbSet<Account> Accounts { get; set; } 


        public PrettigLokaalDataContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<Account>(MapAccount);
        }

        private static void MapAccount(EntityTypeBuilder<Account> builder)
        {
            builder.ToTable("Account");
            builder.HasKey(a => a.Id);
            builder.HasOne(a => a.Merchant);
        }
    }
}
