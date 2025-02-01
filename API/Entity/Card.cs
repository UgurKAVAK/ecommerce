namespace API.Entity
{
    public class Card
    {
        // Bir kullanıcı sepetine ürün eklediği zaman bu Card sınıfından bir nesne üretilecek.
        // Bu nesne bizim için kapsayıcı bir eleman olarak düşünüp bu nesne altında
        // Her bir satırı temsil edecek olan bir CardItem olacak.
        // A Card'ım varsa ve bu kartın altında ise 5 ürün varsa 5 tane CardItem'dan nesne türetilecek
        // ve bu nesne hangi üründen kaç tane olduğunu bize belirtecek.
        public int CardId { get; set; }
        public string CustomerId { get; set; } = null!;
        public List<CardItem> CardItems { get; set; } = new(); 
        public void AddItem(Product product, int quantity)
        {
            var item = CardItems.Where(c => c.ProductId == product.Id).FirstOrDefault();
            if (item == null)
            {
                CardItems.Add(new CardItem { Product = product, Quantity = quantity });
            }
            else
            {
                item.Quantity += quantity;
            }
        }
        public void DeleteItem(int productId, int quantity)
        {
            var item = CardItems.Where(c => c.ProductId == productId).FirstOrDefault();
            if (item == null) return;
            item.Quantity -= quantity;
            if(item.Quantity == 0)
            {
                CardItems.Remove(item);
            }
        }
    }

    public class CardItem()
    {
        public int CardItemId { get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; } = null!;
        public int CardId { get; set; }
        //public Card Card { get; set; } = null!;
        public int Quantity { get; set; } 
    }
}
