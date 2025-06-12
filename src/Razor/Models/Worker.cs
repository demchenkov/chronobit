namespace Razor.Models;

public class Worker
{
  public required Guid WorkplaceId { get; set; }
  public required string UserId { get; set; }
  public required AdministrationAction AllowedActions { get; set; }
  public string? Badge { get; set; }

  public Workplace? Workplace { get; set; }
}
