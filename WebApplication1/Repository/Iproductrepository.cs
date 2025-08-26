using WebApplication1.Entities;

namespace WebApplication1.Repository
{
    public interface Iproductrepository
    {
        Task<Product> GetProduct();
        Task<Product> GetProductId(int id);

    
    }
}
