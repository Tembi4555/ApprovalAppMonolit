using ApprovalApp.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApprovalApp.Data.Configurations
{
    public class TicketConfiguration : IEntityTypeConfiguration<TicketEntity>
    {
        public void Configure(EntityTypeBuilder<TicketEntity> builder)
        {
            builder.ToTable("Tickets");
            builder.HasKey(t => t.Id);
            builder.Property(t => t.Title)
                .HasMaxLength(125)
                .IsRequired();
            builder.Property(t => t.Description)
                .HasMaxLength(500)
                .IsRequired();
            builder.Property(t => t.CreateDate).IsRequired();
            builder.Property(t => t.IdAuthor).IsRequired();
            builder.HasOne(t => t.Person)
                .WithMany(t => t.Tickets)
                .HasForeignKey(t => t.IdAuthor)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();
        }
    }
}
