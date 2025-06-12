namespace Razor.Models;

public class WorkingHour
{
  public int Id { get; set; }

  public DayOfWeek DayOfWeek { get; set; }

  public TimeSpan? OpenTime { get; set; }
  public TimeSpan? CloseTime { get; set; }

  public Guid WorkplaceId { get; set; }
  public Workplace? Workplace { get; set; }
}
