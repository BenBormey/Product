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
            order.Status = "Completed"; // or "Paid"
            await _db.SaveChangesAsync();

            TempData["ok"] = "Payment confirmed.";
            return RedirectToAction("Details", "Cart", new { id = orderId }); // or success page
        }

        // STEP 2B: cash page
        [HttpGet]
        public async Task<IActionResult> Cash(int orderId, CancellationToken ct)
        {
            var order = await _db.orders.FindAsync(new object?[] { orderId }, ct);
            if (order is null) return NotFound();

            var vm = new CashVm { OrderId = orderId, Total = order.TotalPrice };
            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> ConfirmCashOnDelivery(int orderId, double CashAmount, CancellationToken ct)
        {
            var order = await _db.orders.FindAsync(new object?[] { orderId }, ct);
            if (order is null) return NotFound();
           

            // compare numbers, not strings
            if ( (decimal)CashAmount < order.TotalPrice)
            {
                TempData["error"] = "Cash amount must be at least the total price.";
                return RedirectToAction(nameof(Cash), new { orderId });
            }
            order.Status = "Completed";
       
            // ... mark paid, etc.
           
            TempData["ok"] = $"Paid successfully. Change: {CashAmount:C}";
            return RedirectToAction("Details", "Cart", new { id = orderId, CashAmount });
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
        public decimal CashAmount { get; set; }
    }

}
