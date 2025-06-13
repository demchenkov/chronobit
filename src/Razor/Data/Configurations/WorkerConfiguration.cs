using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Razor.Models;

namespace Razor.Data.Configurations;

public class WorkerConfiguration : IEntityTypeConfiguration<Worker>
{
  public void Configure(EntityTypeBuilder<Worker> builder)
  {
    builder.HasKey(x => x.Id);

    builder.Property(x => x.UserId).HasMaxLength(255).UseCollation("NOCASE");
    builder.Property(x => x.AllowedActions).IsRequired();
    builder.Property(x => x.WorkerName).HasMaxLength(255);

    builder.Property(x => x.Id).UseCollation("NOCASE");
    builder.Property(x => x.WorkplaceId).UseCollation("NOCASE");

    // Configure many-to-one relationship with User
    builder.HasOne(x => x.User)
           .WithMany()
           .HasForeignKey(x => x.UserId)
           .OnDelete(DeleteBehavior.SetNull);

    // Configure many-to-one relationship with Workplace
    // This is the other side of the relationship defined in WorkplaceConfiguration
    builder.HasOne(x => x.Workplace)
           .WithMany(x => x.Workers)
           .HasForeignKey(x => x.WorkplaceId)
           .OnDelete(DeleteBehavior.SetNull);
  }
}
