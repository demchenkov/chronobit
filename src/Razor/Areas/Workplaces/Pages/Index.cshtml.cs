using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Razor.Data;
using Razor.Models;

namespace Razor.Areas.Workplaces.Pages;

[Authorize]
public class IndexModel : PageModel
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public IndexModel(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
      _context = context;
      _userManager = userManager;
    }

    public IList<Workplace> Workplaces { get; set; } = new List<Workplace>();

    [BindProperty]
    public required Workplace NewWorkplace { get; set; }

    public async Task OnGetAsync()
    {
      var user = await _userManager.GetUserAsync(HttpContext.User);
      Workplaces = await _context.Workplaces
        .Include(x => x.Workers)
        .Where(x => x.Workers.Any(y => y.UserId == user!.Id && y.AllowedActions.HasFlag(AdministrationAction.ViewShifts) ))
        .ToListAsync();
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
