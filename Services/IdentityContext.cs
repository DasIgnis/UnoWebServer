using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnoServer.Models;

namespace UnoServer.Services
{
    public class IdentityContext: IdentityDbContext<User>
    {
        public DbSet<DbMatch> Matches { get; set; }
        public DbSet<DbHand> Hands { get; set; }
        public IdentityContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<User>().ToTable("Users");
            modelBuilder.Entity<DbMatch>().ToTable("Matches");
            modelBuilder.Entity<DbHand>().ToTable("Hands");
        }
    }
}
