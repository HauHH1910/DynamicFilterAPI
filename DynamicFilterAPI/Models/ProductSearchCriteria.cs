namespace DynamicFilterAPI.Models
{

    public record PriceRange(decimal? Min, decimal? Max);

    public record Category(string Name);

    public record ProductName(String Name);
    public class ProductSearchCriteria
    {

        public bool IsActive { get; set; }

        public PriceRange? Price { get; set; }

        public Category[]? Categories { get; set; }

        public ProductName[]? ProductName { get; set; }
    }
}
