using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Razor.Data;
using Razor.Models;

namespace Razor.Areas.Workplaces.Pages;

public class DetailsModel : PageModel
{
  private readonly ApplicationDbContext _context;

  public DetailsModel(ApplicationDbContext context)
  {
    _context = context;
  }

  public Workplace Workplace { get; set; } = default!;

  public async Task<IActionResult> OnGetAsync(Guid? id)
  {
    if (id == null)
    {
      return NotFound();
    }

    var workplace = await _context.Workplaces.FirstOrDefaultAsync(m => m.Id == id);

    if (workplace is not null)
    {
      Workplace = workplace;

      return Page();
    }

    return NotFound();
  }
}