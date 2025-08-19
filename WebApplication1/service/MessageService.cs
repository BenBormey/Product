using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Entities;

namespace WebApplication1.service
{
    public class MessageService
    {
        private readonly AppDbContext _db;
        public MessageService(AppDbContext db)
        {
            _db = db;
            
        }
        public Task<int> UnreadCountAsync(int userId, CancellationToken ct = default) =>
            _db.Messages.CountAsync(m => m.ReceiverId == userId && !m.IsRead && !m.IsArchived, ct);
        public async Task<Message> SendAsync(int senderId, int receiverId, string subject, string body, int? parentId = null, CancellationToken ct = default)
        {
            var msg = new Message { SenderId = senderId, ReceiverId = receiverId, Subject = subject, Body = body, ParentId = parentId };
            _db.Messages.Add(msg);
            await _db.SaveChangesAsync(ct);
            return msg;
        }

        public Task<List<Message>> InboxAsync(int userId, CancellationToken ct = default) =>
            _db.Messages.Where(m => m.ReceiverId == userId && !m.IsArchived)
                .OrderByDescending(m => m.CreatedAt).Take(200).ToListAsync(ct);

        public Task<List<Message>> SentAsync(int userId, CancellationToken ct = default) =>
            _db.Messages.Where(m => m.SenderId == userId)
                .OrderByDescending(m => m.CreatedAt).Take(200).ToListAsync(ct);

        public async Task<Message?> OpenAsync(int id, int userId, CancellationToken ct = default)
        {
            var msg = await _db.Messages.FindAsync(new object?[] { id }, ct);
            if (msg == null) return null;
            if (msg.ReceiverId == userId && !msg.IsRead) { msg.IsRead = true; await _db.SaveChangesAsync(ct); }
            return msg;
        }

        public async Task ArchiveAsync(int id, int userId, CancellationToken ct = default)
        {
            var msg = await _db.Messages.FindAsync(new object?[] { id }, ct);
            if (msg is null || msg.ReceiverId != userId) return;
            msg.IsArchived = true;
            await _db.SaveChangesAsync(ct);
        }
    }
}
