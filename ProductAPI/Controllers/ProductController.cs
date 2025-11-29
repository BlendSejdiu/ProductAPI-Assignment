using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductAPI.DataAccess.Services.IServices;
using ProductAPI.Models.DTO;
using System.Security.Claims;

namespace ProductAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProductController : ControllerBase
{
    #region CTOR
    private readonly IProductService _productService;
    private readonly ILogger<ProductController> _logger;
    public ProductController(IProductService service, ILogger<ProductController> logger)
    {
        _productService = service;
        _logger = logger;
    }
    #endregion

    #region Endpoints

    #region Create Product
    /// <summary>
    /// Creates a new product. Requires authorization.
    /// </summary>
    /// <param name="product">The product details to create.</param>
    /// <returns>The created product or a 400 Bad Request if creation fails.</returns>
    /// <response code="200">Returns the created product.</response>
    /// <response code="400">If the product data is invalid.</response>
    /// <response code="401">If the user is not authorized.</response>
    /// <response code="500">If an internal server error occurs.</response>
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> CreateProduct(ProductDTO product)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var claims = new ClaimsDTO 
            {
                UserId = GetUserIdFromClaims(),
                UserName = GetUsernameFromClaims()
            };    

            var result = await _productService.CreateProductAsync(product, claims);
            if (!result.Item1)
                return BadRequest(result.Item2);

            _logger.LogInformation("Product created - Name: {Name} by User: {UserId}", product.Name, GetUserIdFromClaims());

            return Ok(result.Item2);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in CreateProduct for Name : {Name}", product?.Name);
            return StatusCode(500, "Internal server error");
        }
    }
    #endregion

    #region Get single Product
    /// <summary>
    /// Retrieves a single product by its ID. Requires authorization.
    /// </summary>
    /// <param name="Id">The ID of the product to retrieve.</param>
    /// <returns>The requested product, or 404 if not found.</returns>
    /// <response code="200">Returns the product.</response>
    /// <response code="404">If the product does not exist.</response>
    /// <response code="401">If the user is not authorized.</response>
    /// <response code="500">If an internal server error occurs.</response>
    [HttpGet("{Id}")]
    [Authorize]
    public async Task<IActionResult> GetSingleProduct(int Id)
    {
        try
        {
            var product = await _productService.GetProductByIdAsync(Id);
            if (product is null)
                return NotFound("The product doesnt exist!");

            return Ok(product);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in GetSingleProduct for Id {Id}", Id);
            return StatusCode(500, "Internal server error");
        }
    }
    #endregion

    #region Get Products with Pagination and other filters
    /// <summary>
    /// Retrieves products with optional pagination, search, category, and price filters. Requires authorization.
    /// </summary>
    /// <param name="pageNumber">The page number for pagination.</param>
    /// <param name="pageSize">The number of items per page.</param>
    /// <param name="search">Optional search term for product names.</param>
    /// <param name="category">Optional category filter.</param>
    /// <param name="minPrice">Optional minimum price filter.</param>
    /// <param name="maxPrice">Optional maximum price filter.</param>
    /// <returns>A list of products matching the filters, or 404 if none found.</returns>
    /// <response code="200">Returns a list of products.</response>
    /// <response code="404">If no products match the filters.</response>
    /// <response code="401">If the user is not authorized.</response>
    /// <response code="500">If an internal server error occurs.</response>
    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetProducts([FromQuery] int pageNumber, [FromQuery] int pageSize, [FromQuery] string? search = null, 
                                                 [FromQuery]string? category = null, [FromQuery] decimal? minPrice = null, [FromQuery] decimal? maxPrice = null)
    {
        try
        {
            var product = await _productService.GetProductsAsync(pageNumber, category, minPrice, maxPrice, search, pageSize);
            if (!product.Any())
                return NotFound("No products found.");

            return Ok(product);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                    "Error in GetProducts with params: PageNumber={PageNumber}, PageSize={PageSize}, Search={Search}," +
                    " Category={Category}, MinPrice={MinPrice}, MaxPrice={MaxPrice}",
                    pageNumber, pageSize, search, category, minPrice, maxPrice);
            return StatusCode(500, "Internal server error");
        }
    }
    #endregion

    #region Update Product
    /// <summary>
    /// Updates an existing product by its ID. Requires authorization.
    /// </summary>
    /// <param name="Id">The ID of the product to update.</param>
    /// <param name="product">The updated product data.</param>
    /// <returns>No content if successful, or 404 if product not found.</returns>
    /// <response code="204">Product updated successfully.</response>
    /// <response code="404">If the product does not exist.</response>
    /// <response code="401">If the user is not authorized.</response>
    /// <response code="500">If an internal server error occurs.</response>
    [HttpPut("{Id}")]
    [Authorize]
    public async Task<IActionResult> UpdateProduct(int Id, ProductDTO product)
    {
        try
        {
            var claims = new ClaimsDTO
            {
                UserName = GetUsernameFromClaims()
            };

            var result = await _productService.UpdateProductAsync(Id, product, claims);
            if (!result.Item1)
                return NotFound(result.Item2);

            _logger.LogInformation("Product updated - Id : {Id} by User : {UserId}", Id, GetUserIdFromClaims());

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in UpdateProduct for Id : {Id}", Id);
            return StatusCode(500, "Internal server error");
        }
    }
    #endregion

    #region Delete Product
    /// <summary>
    /// Deletes a product by its ID. Requires authorization.
    /// </summary>
    /// <param name="Id">The ID of the product to delete.</param>
    /// <returns>No content if deleted successfully, or 404 if product not found.</returns>
    /// <response code="204">Product deleted successfully.</response>
    /// <response code="404">If the product does not exist.</response>
    /// <response code="401">If the user is not authorized.</response>
    /// <response code="500">If an internal server error occurs.</response>
    [HttpDelete("{Id}")]
    [Authorize]
    public async Task<IActionResult> DeleteProduct(int Id)
    {
        try
        {
            var result = await _productService.DeleteProductAsync(Id);
            if (!result.Item1)
                return NotFound(result.Item2);

            _logger.LogInformation("Product deleted - Id: {Id} by User : {UserId}", Id, GetUserIdFromClaims());

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in DeleteProduct for Id {Id}", Id);
            return StatusCode(500, "Internal server error");
        }
    }
    #endregion

    #endregion

    #region Private Methods
    private Guid GetUserIdFromClaims()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (Guid.TryParse(userIdClaim, out var userId))
            return userId;

        return Guid.Empty;
    }
    private string GetUsernameFromClaims()
    {
        var usernameClaim = User.FindFirst(ClaimTypes.Name)?.Value;
        if (!string.IsNullOrEmpty(usernameClaim))
            return usernameClaim;

        return string.Empty;
    }
    #endregion
}
