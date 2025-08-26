using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebApplication1.Entities;
using WebApplication1.service;

namespace WebApplication1.Controllers
{
    [Authorize]
    public class CheckoutController : Controller
    {
        private readonly CheckoutService _service;
        public CheckoutController(CheckoutService service) => _service = service;


        int CurrentUserId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));


        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var vm = await _service.BuildSummaryAsync(CurrentUserId, deliveryFee: 0m, discount: 0m);
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Place(ShippingAddress form, decimal deliveryFee = 0m, decimal discount = 0m)
        {
            var id = await _service.PlaceOrderAsync(CurrentUserId, form, deliveryFee, discount);
            TempData["ok"] = "Order placed!";
            return RedirectToAction("Choose", "Payments", new { orderId = id });
        }

        [HttpGet]
        public async Task<IActionResult> Success(int id)
        {
            // simple confirmation
            ViewBag.OrderId = id;
            return View();
        }


    }

}
