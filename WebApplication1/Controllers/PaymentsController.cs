using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Entities;
using WebApplication1.Models;
using WebApplication1.Repository;
using WebApplication1.service;

namespace WebApplication1.Controllers
{
    public class PaymentsController : Controller
    {
        private readonly AppDbContext _db;
        private readonly IQrService _qr; // បើមានស្រាប់ សម្រាប់បង្កើត QR data URL

        public PaymentsController(AppDbContext db, IQrService qr /* optional */)
        {
            _db = db;
            _qr = qr;
        }

        // STEP 1: choose method
        [HttpGet]
        public async Task<IActionResult> Choose(int orderId)
        {
            var order = await _db.orders.FindAsync(orderId);
            if (order == null) return NotFound();

            var vm = new PaymentChoiceVm
            {
                OrderId = order.Id,
                Total = order.TotalPrice
            };
            return View(vm); // Views/Payment/Choose.cshtml
        }

        // STEP 2A: bank page
        [HttpGet]
        public async Task<IActionResult> Bank(int orderId)
        {
            var order = await _db.orders.FirstOrDefaultAsync(o => o.Id == orderId);
            if (order == null) return NotFound();

            var vm = new PayVm
            {
                Order = order,
                QrImageDataUrl = _qr?.MakeQrForAmount(order.TotalPrice) ?? "" // replace with your QR logic
            };
            return View(vm); // Views/Payment/Bank.cshtml
        }

        [HttpPost]
        public async Task<IActionResult> ConfirmBankPaid(int orderId)
        {
            var order = await _db.orders.FindAsync(orderId);
            if (order == null) return NotFound();

            // TODO: create Payment row if you have one
            order.Status = "Processing"; // or "Paid"
            await _db.SaveChangesAsync();

            TempData["ok"] = "Payment confirmed.";
            return RedirectToAction("Details", "Cart", new { id = orderId }); // or success page
        }

        // STEP 2B: cash page
        [HttpGet]
        public async Task<IActionResult> Cash(int orderId)
        {
            var order = await _db.orders.FindAsync(orderId);
            if (order == null) return NotFound();

            var vm = new CashVm
            {
                OrderId = order.Id,
                Total = order.TotalPrice
            };
            return View(vm); // Views/Payment/Cash.cshtml
        }

        [HttpPost]
        public async Task<IActionResult> ConfirmCashOnDelivery(int orderId)
        {
            var order = await _db.orders.FindAsync(orderId);
            if (order == null) return NotFound();

            order.Status = "CashOnDelivery"; // keep PendingPayment if you prefer
            await _db.SaveChangesAsync();

            TempData["ok"] = "Cash on Delivery selected.";
            return RedirectToAction("Details", "Cart", new { id = orderId });
        }
    }
    public class PayVm
    {
        public Order Order { get; set; } = default!;     // Order ទាំងមូល ឬច្រើនគ្រាន់បង្ហាញ Id, TotalPrice
        public string QrImageDataUrl { get; set; } = ""; // QR Image (Base64 / Url)
    }
    public class PaymentChoiceVm
    {
        public int OrderId { get; set; }
        public decimal Total { get; set; }
    }
    public class CashVm
    {
        public int OrderId { get; set; }
        public decimal Total { get; set; }
    }

}
