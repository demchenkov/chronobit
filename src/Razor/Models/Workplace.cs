namespace Razor.Models;

public class Workplace
{
  public Guid Id { get; set; }
  public required string Name { get; set; }
  public string? Description { get; set; }

  public ICollection<Worker> Workers { get; set; } = [];
}
