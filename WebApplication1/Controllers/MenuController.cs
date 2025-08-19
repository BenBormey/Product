using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Entities;
using WebApplication1.Models;


namespace WebApplication1.Controllers
{
    [Authorize]
    public class MenuController : Controller
    {
        private readonly IWebHostEnvironment _env;
        private readonly AppDbContext _db;
        public MenuController(AppDbContext db, IWebHostEnvironment end)
        {
            this._db = db;
            this._env = end;    
        }

        public async Task<IActionResult> Index(string? q, int? categoryId)
        {
            // options for the dropdown + "All"
            var options = await _db.categories
                .OrderBy(c => c.CategoryName)
                .Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.CategoryName! })
                .ToListAsync();

            // base query
            var query = _db.products
                .AsNoTracking()
                .Include(p => p.Category)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(q))
                query = query.Where(p => p.Name.Contains(q));

            if (categoryId.HasValue && categoryId.Value > 0)
                query = query.Where(p => p.CategoryId == categoryId.Value);

            var items = await query
                .OrderByDescending(p => p.CreatedDate)
                .ToListAsync();

            var vm = new ProductIndexVM
            {
                Items = items,
                Q = q,
                CategoryId = categoryId,
                CategoryOptions = options
            };
            return View(vm);
        }
        public async Task<IActionResult> Details(int id)
        {
            var item = await _db.products.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
            return item is null ? NotFound() : View(item);
        }

        // GET: /Menu/Create

        // POST: /Menu/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateProductVM model)
        {
            if (!ModelState.IsValid)
            {
                model.CategoryOptions = await GetCategoryOptionsAsync();
                return View(model);
            }

            // --- image upload ---
            string? imagePath = null;

            if (model.ImageFile is not null && model.ImageFile.Length > 0)
            {
                // Safe web root (in case WebRootPath is null)
                var webRoot = _env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");

                var uploads = Path.Combine(webRoot, "uploads", "products");
                Directory.CreateDirectory(uploads);

                var ext = Path.GetExtension(model.ImageFile.FileName).ToLowerInvariant();
                var fileName = $"{Guid.NewGuid():N}{ext}";
                var fullPath = Path.Combine(uploads, fileName);

                await using var fs = System.IO.File.Create(fullPath);
                await model.ImageFile.CopyToAsync(fs);

                imagePath = $"/uploads/products/{fileName}";
            }

            // map & save
            var entity = new Product
            {
                Name = model.Name,
                Price = model.Price,
                Description = model.Description,
                CategoryId = model.CategoryId,
                CodeOrBarcode = model.CodeOrBarcode,
                Image = imagePath,
                CreatedDate = DateTime.UtcNow
            };

            _db.products.Add(entity);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: /Menu/Edit/5
        // GET: /Products/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var p = await _db.products.FindAsync(id);
            if (p is null) return NotFound();
            ViewBag.Categories = await _db.categories.OrderBy(c => c.CategoryName).ToListAsync();
            return View(p);
        }

        // POST: /Products/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Product model, IFormFile? ImageFile)
        {
            if (id != model.Id) return BadRequest();
            if (!ModelState.IsValid)
            {
                ViewBag.Categories = await _db.categories.OrderBy(c => c.CategoryName).ToListAsync();
                return View(model);
            }

            var p = await _db.products.FindAsync(id);
            if (p is null) return NotFound();

            // optional image upload
            if (ImageFile is { Length: > 0 })
            {
                var dir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "products");
                Directory.CreateDirectory(dir);
                var file = $"{Guid.NewGuid():N}{Path.GetExtension(ImageFile.FileName)}";
                var path = Path.Combine(dir, file);
                using var stream = System.IO.File.Create(path);
                await ImageFile.CopyToAsync(stream);
                model.Image = $"/uploads/products/{file}";
            }

            // update fields
            p.Name = model.Name;
            p.Price = model.Price;
            p.Description = model.Description;
            p.Image = model.Image;          // keep old if no upload
            p.CategoryId = model.CategoryId;

            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: /Menu/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var item = await _db.products.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
            return item is null ? NotFound() : View(item);
        }

        // POST: /Menu/DeleteConfirmed/5
        [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var item = await _db.products.FindAsync(id);
            if (item is null) return NotFound();
            _db.products.Remove(item);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        //[HttpGet]
        //public async Task<IActionResult> Create()
        //{
        //    var vm = new CreateProductVM
        //    {
        //        CategoryOptions = await _db.categories
        //            .OrderBy(c => c.CategoryName)
        //            .Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.CategoryName! })
        //            .ToListAsync()
        //    };
        //    return View(vm);
        //}

        private async Task LoadCategoriesAsync()
        {
            ViewBag.CategoryOptions = await _db.categories
                .OrderBy(c => c.CategoryName)
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.CategoryName!
                })
                .ToListAsync();
        }
        private async Task<List<SelectListItem>> GetCategoryOptionsAsync()
        {
            return await _db.categories
                .OrderBy(c => c.CategoryName)
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.CategoryName!
                })
                .ToListAsync();
        }
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var vm = new CreateProductVM
            {
                CategoryOptions = await GetCategoryOptionsAsync()
            };
            return View(vm);
        }

    }
}
