namespace ProductAPI.Models.DTO;

public class ProductDTO
{
    public string Name { get; set; } = string.Empty;

    public string Category { get; set; } = string.Empty;

    public decimal Price { get; set; }

    public int StockQuantity { get; set; }
    public bool InStock { get; set; }

    public string? CreatedBy { get; set; } = string.Empty;

    public DateTime? CreatedAt { get; set; }

    public string? UpdatedBy { get; set; } = string.Empty;

    public DateTime? UpdatedAt { get; set; }
}
