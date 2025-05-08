using AdacaProduct.API.Middleware;
using AdacaProduct.Model;
using AdacaProduct.Model.Command;
using AdacaProduct.Model.Query;
using AdacaProduct.Service.Commands;
using AdacaProduct.Model.Data;
using AdacaProduct.Service.Interface;
using Microsoft.EntityFrameworkCore;
using AdacaProduct.Service.QueryRepository;
using AdacaProduct.Service.RepositoryInterface; 

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer(); 
builder.Services.AddSwaggerGen();
builder.Services.AddMemoryCache(); 
builder.Services.AddScoped<IProductQueryHandler<GetProductsQuery, List<Product>>, ProductQueryHandler>();
builder.Services.AddScoped<IProductQueryHandler<GetProductByIdQuery, Product?>, ProductQueryHandler>();
builder.Services.AddScoped<ICommandHandler<AddProductCommand, Product>, ProductCommandHandler>();
builder.Services.AddScoped<IProductRepository, ProductQueryRepository>();
builder.Services.AddDbContext<AppDBContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ErrorHandlingMiddleware>();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
