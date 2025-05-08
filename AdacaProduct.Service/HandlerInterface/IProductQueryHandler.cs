using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AdacaProduct.Model;
using AdacaProduct.Model.Query;

namespace AdacaProduct.Service.Interface
{
    public interface IProductQueryHandler<TQuery, TResult>
    {
        Task<List<Product>> HandleGetProducts(GetProductsQuery query);
        Task<Product?> HandleGetById(GetProductByIdQuery query);
    }
}
