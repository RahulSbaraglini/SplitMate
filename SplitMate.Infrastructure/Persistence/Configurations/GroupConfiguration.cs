using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SplitMate.Domain.Entities;

namespace SplitMate.Infrastructure.Persistence.Configurations;

public class GroupConfiguration : IEntityTypeConfiguration<Group>
{
    public void Configure(EntityTypeBuilder<Group> builder)
    {
        builder.HasKey(g => g.Id);

        builder.Property(g => g.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(g => g.InviteCode)
            .IsRequired()
            .HasMaxLength(8);

        builder.HasIndex(g => g.InviteCode)
            .IsUnique();

        builder.HasMany(g => g.Members)
            .WithOne(m => m.Group)
            .HasForeignKey(m => m.GroupId);

        builder.HasMany(g => g.Expenses)
            .WithOne(e => e.Group)
            .HasForeignKey(e => e.GroupId);
    }
}