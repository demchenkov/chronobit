using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Razor.Data;
using Razor.Models;

namespace Razor.Areas.Workplaces.Pages;

public class EditModel : PageModel
{
  private readonly ApplicationDbContext _context;

  public EditModel(ApplicationDbContext context)
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

    var workplace =  await _context.Workplaces.FirstOrDefaultAsync(m => m.Id == id);
    if (workplace == null)
    {
      return NotFound();
    }
    Workplace = workplace;
    return Page();
  }

  // To protect from overposting attacks, enable the specific properties you want to bind to.
  // For more information, see https://aka.ms/RazorPagesCRUD.
  public async Task<IActionResult> OnPostAsync()
  {
    if (!ModelState.IsValid)
    {
      return Page();
    }

    _context.Attach(Workplace).State = EntityState.Modified;

    try
    {
      await _context.SaveChangesAsync();
    }
    catch (DbUpdateConcurrencyException)
    {
      if (!WorkplaceExists(Workplace.Id))
      {
        return NotFound();
      }
      else
      {
        throw;
      }
    }

    return RedirectToPage("./Index");
  }

  private bool WorkplaceExists(Guid id)
  {
    return _context.Workplaces.Any(e => e.Id == id);
  }
}