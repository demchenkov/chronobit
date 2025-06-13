using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Razor.Data;
using Razor.Models;

namespace Razor.Areas.Workplaces.Pages;

[Authorize]
public class TeamModel : PageModel
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public TeamModel(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public List<Worker> Workers { get; set; } = new();
    public List<ApplicationUser> Users { get; set; } = new();
    public bool IsCreator { get; set; }
    public bool CanManageTeam => true;
    public AdministrationAction[] AllActions { get; set; } = Enum.GetValues<AdministrationAction>()
        .Where(a => a != AdministrationAction.All && a != AdministrationAction.None)
        .OrderBy(a => a == AdministrationAction.ViewShifts ? 0 : (int)a)
        .ToArray();

    [BindProperty]
    public string? UserId { get; set; }

    [BindProperty]
    public string? WorkerName { get; set; }

    [BindProperty]
    public WorkerType WorkerType { get; set; }

    [BindProperty]
    public AdministrationAction[] SelectedActions { get; set; } = Array.Empty<AdministrationAction>();


    public async Task<IActionResult> OnGetAsync(Guid? id)
    {
        if (id == null)
        {
            return RedirectToPage("/Dashboard");
        }

        var workplace = await _context.Workplaces
            .Include(w => w.Workers)
                .ThenInclude(w => w.User)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (workplace == null)
        {
            return RedirectToPage("/Dashboard");
        }

        var currentUserId = _userManager.GetUserId(User);
        var currentWorker = workplace.Workers.FirstOrDefault(w => w.UserId == currentUserId);

        if (currentWorker == null || !currentWorker.AllowedActions.HasFlag(AdministrationAction.ManageWorkers))
        {
            return Forbid();
        }

        var creator = workplace.Workers.FirstOrDefault(w => w.AllowedActions == AdministrationAction.All);
        IsCreator = creator?.UserId == currentUserId;
        Workers = workplace.Workers.ToList();

        var workerUserIds = workplace.Workers.Select(w => w.UserId);
        Users = _context.Users
          .Where(u => !workerUserIds.Contains(u.Id))
          .ToList();

        return Page();
    }

    public async Task<IActionResult> OnPostAddUserAsync(Guid id)
    {
        if (!ModelState.IsValid)
        {
            return RedirectToPage(new { id });
        }

        var workplace = await _context.Workplaces
            .Include(w => w.Workers)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (workplace == null)
        {
            return NotFound();
        }

        var currentUserId = _userManager.GetUserId(User);
        var currentWorker = workplace.Workers.FirstOrDefault(w => w.UserId == currentUserId);

        if (currentWorker == null || !currentWorker.AllowedActions.HasFlag(AdministrationAction.ManageWorkers))
        {
            return Forbid();
        }

        if (WorkerType == WorkerType.Register)
        {
            if (string.IsNullOrWhiteSpace(UserId))
            {
                ModelState.AddModelError("UserId", "User is not provided.");
                return RedirectToPage(new { id });
            }

            var user = await _userManager.FindByIdAsync(UserId);
            if (user == null)
            {
                ModelState.AddModelError("UserId", "User not found");
                return RedirectToPage(new { id });
            }

            if (workplace.Workers.Any(w => w.UserId == user.Id))
            {
                ModelState.AddModelError("Email", "User is already a member of this workplace");
                return RedirectToPage(new { id });
            }

            workplace.Workers.Add(new Worker
            {
                UserId = user.Id,
                WorkplaceId = id,
                Type = WorkerType.Register,
                AllowedActions = AdministrationAction.ViewShifts,
                User = user
            });
        }
        else
        {
            if (string.IsNullOrWhiteSpace(WorkerName))
            {
                ModelState.AddModelError("WorkerName", "Name is required for unregistered workers");
                return RedirectToPage(new { id });
            }

            workplace.Workers.Add(new Worker
            {
                WorkerName = WorkerName,
                WorkplaceId = id,
                Type = WorkerType.Unregister,
                AllowedActions = AdministrationAction.None
            });
        }

        await _context.SaveChangesAsync();
        return RedirectToPage(new { id });
    }

    public async Task<IActionResult> OnPostUpdateActionsAsync(Guid id, Guid workerId, AdministrationAction[] actions)
    {
        var workplace = await _context.Workplaces
            .Include(w => w.Workers)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (workplace == null)
        {
            return NotFound();
        }

        var currentUserId = _userManager.GetUserId(User);
        var currentWorker = workplace.Workers.FirstOrDefault(w => w.UserId == currentUserId);

        if (currentWorker == null || !currentWorker.AllowedActions.HasFlag(AdministrationAction.ManageWorkers))
        {
            return Forbid();
        }

        var worker = workplace.Workers.FirstOrDefault(w => w.Id == workerId);
        if (worker == null)
        {
            return NotFound();
        }

        // Ensure ViewShifts is included if ManageShifts is selected
        if (actions.Contains(AdministrationAction.ManageShifts) && !actions.Contains(AdministrationAction.ViewShifts))
        {
            actions = actions.Append(AdministrationAction.ViewShifts).ToArray();
        }

        worker.AllowedActions = actions.Aggregate(AdministrationAction.None, (current, action) => current | action);
        await _context.SaveChangesAsync();

        return RedirectToPage(new { id });
    }
}
