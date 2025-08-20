using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models;
using WebApplication1.Models.Dashboard;
using WebApplication1.Repository;

namespace WebApplication1.Controllers
{
    [Authorize(Roles = "Admin")]
    public class DashboardController : Controller
    {
      
        private readonly IDashboardService _svc;
        public DashboardController(IDashboardService svc) => _svc = svc;


     
        [HttpGet]
        public async Task<IActionResult> Index(DateTime? from, DateTime? to)
        {

            var toDate = (to ?? DateTime.Today).Date;
            var fromDate = (from ?? toDate.AddDays(-6)).Date;
            var days = (toDate - fromDate).Days +1;

            var cards = await _svc.GetCardsAsync(toDate, days);

            var series = await _svc.GetOrdersPerDayAsync(toDate, days);

            var  doughnut = await _svc.GetOrderStatusDoughnutAsync(toDate);

            var trending = await _svc.GetTrendingAsync(toDate, top:3);

            var vm = new AnalyticsVM
            {
                From = fromDate,
                To = toDate,
                Cards = cards,
                OrdersPerDay = series,
                OrderStatus = doughnut,
                Trending = trending
            };
            return View(vm);
        }


        [HttpGet("/api/dashboard/cards")]
        public async Task<IActionResult> Cards(DateTime? to, int days = 7)
        {
            var date = (to ?? DateTime.Today).Date;
            return Json(await _svc.GetCardsAsync(date, days));
        }

        [HttpGet("/api/dashboard/orders-per-day")]
        public async Task<IActionResult> OrdersPerDay(DateTime? to, int days = 7)
        {
            var date = (to ?? DateTime.Today).Date;
            return Json(await _svc.GetOrdersPerDayAsync(date, days));
        }

        [HttpGet("/api/dashboard/status-doughnut")]
        public async Task<IActionResult> StatusDoughnut(DateTime? month)
        {
            var date = (month ?? DateTime.Today).Date;
            return Json(await _svc.GetOrderStatusDoughnutAsync(date));
        }

        [HttpGet("/api/dashboard/trending")]
        public async Task<IActionResult> Trending(DateTime? day, int top = 3)
        {
            var date = (day ?? DateTime.Today).Date;
            return Json(await _svc.GetTrendingAsync(date, top));
        }



    }
}
