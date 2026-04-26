using LojistikAPI.Data;
using LojistikAPI.Models; // EKSİK OLAN SATIR BUYDU (LoginModel'i bulmasını sağlar)
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace LojistikAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly AppDbContext _context;

        public AuthController(IConfiguration configuration, AppDbContext context)
        {
            _configuration = configuration;
            _context = context;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel login)
        {
            // ✅ Model validasyonu — [ApiController] ile otomatik 400 döner
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // ✅ ÖNCE: SingleOrDefaultAsync → birden fazla kayıt varsa exception fırlatır
            // ✅ SONRA: FirstOrDefaultAsync → daha güvenli, crash olmaz
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == login.Username);

            // ✅ BCrypt.Verify — kullanıcı null ise kısa devre (&&)
            if (user == null || !BCrypt.Net.BCrypt.Verify(login.Password, user.Password))
            {
                // ✅ Güvenlik: Hangi alanın yanlış olduğunu belirtme
                return Unauthorized(new { message = "Hatalı kullanıcı adı veya şifre!" });
            }

            var tokenString = GenerateJwtToken(user);

            return Ok(new
            {
                token = tokenString,
                role = user.Role,   // ✅ Frontend routing için rol bilgisi
                email = user.Email
            });
        }

        // ✅ Token üretimi ayrı private metoda taşındı — tek sorumluluk prensibi
        private string GenerateJwtToken(dynamic user)
        {
            // ✅ CS8604 çözümü: null ise erken exception — sessiz default tehlikelidir
            var jwtKey = _configuration["Jwt:Key"]
                ?? throw new InvalidOperationException("Jwt:Key tanımlanmamış!");

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email,          user.Email),
                new Claim(ClaimTypes.Role,           user.Role)
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                // ✅ ÖNCE: DateTime.Now  → sunucu timezone'una bağlı, hatalı token süresi riski
                // ✅ SONRA: DateTime.UtcNow → UTC standart, timezone bağımsız
                expires: DateTime.UtcNow.AddDays(1),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}