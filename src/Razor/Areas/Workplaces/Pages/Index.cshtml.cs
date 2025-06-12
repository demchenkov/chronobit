using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Razor.Data;
using Razor.Models;

namespace Razor.Areas.Workplaces.Pages;

public class IndexModel : PageModel
{
  private readonly ApplicationDbContext _context;

  public IndexModel(ApplicationDbContext context)
  {
    _context = context;
  }

  public IList<Workplace> Workplace { get;set; } = default!;

  public async Task OnGetAsync()
  {
    Workplace = await _context.Workplaces.ToListAsync();
  }
}
