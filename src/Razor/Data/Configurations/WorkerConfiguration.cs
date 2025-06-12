using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Razor.Models;

namespace Razor.Data.Configurations;

public class WorkerConfiguration : IEntityTypeConfiguration<Worker>
{
  public void Configure(EntityTypeBuilder<Worker> builder)
  {
    builder.HasKey(x => new { x.WorkplaceId, x.UserId });

    builder.Property(x => x.AllowedActions).IsRequired();
    builder.Property(x => x.Badge);

    // Configure many-to-one relationship with User
    builder.HasOne<ApplicationUser>()
           .WithMany()
           .HasForeignKey(x => x.UserId)
           .OnDelete(DeleteBehavior.Cascade);

    // Configure many-to-one relationship with Workplace
    // This is the other side of the relationship defined in WorkplaceConfiguration
    builder.HasOne(x => x.Workplace)
           .WithMany(x => x.Workers)
           .HasForeignKey(x => x.WorkplaceId)
           .OnDelete(DeleteBehavior.Cascade);
  }
}
