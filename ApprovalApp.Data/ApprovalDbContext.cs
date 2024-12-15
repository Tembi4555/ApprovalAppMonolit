using ApprovalApp.Data.Configurations;
using ApprovalApp.Data.Entities;
using ApprovalApp.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace ApprovalApp.Data
{
    public class ApprovalDbContext : DbContext
    {
        public ApprovalDbContext(DbContextOptions<ApprovalDbContext> options)
            :base(options) 
        {
            
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new PersonConfiguration());
            modelBuilder.ApplyConfiguration(new TicketConfiguration());
            modelBuilder.ApplyConfiguration(new TicketApprovalConfiguration());

            modelBuilder.Entity<PersonEntity>()
                .HasMany(p => p.TicketsForApprovals)
                .WithMany(t => t.Peoples)
                .UsingEntity<TicketApprovalEntity>(
                    j => j
                        .HasOne(pt => pt.Ticket)
                        .WithMany(t => t.TicketApprovalEntities)
                        .HasForeignKey(pt => pt.TicketId),
                    j => j
                        .HasOne(pt => pt.Person)
                        .WithMany(t => t.TicketApprovalEntities)
                        .HasForeignKey(pt => pt.ApprovingPersonId));
        }

        public DbSet<PersonEntity> Persons { get; set; }
        public DbSet<TicketEntity> Tickets { get; set; }
        public DbSet<TicketApprovalEntity> TicketsApprovals { get; set; }
        public DbSet<UserEntity> Users { get; set; }
    }
}
