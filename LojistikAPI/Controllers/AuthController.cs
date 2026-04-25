using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace LojistikAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public AuthController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginModel login)
        {
            // Şimdilik test için kullanıcı adını "Muhammed", şifreyi "123456" yapıyoruz
            if (login.Username == "Muhammed" && login.Password == "123456")
            {
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var token = new JwtSecurityToken(
                    issuer: _configuration["Jwt:Issuer"],
                    audience: _configuration["Jwt:Audience"],
                    expires: DateTime.Now.AddDays(1),
                    signingCredentials: creds
                );

                // Bu satır bize o meşhur uzun dijital anahtarı (Token) üretir
                var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

                return Ok(new { token = tokenString });
            }

            return Unauthorized("Hatalı kullanıcı adı veya şifre!");
        }
    }

    // Giriş bilgilerini taşımak için yardımcı model
    public class LoginModel
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}