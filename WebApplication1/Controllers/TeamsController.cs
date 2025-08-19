// Controllers/TeamsController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Models;
[Authorize]
public class TeamsController : Controller
{
    private readonly AppDbContext _db;
    public TeamsController(AppDbContext db) => _db = db;

    // GET: /Teams
    public async Task<IActionResult> Index(string? q, string? role)
    {
        var query = _db.users.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(q))
            query = query.Where(u => u.Name.Contains(q) || u.Email.Contains(q) || (u.Phone ?? "").Contains(q));

        if (!string.IsNullOrWhiteSpace(role))
            query = query.Where(u => u.Role == role);

        var roles = await _db.users
            .Select(u => u.Role!)
            .Where(r => r != null && r != "")
            .Distinct()
            .OrderBy(r => r)
            .Select(r => new SelectListItem { Value = r, Text = r })
            .ToListAsync();

        var vm = new TeamIndexVM
        {
            Q = q,
            Role = role,
            RoleOptions = roles,
            Items = await query
                .OrderBy(u => u.Name)
                .Select(u => new TeamRowVM
                {
                    Id = u.Id,
                    FullName = u.Name,
                    Role = u.Role,
                    Email = u.Email,
                    Phone = u.Phone,
                    JoinedAt = u.RegisteredDate
                })
                .ToListAsync()
        };

        return View(vm);
    }

    // OPTIONAL: update role/phone quickly
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Update(int id, string? role, string? phone)
    {
        var u = await _db.users.FindAsync(id);
        if (u is null) return NotFound();
        if (role != null) u.Role = role;
        if (phone != null) u.Phone = phone;
        await _db.SaveChangesAsync();
        TempData["ok"] = "Member updated.";
        return RedirectToAction(nameof(Index));
    }
}
