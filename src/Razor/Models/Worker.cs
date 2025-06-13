using Razor.Data;

namespace Razor.Models;

public class Worker
{
  public Guid Id { get; set; }
  public required Guid WorkplaceId { get; set; }
  public WorkerType Type { get; set; }
  public required AdministrationAction AllowedActions { get; set; }
  public string? WorkerName { get; set; }

  public string? UserId { get; set; }
  public ApplicationUser? User { get; set; }

  public Workplace? Workplace { get; set; }
  public string DisplayName => User?.DisplayName ?? WorkerName ?? "Unknown";
}

public enum WorkerType
{
  Register,
  Unregister,
}
