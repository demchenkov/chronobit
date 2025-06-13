using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Razor.Data;
using Razor.Models;

namespace Razor.Areas.Workplaces.Pages;

[Authorize]
public class ScheduleModel : PageModel
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public ScheduleModel(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
      _context = context;
      _userManager = userManager;
    }

    public Workplace CurrentWorkplace { get; set; } = null!;
    public Worker CurrentWorker { get; set; }
    public List<Worker> Workers { get; set; } = new();
    public Dictionary<Guid, List<Shift>> WorkerShifts { get; set; } = new();
    public DateTime StartOfWeek { get; set; }
    public DateTime EndOfWeek { get; set; }

    [BindProperty]
    public Guid? ShiftId { get; set; }

    [BindProperty]
    public required Guid WorkerId { get; set; }

    [BindProperty]
    public DateOnly ShiftDate { get; set; }

    [BindProperty]
    public TimeOnly StartTime { get; set; }

    [BindProperty]
    public TimeOnly EndTime { get; set; }

    public async Task<IActionResult> OnGetAsync(int weekOffset = 0, Guid? id = null)
    {
        if (id == null)
        {
            return RedirectToPage("/Index");
        }

        var workplace = await _context.Workplaces
            .Include(w => w.Workers)
                .ThenInclude(w => w.User)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (workplace == null)
        {
            return RedirectToPage("/Index");
        }

        var currentUser = await _userManager.GetUserAsync(User);
        if (currentUser is null)
        {
          return Unauthorized();
        }

        CurrentWorkplace = workplace;
        CurrentWorker = await _context.Workers
          .FirstAsync(x => x.UserId == currentUser.Id && x.WorkplaceId == workplace.Id);

        Workers = workplace.Workers
            .OrderBy(w => w.WorkerName)
            .ToList();

        // Calculate week range
        var today = DateTimeOffset.UtcNow.Date;
        StartOfWeek = today.AddDays(-(int)today.DayOfWeek + (int)DayOfWeek.Monday).AddDays(7 * weekOffset);
        EndOfWeek = StartOfWeek.AddDays(6);

        // Load shifts for the week
        var shifts = await _context.Shifts
            .Where(s => s.WorkplaceId == id)
            .Where(s => s.StartedAt.CompareTo(StartOfWeek) >= 0 && s.StartedAt.CompareTo(EndOfWeek) <= 0)
            .ToListAsync();

        WorkerShifts = shifts.GroupBy(s => s.WorkerId)
            .ToDictionary(g => g.Key, g => g.ToList());

        return Page();
    }

    public List<Shift> GetShiftsForWorker(Guid workerId)
    {
        return WorkerShifts.TryGetValue(workerId, out var shifts) ? shifts : new List<Shift>();
    }

    public async Task<IActionResult> OnPostUpsertAsync(Guid? id)
    {
        if (id is null)
        {
            return RedirectToPage("/Index");
        }

        if (!ModelState.IsValid)
        {
            return Page();
        }

        var shiftDate = ShiftDate.ToDateTime(TimeOnly.MinValue);
        var startAt = shiftDate.Add(StartTime.ToTimeSpan());
        var endAt = shiftDate.Add(EndTime.ToTimeSpan());

        if (ShiftId.HasValue)
        {
            // Update existing shift
            var shift = await _context.Shifts.FindAsync(ShiftId.Value);
            if (shift == null)
            {
                return NotFound();
            }

            shift.WorkerId = WorkerId;
            shift.StartedAt = startAt;
            shift.FinishedAt = endAt;
        }
        else
        {
            // Create new shift
            var shift = new Shift
            {
                Id = Guid.NewGuid(),
                WorkplaceId = id.Value,
                WorkerId = WorkerId,
                StartedAt = startAt,
                FinishedAt = endAt
            };

            _context.Shifts.Add(shift);
        }

        var weekOffset = (int)(shiftDate - DateTime.Today).TotalDays / 7;

        await _context.SaveChangesAsync();
        return RedirectToPage(new {id, weekOffset });
    }

    public async Task<IActionResult> OnPostDeleteAsync(Guid? id)
    {
      if (id is null)
      {
        return RedirectToPage("/Index");
      }

      if (!ModelState.IsValid)
      {
        return Page();
      }

      if (ShiftId.HasValue)
      {
        // Update existing shift
        var shift = await _context.Shifts.FindAsync(ShiftId.Value);
        if (shift == null)
        {
          return NotFound();
        }

        _context.Shifts.Remove(shift);
      }

      await _context.SaveChangesAsync();
      return RedirectToPage(id);
    }

    public string GetPartialSerializedShift(Shift shift)
    {
      return JsonSerializer.Serialize(new
      {
        shift.Id,
        shift.StartedAt,
        shift.FinishedAt,
        shift.WorkerId,
      }, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
    }

    public string GetSerializedShift(Shift shift)
    {
      return JsonSerializer.Serialize(new
      {
        shift.Id,
        shift.StartedAt,
        shift.FinishedAt,
        shift.WorkerId,
      }, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
    }

    public string GetPartialSerializedShift(DateTime day, Guid workerId)
    {
      return JsonSerializer.Serialize(new
      {
        StartedAt = day.AddHours(8),
        FinishedAt = day.AddHours(22).AddMinutes(30),
        WorkerId = workerId,
      }, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
    }
}
