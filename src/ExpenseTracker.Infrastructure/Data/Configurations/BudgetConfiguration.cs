
using ExpenseTracker.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ExpenseTracker.Infrastructure.Data.Configurations
{
    public class BudgetConfiguration : IEntityTypeConfiguration<Budget>
    {
        public void Configure(EntityTypeBuilder<Budget> builder)
        {
            builder.HasKey(b => b.Id);

            builder.Property(b => b.Amount)
                .HasPrecision(18, 2)
                .IsRequired();

            builder.ToTable(t =>
            {
                t.HasCheckConstraint(
                    "CK_Budgets_Amount",
                    "\"Amount\" > 0");

                t.HasCheckConstraint(
                    "CK_Budgets_Month",
                    "\"Month\" BETWEEN 1 AND 12");

                t.HasCheckConstraint(
                    "CK_Budgets_Year",
                    "\"Year\" >= 1");
            });

            builder.Property(b => b.Month)
                .IsRequired();

            builder.Property(b => b.Year)
                .IsRequired();

            builder.HasOne(b => b.Category)
                .WithMany()
                .HasForeignKey(b => b.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            // Один бюджет на категорию в конкретном месяце и году.
            builder.HasIndex(b => new
            {
                b.CategoryId,
                b.Month,
                b.Year
            }).IsUnique();
        }
    }
}
