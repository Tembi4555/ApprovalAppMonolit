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
    public class UserConfiguration : IEntityTypeConfiguration<UserEntity>
    {
        public void Configure(EntityTypeBuilder<UserEntity> builder)
        {
            builder.ToTable("Users");
            builder.HasKey(t => t.Id);
            builder.Property(t => t.UserName)
                .HasMaxLength(125)
                .IsRequired();
            builder.Property(t => t.Password)
                .HasMaxLength(100)
                .IsRequired();
            builder.Property(t => t.PersonId);
            builder.HasIndex(t => t.UserName).IsUnique();
            builder.HasIndex(t => t.PersonId).IsUnique();
        }
    }
}
