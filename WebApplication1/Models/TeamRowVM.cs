namespace WebApplication1.Models
{
    public class TeamRowVM
    {
        public int Id { get; set; }
        public string FullName { get; set; } = default!;
        public string? Role { get; set; }
        public string Email { get; set; } = default!;
        public string? Phone { get; set; }
        public DateTime? JoinedAt { get; set; }
    }
}
