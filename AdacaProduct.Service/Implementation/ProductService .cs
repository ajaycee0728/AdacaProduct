using AdacaProduct.Model;
using AdacaProduct.Service.Interface;
using Microsoft.Extensions.Caching.Memory;

namespace AdacaProduct.Service.Implementation
{
    public class ProductService : IProductService
    {
        private readonly AppDBContext _context;
        private readonly IMemoryCache _cache;
        private readonly string _cacheKey = "products_cache";

        public ProductService(AppDBContext context, IMemoryCache cache)
        {
            _context = context;
            _cache = cache;
        }

        public IEnumerable<Product> GetAll()
        {
            if (!_cache.TryGetValue(_cacheKey, out List<Product> products))
            {
                products = _context.Products.ToList();

                var cacheOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromMinutes(5)); 

                _cache.Set(_cacheKey, products, cacheOptions);
            }

            return products;
        }

        public Product? GetById(int id)
        {
            return _context.Products.Find(id);
        }

        public Product Add(Product product)
        {
            _context.Products.Add(product);
            _context.SaveChanges();

            _cache.Remove(_cacheKey); 
            return product;
        }
    }
}
