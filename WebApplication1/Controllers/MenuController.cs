using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Entities;
using WebApplication1.Models.Product;


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

        [HttpGet]
        public async Task<IActionResult> Index(string? q, int? categoryId)
        {
            var now = DateTime.UtcNow;
            var term = (q ?? "").Trim();

            // ---- Category dropdown (+ All) ----
            var categoryOptions = await _db.categories
                .AsNoTracking()
                .Where(c => c.CategoryName != null)
                .OrderBy(c => c.CategoryName)
                .Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.CategoryName! })
                .ToListAsync();

            categoryOptions.Insert(0, new SelectListItem
            {
                Value = "0",
                Text = "All",
                Selected = !(categoryId.HasValue && categoryId.Value > 0)
            });

            // ---- Base query ----
            var baseQuery = _db.products
                .AsNoTracking()
                .Where(p => p.QtyInStock > 0);

            if (!string.IsNullOrWhiteSpace(term))
            {
                baseQuery = baseQuery.Where(p =>
                    EF.Functions.Like(p.Name, $"%{term}%") ||
                    (p.CodeOrBarcode != null && EF.Functions.Like(p.CodeOrBarcode, $"%{term}%"))
                );
            }

            if (categoryId.HasValue && categoryId.Value > 0)
                baseQuery = baseQuery.Where(p => p.CategoryId == categoryId.Value);

            // ---- Project to VM + active promotion (max %) ----
            var items = await baseQuery
                .OrderByDescending(p => p.CreatedDate)
                .Select(p => new ProductListItemVM
                {
                    Id = p.Id,
                    Name = p.Name!,
                    Image = p.Image,
                    CategoryName = p.Category != null ? p.Category.CategoryName! : "-",
                    Price = p.Price,
                    QtyInStock = (int)p.QtyInStock,
                    CreatedDate = (DateTime)p.CreatedDate,
                    // promotion.discount_percent is FLOAT in your schema → map to double?
                    DiscountPercent = _db.promotions
                        .Where(pr => pr.ProductId == p.Id &&
                                     pr.StartDate <= now &&
                                     now <= pr.EndDate)
                        .Max(pr => (double?)pr.DiscountPercent) ?? 0d
                })
                .ToListAsync();

            // compute final price in C# to avoid provider-cast issues
            foreach (var it in items)
            {
                it.IsOnSale = it.DiscountPercent > 0d;
                it.FinalPrice =  (decimal)it.DiscountPercent;
            }

            var vm = new ProductIndexVM
            {
                Q = term,
                CategoryId = categoryId,
                CategoryOptions = categoryOptions,
                Items = items
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
                model.CategoryOptions = await _db.categories
           .OrderBy(c => c.CategoryName)
           .Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.CategoryName! })
           .ToListAsync();
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
            var exiting = await _db.products.FirstOrDefaultAsync(p => p.CodeOrBarcode == model.CodeOrBarcode);
            if (exiting is not null)
            {
                ModelState.AddModelError(nameof(model.CodeOrBarcode), "Code or barcode already exists.");
                model.CategoryOptions = await GetCategoryOptionsAsync();
                return View(model);
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
                CreatedDate = DateTime.UtcNow,
                QtyInStock = model.QtyInStock,
            };

            _db.products.Add(entity);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: /Menu/Edit/5
        // GET: /Products/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var p = await _db.products.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
            if (p is null) return NotFound();

            var vm = new ProductEditVM
            {
                Id = p.Id,
                Name = p.Name,
                Price = p.Price,
                Description = p.Description,
                Image = p.Image,
                CategoryId = p.CategoryId,
                qty =(int) p.QtyInStock
            };

            vm.Categories = await _db.categories
                .OrderBy(c => c.CategoryName)
                .Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.CategoryName! })
                .ToListAsync();

            return View(vm);
        }

        // POST: /Products/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ProductEditVM model, IFormFile? ImageFile)
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
            p.QtyInStock = model.qty;

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
