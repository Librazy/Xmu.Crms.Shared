using System;
using Microsoft.EntityFrameworkCore;

namespace Xmu.Crms.Shared.Models
{
    public class CrmsContext : DbContext
    {
        public CrmsContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<School> Schools { get; set; }

        public DbSet<UserInfo> UserInfos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<School>()
                .Property<DateTime>("gmt_modified")
                .IsRowVersion();

            modelBuilder.Entity<School>()
                .Property<DateTime>("gmt_create")
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<School>()
                .HasKey(s => s.Id);

            modelBuilder.Entity<UserInfo>()
                .Property<DateTime>("gmt_modified")
                .IsRowVersion();

            modelBuilder.Entity<UserInfo>()
                .Property<DateTime>("gmt_create")
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<UserInfo>()
                .HasKey(u => u.Id);

            modelBuilder.Entity<UserInfo>()
                .HasAlternateKey(u => u.Email);

            modelBuilder.Entity<UserInfo>()
                .HasAlternateKey(u => u.Number);

            modelBuilder.Entity<UserInfo>()
                .HasAlternateKey(u => u.Phone);
        }
    }
}