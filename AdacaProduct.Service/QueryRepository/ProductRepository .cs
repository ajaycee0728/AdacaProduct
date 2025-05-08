using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AdacaProduct.Model;
using AdacaProduct.Model.Query;
using AdacaProduct.Model.Data;
using AdacaProduct.Service.RepositoryInterface;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace AdacaProduct.Service.QueryRepository
{
    public class ProductQueryRepository : IProductRepository
    {
        private readonly AppDBContext _context;
        private readonly IMemoryCache _cache;

        public ProductQueryRepository(AppDBContext context, IMemoryCache cache)
        {
            _context = context;
            _cache = cache;
        }

        public async Task<List<Product>> GetProductsAsync(GetProductsQuery query)
        {

            string cacheKey = $"products_{query.SearchTerm}_{query.SortBy}_{query.SortDescending}_{query.PageNumber}_{query.PageSize}";

            if (_cache.TryGetValue(cacheKey, out List<Product> cachedProducts))
            {
                return cachedProducts;
            }

            var products = _context.Products.AsQueryable();

            if (!string.IsNullOrWhiteSpace(query.SearchTerm))
            {
                var search = query.SearchTerm.ToLower();
                products = products.Where(p =>
                    p.Name.ToLower().Contains(search) ||
                    p.Description.ToLower().Contains(search));
            }

            if (!string.IsNullOrEmpty(query.SortBy))
            {
                products = query.SortBy.ToLower() switch
                {
                    "name" => query.SortDescending ? products.OrderByDescending(p => p.Name) : products.OrderBy(p => p.Name),
                    "price" => query.SortDescending ? products.OrderByDescending(p => p.Price) : products.OrderBy(p => p.Price),
                    _ => products
                };
            }

            int skip = (query.PageNumber - 1) * query.PageSize;
            products = products.Skip(skip).Take(query.PageSize);

            var result = await products.ToListAsync();

            _cache.Set(cacheKey, result, TimeSpan.FromMinutes(5));

            return result;
        }

        public async Task<Product?> GetProductByIdAsync(int id)
        {
            return await _context.Products.FindAsync(id);
        }
        public async Task AddAsync(Product product)
        {
            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();
        }
    }
}