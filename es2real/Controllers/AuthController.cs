using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;


[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IConfiguration _configuration;

    public AuthController(ApplicationDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }
    
    
    [HttpGet("exists")]
    public async Task<IActionResult> CheckUserExists([FromQuery] string? email, [FromQuery] string? username)
    {
        if (string.IsNullOrEmpty(email) && string.IsNullOrEmpty(username))
            return BadRequest(new { message = "Email or username is required." });

        bool exists = false;

        if (!string.IsNullOrEmpty(email))
        {
            exists = await _context.UtilizadorAuth.AnyAsync(u => u.Email == email);
        }
        else if (!string.IsNullOrEmpty(username))
        {
            exists = await _context.UtilizadorAuth.AnyAsync(u => u.Username == username);
        }

        return Ok(exists);  // Return just the boolean value
    }

    
    private bool VerifyPassword(string inputPassword, string storedHash, string storedSalt)
    {
        using var hmac = new HMACSHA256(Convert.FromBase64String(storedSalt));
        var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(inputPassword));
        var computedHashString = Convert.ToBase64String(computedHash);
        return computedHashString == storedHash;
    }

    private string GenerateJwtToken(UtilizadorAuth user)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.TipoUsuario) // Include user type/role if needed
        };

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],    // Optional: Add issuer in config
            audience: _configuration["Jwt:Audience"], // Optional: Add audience in config
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
    
    // 🔹 Endpoint para obter todos os utilizadores
    [HttpGet]
    public async Task<ActionResult<IEnumerable<UtilizadorAuth>>> GetUsuarios()
    {
        var usuarios = await _context.UtilizadorAuth.ToListAsync();

        if (usuarios == null || usuarios.Count == 0)
        {
            return NotFound("Nenhum utilizador encontrado.");
        }

        return Ok(usuarios);
    }
    
}



// Request models
public class RegisterRequest
{
    public string Username { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
}
//-----------------
public class CreateUsuarioRequest
{
    public string Username { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
}
//-----------------
public class LoginRequest
{
    public string Email { get; set; }
    public string Password { get; set; }
}