using AdacaProduct.Model;
using AdacaProduct.Model.Query;

namespace AdacaProduct.Service.RepositoryInterface
{
    public interface IProductRepository
    {
        Task<List<Product>> GetProductsAsync(GetProductsQuery query);
        Task<Product?> GetProductByIdAsync(int id);
        Task AddAsync(Product product);
    }
}
