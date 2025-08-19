using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models;
using WebApplication1.service;

namespace WebApplication1.Controllers
{
    // Web/Controllers/MessagesController.cs
    [Route("messages")]
    public class MessagesController : Controller
    {
        private readonly MessageService _svc;
        public MessagesController(MessageService svc) => _svc = svc;

        private int CurrentUserId => int.Parse(User.FindFirst("uid")?.Value ?? "1"); // replace with your auth

        [HttpGet("")]
        public async Task<IActionResult> Index(string folder = "inbox")
        {
            var uid = CurrentUserId;
            var model = folder.ToLower() == "sent"
                ? await _svc.SentAsync(uid)
                : await _svc.InboxAsync(uid);
            ViewBag.Folder = folder;
            return View(model);
        }

        [HttpGet("compose")]
        public IActionResult Compose(int? toUserId = null, int? replyToId = null) =>
            View(new ComposeMessageVM { ToUserId = (int)toUserId, ReplyToId = replyToId });

        [HttpPost("compose")]
        public async Task<IActionResult> Compose(ComposeMessageVM vm)
        {
            if (!ModelState.IsValid) return View(vm);
            await _svc.SendAsync(CurrentUserId, vm.ToUserId, vm.Subject ?? "", vm.Body ?? "", vm.ReplyToId);
            TempData["ok"] = "Message sent.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> Details(int id)
        {
            var msg = await _svc.OpenAsync(id, CurrentUserId);
            if (msg is null) return NotFound();
            return View(msg);
        }

        [HttpPost("{id:int}/archive")]
        public async Task<IActionResult> Archive(int id)
        {
            await _svc.ArchiveAsync(id, CurrentUserId);
            return RedirectToAction(nameof(Index));
        }
    }

}
