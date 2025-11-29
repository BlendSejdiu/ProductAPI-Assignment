using Microsoft.EntityFrameworkCore;
using ProductAPI.DataAccess.Services.IServices;
using ProductAPI.Models.Models;

namespace ProductAPI.DataAccess.Services;

public class DbInitializer : IDbInitializer
{
    private readonly ApplicationDbContext _context;

    public DbInitializer(ApplicationDbContext context)
    {
        _context = context;
    }

    public void Initialize()
    {
        try
        {
            if (_context.Database.GetPendingMigrations().Any())
                _context.Database.Migrate();

            if (!_context.Products.AsNoTracking().Any())
            {
                _context.Products.AddRange(
                    new Product
                    {
                        Name = "Test Product 1",
                        Category = "Cookware",
                        Price = 10,
                        StockQuantity = 10,
                        CreatedAt = DateTime.UtcNow,
                        CreatedBy = "System"
                    },
                    new Product
                    {
                        Name = "Test Product 2",
                        Category = "Electrical",
                        Price = 50,
                        StockQuantity = 25,
                        CreatedAt = DateTime.UtcNow,
                        CreatedBy = "System"
                    },
                    new Product
                    {
                        Name = "Test Product 3",
                        Category = "Food",
                        Price = 18,
                        StockQuantity = 125,
                        CreatedAt = DateTime.UtcNow,
                        CreatedBy = "System"
                    }
                );

                _context.SaveChanges();
            }
        }
        catch (Exception ex)
        {
            throw;
        }
    }
}
