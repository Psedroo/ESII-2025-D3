using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ES2Real.Controllers;

[Route("api/usuario")]
[ApiController]
public class UtilizadorController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public UtilizadorController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<UtilizadorAuth>>> GetUsers()
    {
        var users = await _context.UtilizadorAuth.ToListAsync();
        if (!users.Any())
        {
            return NotFound("No users found.");
        }
        return Ok(users);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<UtilizadorAuth>> GetUser(int id)
    {
        var User = await _context.UtilizadorAuth.FindAsync(id);

        if (User == null)
        {
            return NotFound();
        }

        return User;
    }
    [HttpPost]  
    public async Task<ActionResult<UtilizadorAuth>> SetUser([FromBody]UtilizadorAuth utilizador)
    {
        if (utilizador == null)
        {   
            return BadRequest("Invalid user data.");
        }

        _context.UtilizadorAuth.Add(utilizador);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetUser), new { id = utilizador.Id }, utilizador);
    }
    
}