using AdacaProduct.Model;
using AdacaProduct.Service.Interface;
using Microsoft.AspNetCore.Mvc;

namespace AdacaProduct.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _service;

        public ProductsController(IProductService service)
        {
            _service = service;
        }

        [HttpGet]
        public ActionResult<ApiResponse<IEnumerable<Product>>> GetAll()
        {
            try
            {
                var products = _service.GetAll();
                return Ok(ApiResponse<IEnumerable<Product>>.Ok(products));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<string>.Fail($"Server error: {ex.Message}"));
            }
        }

        [HttpGet("{id}")]
        public ActionResult<ApiResponse<Product>> GetById(int id)
        {
            var product = _service.GetById(id);
            if (product == null)
                return NotFound(ApiResponse<string>.Fail("Product not found"));

            return Ok(ApiResponse<Product>.Ok(product));
        }

        [HttpPost]
        public ActionResult<ApiResponse<Product>> Add(Product product)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<string>.Fail("Invalid product data."));

            var added = _service.Add(product);
            return CreatedAtAction(nameof(GetById), new { id = added.Id }, ApiResponse<Product>.Ok(added, "Product created"));
        }
    }

}
