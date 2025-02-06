using API.Data;
using API.Dto;
using API.Entity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CardController : ControllerBase
    {
        private readonly DataContext _context;
        public CardController(DataContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<CardDto>> GetCard()
        {
            var card = await GetOrCreate();
            return CardToDto(card);
            //return CardToDto(await GetOrCreate());
        }

        [HttpPost]
        public async Task<ActionResult> AddItemToCard(int productId, int quantity)
        {
            var card = await GetOrCreate();
            var product = await _context.Products.FirstOrDefaultAsync(i => i.Id == productId);
            if (product == null)
            {
                return NotFound("The Product Is Not In Database");
            }
            card.AddItem(product, quantity);
            var result = await _context.SaveChangesAsync() > 0;
            if (result)
            {
                return CreatedAtAction(nameof(GetCard), CardToDto(card));
            }
            return BadRequest(new ProblemDetails { Title = "The Product Can Not Be Added To Card" });
        }

        [HttpDelete]
        public async Task<ActionResult> DeleteItemFromItem(int productId, int quantity)
        {
            var card = await GetOrCreate();
            card.DeleteItem(productId, quantity);
            var result = await _context.SaveChangesAsync() > 0;
            if (result)
            {
                return CreatedAtAction(nameof(GetCard), CardToDto(card));
            }
            return BadRequest(new ProblemDetails { Title = "Problem Removing Item From The Card" });
        }

        private async Task<Card> GetOrCreate()
        {
            var card = await _context.Cards.Include(i => i.CardItems).ThenInclude(i => i.Product).Where(i => i.CustomerId == Request.Cookies["customerId"]).FirstOrDefaultAsync();
            if (card == null)
            {
                var customerId = Guid.NewGuid().ToString();
                var cookieOptions = new CookieOptions
                {
                    Expires = DateTime.Now.AddMonths(1),
                    IsEssential = true
                };
                Response.Cookies.Append("customerId", customerId, cookieOptions);
                card = new Card { CustomerId = customerId };
                _context.Cards.Add(card);
                await _context.SaveChangesAsync();
            }
            return card;
        }

        private CardDto CardToDto(Card card)
        {
            return new CardDto
            {
                CardId = card.CardId,
                CustomerId = card.CustomerId,
                CardItems = card.CardItems.Select(item => new CardItemDto
                {
                    ProductId = item.ProductId,
                    ProductName = item.Product.Name,
                    Price = item.Product.Price,
                    Quantity = item.Quantity,
                    ImageUrl = item.Product.ImageUrl
                }).ToList()
            };
        }

    }
}
