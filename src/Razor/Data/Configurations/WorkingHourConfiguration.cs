using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Razor.Models;

namespace Razor.Data.Configurations;

public class WorkingHourConfiguration : IEntityTypeConfiguration<WorkingHour>
{
  public void Configure(EntityTypeBuilder<WorkingHour> builder)
  {
    builder.HasKey(x => x.Id);

    builder.Property(x => x.DayOfWeek).IsRequired();
    builder.Property(x => x.OpenTime);
    builder.Property(x => x.CloseTime);

    // Configure many-to-one relationship with Workplace
    builder.HasOne(x => x.Workplace)
           .WithMany()
           .HasForeignKey(x => x.WorkplaceId)
           .OnDelete(DeleteBehavior.Cascade);
  }
}
