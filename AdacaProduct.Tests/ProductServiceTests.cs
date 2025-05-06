using Xunit;
using Moq;
using Microsoft.EntityFrameworkCore;
using AdacaProduct.Model;
using AdacaProduct.Service.Implementation;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Caching.Memory;
using AdacaProduct.Service.Interface;
using AdacaProduct.API.Controllers;
using Microsoft.AspNetCore.Mvc;

public class ProductServiceTests
{
    private List<Product> GetFakeProducts() => new()
    {
        new Product { Id = 1, Name = "Test1", Price = 10 },
        new Product { Id = 2, Name = "Test2", Price = 20 }
    };

    private DbContextOptions<AppDBContext> GetInMemoryOptions()
    {
        return new DbContextOptionsBuilder<AppDBContext>()
            .UseInMemoryDatabase(databaseName: "ProductCatalogTestDb")
            .Options;
    }

    [Fact]
    public void GetAll_ReturnsAllProducts()
    {
        // Arrange
        var options = GetInMemoryOptions();

        using (var context = new AppDBContext(options))
        {
            context.Products.AddRange(GetFakeProducts());
            context.SaveChanges();
        }

        using (var context = new AppDBContext(options))
        {
            var service = new ProductService(context, new MemoryCache(new MemoryCacheOptions()));
             
            var result = service.GetAll().ToList();
             
            Assert.Equal(2, result.Count);
            Assert.Equal("Test1", result[0].Name);
        }
    }

    [Fact]
    public void Add_ReturnsCreatedResult_WhenProductIsValid()
    { 
        var mockService = new Mock<IProductService>();
        var newProduct = new Product
        { 
            Name = "Test Product",
            Description = "Test Description",
            Price = 10.99m
        };

        mockService.Setup(s => s.Add(It.IsAny<Product>()))
                   .Returns(newProduct);

        var controller = new ProductsController(mockService.Object);
         
        var result = controller.Add(newProduct);
         
        var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        Assert.Equal("GetById", createdResult.ActionName);

        var response = Assert.IsType<ApiResponse<Product>>(createdResult.Value);
        Assert.True(response.Success);
        Assert.Equal("Test Product", response.Data?.Name);
    }

    [Fact]
    public void GetById_ReturnsNotFound_WhenProductDoesNotExist()
    {
        
        var mockService = new Mock<IProductService>();
        mockService.Setup(s => s.GetById(It.IsAny<int>())).Returns((Product?)null);
        var controller = new ProductsController(mockService.Object); 
        
        var result = controller.GetById(999);
         
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
        Assert.Equal(404, notFoundResult.StatusCode);
    }
}
