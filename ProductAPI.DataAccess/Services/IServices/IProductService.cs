using ProductAPI.Models.DTO;
using ProductAPI.Models.Models;

namespace ProductAPI.DataAccess.Services.IServices;

public interface IProductService
{
    Task<List<ProductDTO>> GetProductsAsync(int pageNumber, string? category,decimal? minPrice, decimal? maxPrice,
                                            string? search, int pageSize = 10);
    Task<(bool, string)> CreateProductAsync(ProductDTO request, ClaimsDTO claims);
    Task<ProductDTO?> GetProductByIdAsync(int Id);
    Task<(bool, string)> UpdateProductAsync(int Id, ProductDTO request, ClaimsDTO claims);
    Task<(bool, string)> DeleteProductAsync(int Id);
}
