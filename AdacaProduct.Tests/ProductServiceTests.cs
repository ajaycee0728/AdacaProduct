using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using AdacaProduct.API.Controllers;
using AdacaProduct.Model;
using AdacaProduct.Model.Command;
using AdacaProduct.Model.Query;
using AdacaProduct.Service.Interface;
using System.Diagnostics;
public class ProductsControllerUnitTests
{
    private readonly Mock<IProductQueryHandler<GetProductsQuery, List<Product>>> _mockGetProductsQueryHandler;
    private readonly Mock<IProductQueryHandler<GetProductByIdQuery, Product?>> _mockGetProductByIdQueryHandler;
    private readonly Mock<ICommandHandler<AddProductCommand, Product?>> _mockProductCommandHandler;
    private readonly ProductsController _controller;

    public ProductsControllerUnitTests()
    {
        _mockGetProductsQueryHandler = new Mock<IProductQueryHandler<GetProductsQuery, List<Product>>>();
        _mockGetProductByIdQueryHandler = new Mock<IProductQueryHandler<GetProductByIdQuery, Product?>>();
        _mockProductCommandHandler = new Mock<ICommandHandler<AddProductCommand, Product?>>();
        _controller = new ProductsController(
            _mockGetProductsQueryHandler.Object,
            _mockGetProductByIdQueryHandler.Object,
            _mockProductCommandHandler.Object);
    }

    [Fact]
    public async Task GetAll_ReturnsOkResult_WithProducts()
    { 
        var products = new List<Product> { new Product { Id = 1, Name = "Test" } };
        _mockGetProductsQueryHandler.Setup(handler => handler.HandleGetProducts(It.IsAny<GetProductsQuery>()))
            .ReturnsAsync(products);
         
        var result = await _controller.GetAll(null, null, false, 1, 10);
         
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedProducts = Assert.IsAssignableFrom<IEnumerable<Product>>(okResult.Value);
        Assert.NotEmpty(returnedProducts);
    } 

    [Fact]
    public async Task GetById_NonExistingId_ReturnsNotFoundResult()
    { 
        int nonExistingId = 99;
        _mockGetProductByIdQueryHandler.Setup(handler => handler.HandleGetById(It.Is<GetProductByIdQuery>(q => q.Id == nonExistingId)));


        var result = await _controller.GetById(nonExistingId);
         
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task Create_ValidCommand_ReturnsCreatedAtAction_WithCreatedProduct()
    { 
        var command = new AddProductCommand { Name = "New Product", Description = "Description", Price = 10.00M };
        var createdProduct = new Product { Id = 5, Name = "New Product", Description = "Description", Price = 10.00M };
        _mockProductCommandHandler.Setup(handler => handler.Handle(command))
            .ReturnsAsync(createdProduct);
         
        var result = await _controller.Create(command);
         
        var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result);
        Assert.Equal(nameof(ProductsController.GetById), createdAtActionResult.ActionName);
        Assert.Equal(createdProduct.Id, createdAtActionResult.RouteValues["id"]);
        Assert.Equal(createdProduct, createdAtActionResult.Value);
    }


    [Fact]
    public async Task Create_InvalidCommand_ReturnsBadRequestResult()
    { 
        var command = new AddProductCommand { Name = "Invalid", Price = 5.00M };
        _controller.ModelState.AddModelError("Description", "Description is required.");
         
        var result = await _controller.Create(command);
         
        Assert.IsType<BadRequestObjectResult>(result);
    }
}