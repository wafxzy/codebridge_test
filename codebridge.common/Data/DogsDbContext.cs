using codebridge.Common.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace codebridge.Common.Data
{
    public class DogsDbContext : DbContext
    {
        public DogsDbContext(DbContextOptions<DogsDbContext> options) : base(options)
        {
        }

        public DbSet<Dog> Dogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Dog>(entity =>
            {
                entity.HasKey(e => e.Name);
                entity.Property(e => e.Name).HasMaxLength(50).IsRequired();
                entity.Property(e => e.Color).HasMaxLength(100).IsRequired();
                entity.Property(e => e.TailLength).IsRequired();
                entity.Property(e => e.Weight).IsRequired();
            });

            // Seed data
            modelBuilder.Entity<Dog>().HasData(
                new Dog { Name = "Neo", Color = "red&amber", TailLength = 22, Weight = 32 },
                new Dog { Name = "Jessy", Color = "black&white", TailLength = 7, Weight = 14 }
            );
        }
    }
}
