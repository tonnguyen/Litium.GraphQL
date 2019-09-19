namespace Litium.GraphQL.Models
{
    public class PriceModel
    {
        public string Currency { get; set; }
        public decimal ListPrice { get; set; }
        public string FormattedPrice { get; set; }
    }
}
