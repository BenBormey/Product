namespace WebApplication1.Entities
{
    public class Message
    {
        public int Id { get; set; }
        public int SenderId { get; set; }        // user who sends
        public int ReceiverId { get; set; }      // user who receives
        public string Subject { get; set; } = "";
        public string Body { get; set; } = "";
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsRead { get; set; }
        public bool IsArchived { get; set; }
        public int? ParentId { get; set; }       // for replies/thread

        // optional navs if you have User entity
        public User? Sender { get; set; }
        public User? Receiver { get; set; }
    }
}
