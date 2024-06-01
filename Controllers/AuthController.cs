using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AuthApp1.Models;
using AuthApp1.Data;
using System.Linq;

namespace AuthApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AuthController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost("login")]
public IActionResult Login([FromBody] User user)
{
    try
    {
        // Перевірка вхідних даних
        if (user == null || string.IsNullOrEmpty(user.Username) || string.IsNullOrEmpty(user.Password))
            return BadRequest("Invalid username or password");

        // Пошук користувача в базі даних
        var dbUser = _context.Users.FirstOrDefault(u => u.Username == user.Username && u.Password == user.Password);
        if (dbUser == null)
            return Unauthorized("Invalid username or password");

        // Генерація токена
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes("YourSec12309120938120938120983012831retKey"); // Замініть на реальний секретний ключ
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, dbUser.Username)
            }),
            Expires = DateTime.UtcNow.AddDays(7),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        var tokenString = tokenHandler.WriteToken(token);

        return Ok(new { Token = tokenString });
    }
    catch (Exception ex)
    {
        // Обробка помилок
        return StatusCode(500, "Internal server error: " + ex.Message);
    }
}

        [HttpPost("register")]
        public IActionResult Register([FromBody] User newUser)
        {
            if (newUser == null)
            {
                return BadRequest("Invalid user data");
            }

            // Check if the username is already taken
            var existingUser = _context.Users.FirstOrDefault(u => u.Username == newUser.Username);
            if (existingUser != null)
            {
                return Conflict("Username already exists");
            }

            // Create a new user and add it to the database
            _context.Users.Add(newUser);
            _context.SaveChanges();

            return Ok("User registered successfully");
        }
    }
}
