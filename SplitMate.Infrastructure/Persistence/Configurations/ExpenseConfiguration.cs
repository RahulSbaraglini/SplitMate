using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SplitMate.Domain.Entities;

namespace SplitMate.Infrastructure.Persistence.Configurations;

public class ExpenseConfiguration : IEntityTypeConfiguration<Expense>
{
    public void Configure(EntityTypeBuilder<Expense> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Description)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(e => e.Amount)
            .IsRequired()
            .HasPrecision(18, 2);

        // configuramos explicitamente o relacionamento com Group
        // sem isso o EF Core não sabe como montar a foreign key corretamente
        builder.HasOne(e => e.Group)
            .WithMany(g => g.Expenses)
            .HasForeignKey(e => e.GroupId)
            .OnDelete(DeleteBehavior.Cascade);

        // configuramos o relacionamento com User (quem pagou)
        builder.HasOne(e => e.PaidBy)
            .WithMany()
            .HasForeignKey(e => e.PaidByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(e => e.Splits)
            .WithOne(s => s.Expense)
            .HasForeignKey(s => s.ExpenseId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}