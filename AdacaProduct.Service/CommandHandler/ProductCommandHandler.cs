using AdacaProduct.Model.Command;
using AdacaProduct.Model;
using AdacaProduct.Model.Data;
using AdacaProduct.Service.Interface;
using AdacaProduct.Service.RepositoryInterface;

namespace AdacaProduct.Service.Commands
{
    public class ProductCommandHandler : ICommandHandler<AddProductCommand, Product>
    {
        private readonly IProductRepository _productRepository;

        public ProductCommandHandler(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<Product?> Handle(AddProductCommand command)
        {
            var newProduct = new Product
            {
                Name = command.Name,
                Description = command.Description,
                Price = command.Price 
            };

            await _productRepository.AddAsync(newProduct);
            return newProduct;
        }
    }
}
