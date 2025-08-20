//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;
//using WebApplication1.Models;

//namespace WebApplication1.Controllers
//{
//    [Authorize]
//    public class AnalyticsController : Controller
//    {  
//        private readonly AnalyticsEfService _svc;
//        public AnalyticsController(AnalyticsEfService svc) => _svc = svc;

//        public async Task<IActionResult> Index(DateTime? from, DateTime? to, CancellationToken ct)
//        {
//            var f = (from ?? DateTime.Today.AddDays(-30)).Date;
//            var t = (to ?? DateTime.Today).Date.AddDays(1); // exclusive upper bound


//            var vm = new AnalyticsVM { From = f, To = t.AddDays(-1) };
//           vm.Kpis = await _svc.GetKpisAsync(f, t, ct);
//            //vm.Daily = await _svc.GetDailyAsync(f, t,ct);
//            //vm.TopProducts = await _svc.GetTopProductsAsync(f, t, 10, ct);
//         //   vm.ByCategory = await _svc.GetByCategoryAsync(f, t, ct);
          
//            return View(vm);
//        }
//    }

//}
