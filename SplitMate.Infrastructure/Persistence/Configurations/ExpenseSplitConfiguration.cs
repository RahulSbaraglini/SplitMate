using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SplitMate.Domain.Entities;

namespace SplitMate.Infrastructure.Persistence.Configurations;

public class ExpenseSplitConfiguration : IEntityTypeConfiguration<ExpenseSplit>
{
    public void Configure(EntityTypeBuilder<ExpenseSplit> builder)
    {
        builder.HasKey(es => new { es.ExpenseId, es.UserId });

        builder.Property(es => es.Amount)
            .IsRequired()
            .HasPrecision(18, 2);
    }
}