using Microsoft.AspNetCore.Mvc.Rendering;

namespace WebApplication1.Models
{
    public class TeamIndexVM
    {
        public string? Q { get; set; }
        public string? Role { get; set; }
        public IEnumerable<SelectListItem> RoleOptions { get; set; } = Enumerable.Empty<SelectListItem>();
        public List<TeamRowVM> Items { get; set; } = new();
    }
}
