using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Entities;
using WebApplication1.Models.Promotion;
using WebApplication1.Repository;
using WebApplication1.service;

namespace WebApplication1.Controllers
{
    public class PromotionController : Controller
    {
        private readonly  IpromotionRepository _context;
        private readonly AppDbContext _db;
        public PromotionController(IpromotionRepository copntext, AppDbContext db)
        {
            this._context = copntext;
            _db = db;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var promos = await _context.GetPromotion();

            return View(promos); 
        }
        [HttpGet("Create")]
        public async Task<IActionResult> Create()
        {
            // ផ្តល់ product list ទៅ View
            ViewBag.Products = await _db.products
                .Select(p => new SelectListItem { Value = p.Id.ToString(), Text = p.Name })
                .ToListAsync();

            return View();
        }
        [HttpPost("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreatePromotion dto, CancellationToken ct)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Products = await _db.products
                    .Select(p => new SelectListItem { Value = p.Id.ToString(), Text = p.Name })
                    .ToListAsync();
                return View(dto);
            }

            try
            {
                await _context.CreatePromotion(dto);           // your service with validations
                TempData["Success"] = "Promotion created.";
                return RedirectToAction("Index");          // e.g., /Promotion/Index
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                ViewBag.Products = await _db.products
                    .Select(p => new SelectListItem { Value = p.Id.ToString(), Text = p.Name })
                    .ToListAsync();
                return View(dto);
            }
        }

        private async Task LoadProductsAsync()
        {
            ViewBag.Products = await _db.products
                .Select(p => new SelectListItem { Value = p.Id.ToString(), Text = p.Name })
                .ToListAsync();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int PromotionId)
        {
             await _context.DeletePromotion(PromotionId);
         

            TempData["Success"] = "Promotion deleted.";
            return RedirectToAction(nameof(Index));
        }




    }
}
