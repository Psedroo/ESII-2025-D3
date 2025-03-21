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

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Username) || 
            string.IsNullOrWhiteSpace(request.Email) || 
            string.IsNullOrWhiteSpace(request.Password))
        {
            Console.WriteLine("❌ Erro: Campos obrigatórios ausentes.");
            return BadRequest(new { message = "Username, email, and password are required" });
        } 
    
        if (await _context.UsuariosAuth.AnyAsync(u => u.Email == request.Email))
            return BadRequest(new { message = "Email already in use" });
    
        if (await _context.UsuariosAuth.AnyAsync(u => u.Username == request.Username))
            return BadRequest(new { message = "Username already in use" });
    
        try
        {
            // Criar hash da senha
            using var hmac = new HMACSHA256();
            var salt = Convert.ToBase64String(hmac.Key);
            var hash = Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes(request.Password)));
    
            // Criar usuário
            var newUser = new UsuarioAuth
            {
                Username = request.Username,
                Email = request.Email,
                SenhaHash = hash,
                SenhaSalt = salt,
                TipoUsuario = "Participante"
            };
            Console.WriteLine("Olá, mundo!");
    
            _context.UsuariosAuth.Add(newUser);
            await _context.SaveChangesAsync(); // Salvar usuário primeiro
           
            // Criar participante vinculado ao usuário
            var participante = new Participante
            {
                Nome = request.Username,
                Contacto = "",
                DataNascimento = DateTime.UtcNow,
                IdUsuario = newUser.Id
            };
    
            _context.Participantes.Add(participante);
            await _context.SaveChangesAsync(); // Salvar participante
    
            return Ok(new { message = "Account created successfully" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error saving to database", error = ex.Message });
        }
    }
    

    //---------------------------------------------------------------------------------------------

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        // Validate input
        if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
        {
            return BadRequest(new { message = "Email and password are required" });
        }

        // Find user by email
        var user = await _context.UsuariosAuth
            .FirstOrDefaultAsync(u => u.Email == request.Email);

        if (user == null || !VerifyPassword(request.Password, user.SenhaHash, user.SenhaSalt))
        {
            return Unauthorized(new { message = "Invalid email or password" });
        }

        // Generate and return JWT token
        var token = GenerateJwtToken(user);
        return Ok(new { token });
    }

    private bool VerifyPassword(string inputPassword, string storedHash, string storedSalt)
    {
        using var hmac = new HMACSHA256(Convert.FromBase64String(storedSalt));
        var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(inputPassword));
        var computedHashString = Convert.ToBase64String(computedHash);
        return computedHashString == storedHash;
    }

    private string GenerateJwtToken(UsuarioAuth user)
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
    public async Task<ActionResult<IEnumerable<UsuarioAuth>>> GetUsuarios()
    {
        var usuarios = await _context.UsuariosAuth.ToListAsync();

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