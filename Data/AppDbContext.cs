using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using EF_project.Entitty;

namespace EF_project.Data
{
    internal class AppDbContext : DbContext
    {
        public DbSet<Film> Films { get; set; }
        public DbSet<Hall> Halls { get; set; }
        public DbSet<Session> Sessions { get; set; }
        public DbSet<StatusSession> StatusSessions { get; set; }
        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<StatusTicket> StatusTickets { get; set; }
        public DbSet<User> Users { get; set; } 
        public DbSet<Sale> Sales{ get; set; }
        public DbSet<Discount> Discounts { get; set; }
        public DbSet<RegularDiscount> RegularDiscounts { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySql(Config.ConString, ServerVersion.AutoDetect(Config.ConString));
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Film>()
                .HasOne(f => f.Discount)
                .WithOne(d => d.Film)
                .HasForeignKey<Discount>(d => d.FilmId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Film>()
                .HasOne(f => f.RegularDiscount)
                .WithOne(d => d.Film)
                .HasForeignKey<RegularDiscount>(d => d.FilmId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Session>()
                .HasOne(s => s.Film)
                .WithMany(f => f.Sessions)
                .HasForeignKey(s => s.FilmId)
                .OnDelete(DeleteBehavior.Cascade); //Non-nullable foreign key problem, cascade time-solution

            modelBuilder.Entity<Session>()
                .HasOne(s => s.Hall)
                .WithMany(h => h.Sessions)
                .HasForeignKey(s => s.HallId)
                .OnDelete(DeleteBehavior.Cascade);//Non-nullable foreign key problem, cascade time-solution

            modelBuilder.Entity<Ticket>()
                .HasOne(t => t.Session)
                .WithMany(s => s.Tickets)
                .HasForeignKey(t => t.SessionId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Ticket>()
                .HasOne(t => t.Status)
                .WithMany(s => s.Tickets)
                .HasForeignKey(t => t.StatusId)
                .OnDelete(DeleteBehavior.Cascade);//Non-nullable foreign key problem, cascade time-solution

            modelBuilder.Entity<Sale>()
                .HasOne(s => s.User)
                .WithMany(u => u.Sales)
                .HasForeignKey(s => s.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Hall>()
                .Property(h => h.IsVip)
                .HasDefaultValue(false);

            modelBuilder.Entity<Sale>()
                .Property(s => s.SaleDate)
                .HasDefaultValue(DateTime.Now);

           modelBuilder.Entity<User>()
                .Property(u => u.Bonuses)
                .HasDefaultValue(0);

            modelBuilder.Entity<User>()
                .Property(u => u.IsAdmin)
                .HasDefaultValue(false);
        }
    }
}
