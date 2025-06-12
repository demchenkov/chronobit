using System.Reflection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Razor.Models;

namespace Razor.Data;

public class ApplicationUser : IdentityUser
{
  public string? AvatarUri { get; set; }
  public string FirstName { get; set; } = "";
  public string LastName { get; set; } = "";
}

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
  public DbSet<Workplace> Workplaces { get; set; } = null!;
  public DbSet<Worker> Workers { get; set; } = null!;
  public DbSet<WorkingHour> WorkingHours { get; set; } = null!;
  public DbSet<Shift> Shifts { get; set; } = null!;

  public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : base(options)
  {
  }

  protected override void OnModelCreating(ModelBuilder builder)
  {
    base.OnModelCreating(builder);

    builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
  }
}
