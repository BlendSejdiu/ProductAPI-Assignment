using Microsoft.EntityFrameworkCore;
using ProductAPI.DataAccess.Services.IServices;
using ProductAPI.Models.DTO;
using ProductAPI.Models.Models;

namespace ProductAPI.DataAccess.Services;

public class ProductService : IProductService
{
    #region CTOR
    private readonly ApplicationDbContext _context;
    public ProductService(ApplicationDbContext context)
    {
        _context = context;
    }
    #endregion
   

    public async Task<(bool, string)> CreateProductAsync(ProductDTO request, ClaimsDTO claims)
    {
        var product = new Product();
        if (await _context.Products.AnyAsync(x => x.Name == request.Name))
            return (false, "Product exists.");

        if (string.IsNullOrWhiteSpace(request.Name) || string.IsNullOrWhiteSpace(request.Category) || request.Price <= 0)
            return (false, "Product fields are incorrect.");

        product.Name = request.Name;
        product.Category = request.Category;
        product.Price = request.Price;
        product.StockQuantity = request.StockQuantity;
        product.CreatedBy = claims.UserName;
        product.CreatedAt = DateTime.UtcNow;

        await _context.Products.AddAsync(product);
        await _context.SaveChangesAsync();

        return (true, "Product created!"); ;
    }

    public async Task<(bool, string)> DeleteProductAsync(int Id)
    {
        var productToBeDeleted = await _context.Products.FirstOrDefaultAsync(x => x.Id == Id);
        if (productToBeDeleted is null)
            return (false, "Product does not exist.");

        _context.Products.Remove(productToBeDeleted);
        await _context.SaveChangesAsync();
        return (true, "Product deleted!");
    }

    public async Task<List<ProductDTO>> GetProductsAsync(int pageNumber, string? category = null,
                                                         decimal? minPrice = null, decimal? maxPrice = null,
                                                         string? search = null, int pageSize = 10)
    {
        if (pageNumber < 1)
            pageNumber = 1;

        var query = _context.Products.AsQueryable();

        if (!string.IsNullOrEmpty(category))
            query = query.Where(p => p.Category == category);

        if (minPrice.HasValue)
            query = query.Where(p => p.Price >= minPrice.Value);

        if (maxPrice.HasValue)
            query = query.Where(p => p.Price <= maxPrice.Value);

        if (!string.IsNullOrEmpty(search))
            query = query.Where(p => p.Name.Contains(search));

        var products = 
                    await query.OrderBy(p => p.Name)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize).Select(p => new ProductDTO
                    {
                        Name = p.Name,
                        Category = p.Category,
                        Price = p.Price,
                        StockQuantity = p.StockQuantity,
                        InStock = p.StockQuantity > 0,
                        CreatedBy = p.CreatedBy,
                        CreatedAt = p.CreatedAt,
                        UpdatedBy = p.UpdatedBy,
                        UpdatedAt = p.UpdatedAt
                    }).ToListAsync();

        if (!products.Any() && products.Count <= 0)
            return new List<ProductDTO>();

        return products;
    }

    public async Task<ProductDTO?> GetProductByIdAsync(int Id)
    {
        var productInDb = await _context.Products.FirstOrDefaultAsync(x => x.Id == Id);
        if (productInDb is null)
            return null;
        
        var product = new ProductDTO
        {
            Name = productInDb.Name,
            Category = productInDb.Category,
            Price = productInDb.Price,
            StockQuantity = productInDb.StockQuantity,
            InStock = productInDb.StockQuantity > 0,
            CreatedBy = productInDb.CreatedBy,
            CreatedAt = productInDb.CreatedAt,
            UpdatedBy = productInDb.UpdatedBy,
            UpdatedAt = productInDb.UpdatedAt
        };
        return product;
    }
    public async Task<(bool, string)> UpdateProductAsync(int Id, ProductDTO request, ClaimsDTO claims)
    {
        var productToBeUpdated = await _context.Products.FirstOrDefaultAsync(x => x.Id == Id);
        if (productToBeUpdated is null)
            return (false, "Product does not exist.");

        if (productToBeUpdated.Name == request.Name)
            return (false, "A product with this name already exists.");

        if (string.IsNullOrWhiteSpace(request.Name) || string.IsNullOrWhiteSpace(request.Category) || request.Price <= 0)
            return (false, "Product fields are incorrect.");

        productToBeUpdated.Name = request.Name;
        productToBeUpdated.Category = request.Category;
        productToBeUpdated.Price = request.Price;
        productToBeUpdated.StockQuantity = request.StockQuantity;
        productToBeUpdated.UpdatedAt = DateTime.UtcNow;
        productToBeUpdated.UpdatedBy = claims.UserName;


        _context.Products.Update(productToBeUpdated);
        await _context.SaveChangesAsync();

        return (true, "Product updated!");
    }
    
}
