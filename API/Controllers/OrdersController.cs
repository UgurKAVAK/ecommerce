using API.Data;
using API.Dto;
using API.Entity;
using API.Extensions;
using Iyzipay;
using Iyzipay.Model;
using Iyzipay.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly IConfiguration _config;

        public OrdersController(DataContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        [HttpGet]
        public async Task<ActionResult<List<OrderDto>>> GetOrders()
        {
            return await _context.Orders.Include(x => x.OrderItems).OrderToDto().Where(x => x.CustomerId == User.Identity!.Name).ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<OrderDto?>> GetOrder(int id)
        {
            return await _context.Orders.Include(x => x.OrderItems).OrderToDto().Where(x => x.CustomerId == User.Identity!.Name && x.Id == id).FirstOrDefaultAsync();
        }

        [HttpPost()]
        public async Task<ActionResult<Order>> CreateOrder(CreateOrderDto createOrderDto)
        {
            var card = await _context.Cards.Include(x => x.CardItems).ThenInclude(x => x.Product).Where(x => x.CustomerId == User.Identity!.Name).FirstOrDefaultAsync();
            if (card == null)
            {
                return BadRequest(new ProblemDetails { Title = "Problem Getting Card" });
            }
            var items = new List<Entity.OrderItem>();
            foreach (var item in card.CardItems)
            {
                var product = await _context.Products.FindAsync(item.ProductId);
                var orderItem = new Entity.OrderItem
                {
                    ProductId = product!.Id,
                    ProductName = product.Name!,
                    ProductImage = product.ImageUrl!,
                    Price = product.Price,
                    Quantity = item.Quantity,
                };
                items.Add(orderItem);
                product.Stock -= item.Quantity;
            }
            var subTotal = items.Sum(x => x.Price * x.Quantity);
            var deliveryFee = 0;
            var order = new Order
            {
                OrderItems = items,
                CustomerId = User.Identity!.Name,
                FirstName = createOrderDto.FirstName,
                LastName = createOrderDto.LastName,
                PhoneNumber = createOrderDto.PhoneNumber,
                City = createOrderDto.City,
                AddressLine = createOrderDto.AddressLine,
                SubTotal = subTotal,
                DeliveryFree = deliveryFee,
            };
            // Payment 
            var paymentResult = await ProcessPayment(createOrderDto, card);

            if (paymentResult.Status == "failure")
            {
                return BadRequest(new ProblemDetails { Title = paymentResult.ErrorMessage });
            }

            order.ConversationId = paymentResult.ConversationId;
            order.BasketId = paymentResult.BasketId;

            _context.Orders.Add(order);
            _context.Cards.Remove(card);
            var result = await _context.SaveChangesAsync() > 0;
            if (result)
            {
                return CreatedAtAction(nameof(GetOrder), new { id = order.Id }, order.Id);
            }
            return BadRequest(new ProblemDetails { Title = "Problem Getting Order" });
        }

        private async Task<Payment> ProcessPayment(CreateOrderDto createOrderDto, Entity.Card card)
        {
            Options options = new Options();
            options.ApiKey = _config["PaymentAPI:APIKey"];
            options.SecretKey = _config["PaymentAPI:SecretKey"];
            options.BaseUrl = "https://sandbox-api.iyzipay.com";

            CreatePaymentRequest request = new CreatePaymentRequest();
            request.Locale = Locale.TR.ToString();
            request.ConversationId = Guid.NewGuid().ToString();
            request.Price = card.CalculateTotal().ToString();
            request.PaidPrice = card.CalculateVATTotal().ToString();
            request.Currency = Currency.TRY.ToString();
            request.Installment = 1;
            request.BasketId = card.CardId.ToString();
            request.PaymentChannel = PaymentChannel.WEB.ToString();
            request.PaymentGroup = PaymentGroup.PRODUCT.ToString();

            PaymentCard paymentCard = new PaymentCard();
            paymentCard.CardHolderName = createOrderDto.CardName;
            paymentCard.CardNumber = createOrderDto.CardNumber;
            paymentCard.ExpireMonth = createOrderDto.CardExpireMonth;
            paymentCard.ExpireYear = createOrderDto.CardExpireYear;
            paymentCard.Cvc = createOrderDto.CardCvv;
            paymentCard.RegisterCard = 0;
            request.PaymentCard = paymentCard;

            Buyer buyer = new Buyer();
            buyer.Id = "BY789";
            buyer.Name = createOrderDto.FirstName;
            buyer.Surname = createOrderDto.LastName;
            buyer.GsmNumber = createOrderDto.PhoneNumber;
            buyer.Email = "test@gmail.com";
            buyer.IdentityNumber = "74300864791";
            buyer.LastLoginDate = "2015-10-05 12:43:35";
            buyer.RegistrationDate = "2013-04-21 15:12:09";
            buyer.RegistrationAddress = createOrderDto.AddressLine;
            buyer.Ip = "85.34.78.112";
            buyer.City = createOrderDto.City;
            buyer.Country = "Turkey";
            buyer.ZipCode = "34732";
            request.Buyer = buyer;

            Address shippingAddress = new Address();
            shippingAddress.ContactName = createOrderDto.FirstName + " " + createOrderDto.LastName;
            shippingAddress.City = createOrderDto.City;
            shippingAddress.Country = "Turkey";
            shippingAddress.Description = createOrderDto.AddressLine;
            shippingAddress.ZipCode = "34742";
            request.ShippingAddress = shippingAddress;
            request.BillingAddress = shippingAddress;

            List<BasketItem> basketItems = new List<BasketItem>();

            foreach (var item in card.CardItems)
            {
                BasketItem basketItem = new BasketItem();
                basketItem.Id = item.ProductId.ToString();
                basketItem.Name = item.Product.Name;
                basketItem.Category1 = "Telefon";
                basketItem.ItemType = BasketItemType.PHYSICAL.ToString();
                basketItem.Price = ((double)item.Product.Price * item.Quantity).ToString();
                basketItems.Add(basketItem);
            }

            request.BasketItems = basketItems;

            return await Payment.Create(request, options);
        }

    }
}
