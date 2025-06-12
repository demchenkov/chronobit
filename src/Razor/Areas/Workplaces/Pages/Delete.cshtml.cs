using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Razor.Data;
using Razor.Models;

namespace Razor.Areas.Workplaces.Pages;

public class DeleteModel : PageModel
{
  private readonly ApplicationDbContext _context;

  public DeleteModel(ApplicationDbContext context)
  {
    _context = context;
  }

  [BindProperty]
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

  public async Task<IActionResult> OnPostAsync(Guid? id)
  {
    if (id == null)
    {
      return NotFound();
    }

    var workplace = await _context.Workplaces.FindAsync(id);
    if (workplace != null)
    {
      Workplace = workplace;
      _context.Workplaces.Remove(Workplace);
      await _context.SaveChangesAsync();
    }

    return RedirectToPage("./Index");
  }
}