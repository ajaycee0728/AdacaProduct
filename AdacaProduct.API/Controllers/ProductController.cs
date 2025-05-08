using AdacaProduct.Model;
using AdacaProduct.Service.Commands;
using AdacaProduct.Model.Command;
using AdacaProduct.Model.Query;
using AdacaProduct.Service.Interface;
using Microsoft.AspNetCore.Mvc;

namespace AdacaProduct.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductQueryHandler<GetProductsQuery, List<Product>> _productsQueryHandler;
        private readonly IProductQueryHandler<GetProductByIdQuery, Product?> _productByIdQueryHandler;
        private readonly ICommandHandler<AddProductCommand, Product?> _productCommand; // Assuming this is correct

        public ProductsController(
            IProductQueryHandler<GetProductsQuery, List<Product>> productsQueryHandler,
            IProductQueryHandler<GetProductByIdQuery, Product?> productByIdQueryHandler,
            ICommandHandler<AddProductCommand, Product?> productCommand)
        {
            _productsQueryHandler = productsQueryHandler;
            _productByIdQueryHandler = productByIdQueryHandler;
            _productCommand = productCommand;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] string? searchTerm,
            [FromQuery] string? sortBy,
            [FromQuery] bool sortDescending = false,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            var query = new GetProductsQuery
            {
                SearchTerm = searchTerm,
                SortBy = sortBy,
                SortDescending = sortDescending,
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            var products = await _productsQueryHandler.HandleGetProducts(query);
            return Ok(products);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var query = new GetProductByIdQuery { Id = id };
            var product = await _productsQueryHandler.HandleGetById(query);

            if (product == null)
                return NotFound();

            return Ok(product);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] AddProductCommand command)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var created = await _productCommand.Handle(command);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }
    } 
}
