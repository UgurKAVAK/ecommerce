using API.Data;
using API.Dto;
using API.Entity;
using API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<AppUser>  _userManager;
        private readonly TokenService _tokenService;
        private readonly DataContext _context;

        public AccountController(UserManager<AppUser> userManager, TokenService tokenService, DataContext context)
        {
            _userManager = userManager;
            _tokenService = tokenService;
            _context = context;
        }

        private async Task<Card> GetOrCreate(string custId)
        {
            var card = await _context.Cards.Include(i => i.CardItems).ThenInclude(i => i.Product).Where(i => i.CustomerId == custId).FirstOrDefaultAsync();
            if (card == null)
            {
                var customerId = User.Identity?.Name;
                if (string.IsNullOrEmpty(customerId))
                {
                    customerId = Guid.NewGuid().ToString();
                    var cookieOptions = new CookieOptions
                    {
                        Expires = DateTime.Now.AddMonths(1),
                        IsEssential = true
                    };
                    Response.Cookies.Append("customerId", customerId, cookieOptions);
                }

                card = new Card { CustomerId = customerId };
                _context.Cards.Add(card);
                await _context.SaveChangesAsync();
            }
            return card;
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
        {
            var user = await _userManager.FindByNameAsync(loginDto.UserName);
            if (user == null)
            {
                return BadRequest(new ProblemDetails { Title= $"{loginDto.UserName} Kullanıcı Adına Ait Kullanıcı Bulunamamıştır." });
            }
            var result = await _userManager.CheckPasswordAsync(user, loginDto.Password);
            if (result)
            {
                var userCard = await GetOrCreate(loginDto.UserName);
                var cookieCard = await GetOrCreate(Request.Cookies["customerId"]!);
                if (userCard != null)
                {
                    foreach (var item in userCard.CardItems)
                    {
                        cookieCard.AddItem(item.Product, item.Quantity);
                    }
                    _context.Cards.Remove(userCard);
                }
                cookieCard.CustomerId = loginDto.UserName;
                await _context.SaveChangesAsync();
                return Ok(new UserDto { Name = user.Name!, Token = await _tokenService.GenerateToken(user) });
            }
            return Unauthorized();
        }

        [HttpPost("register")]
        public async Task<IActionResult> CreateUser(RegisterDto registerDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var user = new AppUser
            {
                Name = registerDto.Name,
                UserName = registerDto.UserName,
                Email = registerDto.Email
            };
            var result = await _userManager.CreateAsync(user, registerDto.Password);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "Customer");
                return StatusCode(201);
            }
            return BadRequest(result.Errors);
        }

        [Authorize]
        [HttpGet("getuser")]
        public async Task<ActionResult<UserDto>> GetUser()
        {
            var user = await _userManager.FindByNameAsync(User.Identity?.Name!);
            if (user == null)
            {
                return BadRequest(new ProblemDetails { Title = "Username or Password Incorrect" });
            }
            return new UserDto { Name = user.Name!, Token = await _tokenService.GenerateToken(user) };
        }

    }
}
