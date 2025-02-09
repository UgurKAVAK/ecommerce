using API.Dto;
using API.Entity;

namespace API.Extensions
{
    public static class OrderExtensions
    {
        public static IQueryable<OrderDto> OrderToDto(this IQueryable<Order> query)
        {
            return query.Select(x => new OrderDto
            {
                Id = x.Id,
                CustomerId = x.CustomerId,
                FirstName = x.FirstName,
                LastName = x.LastName,
                PhoneNumber = x.PhoneNumber,
                AddressLine = x.AddressLine,
                City = x.City,
                DeliveryFree = x.DeliveryFree,
                SubTotal = x.SubTotal,
                OrderDate = x.OrderDate,
                OrderStatus = x.OrderStatus,
                OrderItems = x.OrderItems.Select(item => new OrderItemDto
                {
                    Id = item.Id,
                    ProductName = item.ProductName,
                    ProductId = item.ProductId,
                    ProductImage = item.ProductImage,
                    Price = item.Price,
                    Quantity = item.Quantity
                }).ToList()
            });
        }
    }
}
