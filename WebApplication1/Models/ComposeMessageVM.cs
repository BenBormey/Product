using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models
{
    public class ComposeMessageVM
    {
        [Required] public int ToUserId { get; set; }
        [StringLength(200)] public string? Subject { get; set; }
        [Required] public string? Body { get; set; }
        public int? ReplyToId { get; set; }
    }

}
