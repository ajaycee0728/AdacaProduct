using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AdacaProduct.Model.Exceptions;
using AdacaProduct.Model.Data;
using AdacaProduct.Service.Interface;
using AdacaProduct.Service.RepositoryInterface;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace AdacaProduct.Model.Query
{
    public class ProductQueryHandler :
        IProductQueryHandler<GetProductsQuery, List<Product>>,
        IProductQueryHandler<GetProductByIdQuery, Product?>
    {
        private readonly IProductRepository _productRepository;

        public ProductQueryHandler(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }
        public async Task<List<Product>> HandleGetProducts(GetProductsQuery query)
        {
            //string cacheKey = $"products_{query.SearchTerm}_{query.SortBy}_{query.SortDescending}_{query.PageNumber}_{query.PageSize}";

            //if (_cache.TryGetValue(cacheKey, out List<Product> cachedProducts))
            //{
            //    return cachedProducts; 
            //}

            //var products = _context.Products.AsQueryable();
             
            //if (!string.IsNullOrWhiteSpace(query.SearchTerm))
            //{
            //    var search = query.SearchTerm.ToLower();
            //    products = products.Where(p =>
            //        p.Name.ToLower().Contains(search) ||
            //        p.Description.ToLower().Contains(search));
            //}
             
            //if (!string.IsNullOrEmpty(query.SortBy))
            //{
            //    products = query.SortBy.ToLower() switch
            //    {
            //        "name" => query.SortDescending
            //            ? products.OrderByDescending(p => p.Name)
            //            : products.OrderBy(p => p.Name),
            //        "price" => query.SortDescending
            //            ? products.OrderByDescending(p => p.Price)
            //            : products.OrderBy(p => p.Price),
            //        _ => products
            //    };
            //}
             
            //int skip = (query.PageNumber - 1) * query.PageSize;
            //products = products.Skip(skip).Take(query.PageSize);
             
            //var result = await products.ToListAsync();
             
            //_cache.Set(cacheKey, result, TimeSpan.FromMinutes(5));

            //return result;

            return await _productRepository.GetProductsAsync(query);
        }

        public async Task<Product?> HandleGetById(GetProductByIdQuery query)
        {
            var product = await _productRepository.GetProductByIdAsync(query.Id);
            if (product == null)
            {
                throw new NotFoundException($"Product with ID {query.Id} not found.");
            }
            return product;
        }
    }
}
