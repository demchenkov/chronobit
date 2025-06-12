using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Razor.Models;

namespace Razor.Data.Configurations;

public class WorkplaceConfiguration : IEntityTypeConfiguration<Workplace>
{
  public void Configure(EntityTypeBuilder<Workplace> builder)
  {
    builder.HasKey(x => x.Id);
    builder.Property(x => x.Name).IsRequired();
    builder.Property(x => x.Description).HasMaxLength(500);

    // Configure one-to-many relationship with Workers
    builder.HasMany(x => x.Workers)
           .WithOne(x => x.Workplace)
           .HasForeignKey(x => x.WorkplaceId)
           .OnDelete(DeleteBehavior.Cascade);
  }
}
