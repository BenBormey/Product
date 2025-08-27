using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Entities;
using WebApplication1.Models.Promotion;
using WebApplication1.Repository;

namespace WebApplication1.service
{
    public class PromotionService : IpromotionRepository
    {
        private readonly AppDbContext _contex;
        public PromotionService(AppDbContext context)
        {
            this._contex = context;
        }

        public async Task CreatePromotion(CreatePromotion dto)
        {
            var now = DateTime.UtcNow;
            bool alreadyHasActivePromo = await _contex.promotions
    .AnyAsync(p => p.ProductId == dto.ProductId
                   && p.StartDate <= now
                   && now <= p.EndDate);

            if (alreadyHasActivePromo)
            {
                throw new InvalidOperationException("This product already has an active promotion.");
            }
            var entity = new Promotion
            {
                ProductId = dto.ProductId,
                DiscountPercent = dto.DiscountPercent,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate
            };

            _contex.promotions.Add(entity);
            await _contex.SaveChangesAsync();
        }

        public async Task DeletePromotion(int PromotionId)
        {
            var rows = await _contex.promotions
          .Where(p => p.Id == PromotionId)
          .ExecuteDeleteAsync();   // done; no SaveChanges




        }

        public async  Task<IEnumerable<PromotionList>> GetPromotion()
        {
            var now = DateTime.UtcNow;

            return await _contex.promotions
                .Where(p => p.StartDate <= now && now <= p.EndDate)
                .OrderByDescending(p => p.DiscountPercent)
                .Select(p => new PromotionList
                {
                    PromotionId = p.Id,
                    DiscountPercent = p.DiscountPercent,
                    StartDate = p.StartDate,
                    EndDate = p.EndDate,
                    ProductId = p.Product.Id,
                    ProductName = p.Product.Name!,
                    ProductPrice = p.Product.Price,
                    ProductImage = p.Product.Image
                })
                .AsNoTracking()
                .ToListAsync();
           
        }

        public Task<PromotionList> PromotionList(int productId)
        {
            throw new NotImplementedException();
        }

        public Task UpdatePromotin(PromotionList proup)
        {
            throw new NotImplementedException();
        }
    }
}
