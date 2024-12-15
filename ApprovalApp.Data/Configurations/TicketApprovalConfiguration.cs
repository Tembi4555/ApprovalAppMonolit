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
    public class TicketApprovalConfiguration : IEntityTypeConfiguration<TicketApprovalEntity>
    {
        public void Configure(EntityTypeBuilder<TicketApprovalEntity> builder)
        {
            builder.ToTable("TicketsApprovals");
            builder.HasKey(t => t.Id);
            builder.Property(t => t.Status)
                .HasDefaultValue("Новая")
                .IsRequired();
            builder.Property(t => t.Iteration)
                .IsRequired();
            builder.Property(t => t.NumberQueue)
                .IsRequired();
            builder.Property(t => t.ModifiedDate)
                .IsRequired();
        }
    }
}
