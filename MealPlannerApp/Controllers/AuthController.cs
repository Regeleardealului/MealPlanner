using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using MealPlannerApp.Shared.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace MealPlannerApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IConfiguration _config;

        public AuthController(UserManager<IdentityUser> userManager, IConfiguration config)
        {
            _userManager = userManager;
            _config = config;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserRegisterDto userDto)
        {
            // ITT A VÁLTOZÁS: A UserName most már a userDto.Username lesz, az Email pedig az Email!
            var user = new IdentityUser { UserName = userDto.Username, Email = userDto.Email };
            var result = await _userManager.CreateAsync(user, userDto.Password);

            if (!result.Succeeded)
                return BadRequest(new AuthResponseDto { IsAuthSuccessful = false, ErrorMessage = string.Join(", ", result.Errors.Select(e => e.Description)) });

            return Ok(new AuthResponseDto { IsAuthSuccessful = true });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginDto userDto)
        {
            var user = await _userManager.FindByEmailAsync(userDto.Email);
            if (user == null || !await _userManager.CheckPasswordAsync(user, userDto.Password))
                return Unauthorized(new AuthResponseDto { IsAuthSuccessful = false, ErrorMessage = "Hibás felhasználónév vagy jelszó!" });

            var token = GenerateJwtToken(user);
            return Ok(new AuthResponseDto { IsAuthSuccessful = true, Token = token });
        }

        private string GenerateJwtToken(IdentityUser user)
        {
            var claims = new[]
            {
                // Mivel a Register-nél beállítottuk a rendes UserName-t, ez a Claim azt fogja tartalmazni!
                new Claim(ClaimTypes.Name, user.UserName!),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Email, user.Email!)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("EzEgyNagyonHosszuEsBiztonsagosTitkosKulcs1234567890!"));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: "MealPlannerApp",
                audience: "MealPlannerApp",
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}