using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Razor.Data;
using Razor.Models;

namespace Razor.Areas.Workplaces.Pages;

// [Authorize]
public class DashboardModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public DashboardModel(ApplicationDbContext context)
    {
        _context = context;
    }

    public IList<Workplace> Workplaces { get; set; } = new List<Workplace>();

    [BindProperty]
    public Workplace NewWorkplace { get; set; }

    public async Task OnGetAsync()
    {
        Workplaces = await _context.Workplaces.ToListAsync();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        _context.Workplaces.Add(NewWorkplace);
        await _context.SaveChangesAsync();

        return RedirectToPage("./Dashboard");
    }
}
