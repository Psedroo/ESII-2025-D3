using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Logging;

public class UsuarioService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<UsuarioService> _logger;

    public UsuarioService(ApplicationDbContext context, ILogger<UsuarioService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<UsuarioAuth> RegisterUserAsync(string username, string email, string password)
    {
        try
        {
            // Secure password hashing
            using var hmac = new HMACSHA256();
            string salt = Convert.ToBase64String(hmac.Key);
            string hashedPassword = Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes(password)));

            var newUser = new UsuarioAuth
            {
                Username = username,
                Email = email,
                SenhaHash = hashedPassword,
                SenhaSalt = salt,
                TipoUsuario = "Participante" // Default value
            };

            _context.UsuariosAuth.Add(newUser);
            await _context.SaveChangesAsync();  // Save user first to generate Id

            // Ensure newUser.Id is available before creating Participante
            var participante = new Participante
            {
                Nome = username,
                Contacto = email,
                DataNascimento = DateTime.UtcNow,  // Default placeholder
                IdUsuario = newUser.Id  // This should now be properly set
            };

            _context.Participantes.Add(participante);
            await _context.SaveChangesAsync();

            return newUser;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error registering user");
            throw; // Re-throw the exception to be handled by the caller
        }
    }
}