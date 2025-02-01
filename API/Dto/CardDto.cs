using API.Entity;

namespace API.Dto
{
    public class CardDto
    {
        public int CardId { get; set; }
        public string? CustomerId { get; set; }
        public List<CardItemDto> CardItems { get; set; } = new();
    }
    public class CardItemDto
    {
        public int ProductId { get; set; }
        public string? ProductName { get; set; }
        public decimal Price { get; set; }
        public string? ImageUrl { get; set; }
        public int Quantity { get; set; }
    }
}
