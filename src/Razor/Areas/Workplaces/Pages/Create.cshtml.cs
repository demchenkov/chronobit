using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Razor.Data;
using Razor.Models;

namespace Razor.Areas.Workplaces.Pages;

public class CreateModel : PageModel
{
  private readonly ApplicationDbContext _context;

  public CreateModel(ApplicationDbContext context)
  {
    _context = context;
  }

  public IActionResult OnGet()
  {
    return Page();
  }

  [BindProperty]
  public Workplace Workplace { get; set; } = default!;

  // For more information, see https://aka.ms/RazorPagesCRUD.
  public async Task<IActionResult> OnPostAsync()
  {
    if (!ModelState.IsValid)
    {
      return Page();
    }

    _context.Workplaces.Add(Workplace);
    await _context.SaveChangesAsync();

    return RedirectToPage("./Index");
  }
}