using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Razor.Models;

namespace Razor.Data.Configurations;

public class ShiftConfiguration : IEntityTypeConfiguration<Shift>
{
  public void Configure(EntityTypeBuilder<Shift> builder)
  {
    builder.HasKey(x => x.Id);

    builder.Property(x => x.StartedAt).IsRequired();
    builder.Property(x => x.FinishedAt).IsRequired();

    // Configure many-to-one relationship with User
    builder.HasOne<ApplicationUser>()
           .WithMany()
           .HasForeignKey(x => x.UserId)
           .IsRequired()
           .OnDelete(DeleteBehavior.Cascade);

    // Configure many-to-one relationship with Workplace
    builder.HasOne(x => x.Workplace)
           .WithMany()
           .HasForeignKey(x => x.WorkplaceId)
           .OnDelete(DeleteBehavior.Cascade);
  }
}
