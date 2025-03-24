using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ES2Real.Controllers;

[Route("api/usuario")]
[ApiController]
public class UsuarioController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public UsuarioController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<UsuarioAuth>>> GetUsers()
    {
        var users = await _context.UsuariosAuth.ToListAsync();
        if (!users.Any())
        {
            return NotFound("No users found.");
        }
        return Ok(users);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<UsuarioAuth>> GetUser(int id)
    {
        var User = await _context.UsuariosAuth.FindAsync(id);

        if (User == null)
        {
            return NotFound();
        }

        return User;
    }
    [HttpPost]  
    public async Task<ActionResult<UsuarioAuth>> SetUser([FromBody]UsuarioAuth usuario)
    {
        if (usuario == null)
        {   
            return BadRequest("Invalid user data.");
        }

        _context.UsuariosAuth.Add(usuario);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetUser), new { id = usuario.Id }, usuario);
    }
    
}