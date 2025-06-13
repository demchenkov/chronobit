namespace Razor.Models;

public class Shift
{
  public Guid Id { get; set; }

  public required Guid WorkerId { get; set; }
  public Worker? Worker { get; set; }

  public Guid WorkplaceId { get; set; }
  public Workplace? Workplace { get; set; }

  public DateTimeOffset StartedAt { get; set; }
  public DateTimeOffset FinishedAt { get; set; }

  public TimeSpan Duration => FinishedAt - StartedAt;
}
