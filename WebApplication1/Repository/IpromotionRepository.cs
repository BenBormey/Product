using WebApplication1.Entities;
using WebApplication1.Models.Promotion;

namespace WebApplication1.Repository
{
    public interface IpromotionRepository
    {
        Task<IEnumerable<PromotionList>> GetPromotion();
        Task<PromotionList> PromotionList(int productId);
        Task CreatePromotion(CreatePromotion proad);
        Task UpdatePromotin(PromotionList proup);
        Task DeletePromotion(int PromotionId);

    }
}
