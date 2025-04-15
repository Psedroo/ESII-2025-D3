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
    
    [HttpPost]
    public async Task<ActionResult<UtilizadorAuth>> CreateUser([FromBody] UtilizadorAuth newUser)
    {
        _context.UtilizadorAuth.Add(newUser);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetUsers), new { id = newUser.Id }, newUser);
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

    // GET: api/organizador/byUserId/{utilizadorId}
    [HttpGet("byUserId/{utilizadorId}")]
    public async Task<ActionResult<Organizador>> GetOrganizadorByUserId(int utilizadorId)
    {
        var organizador = await _context.Organizadores
            .FirstOrDefaultAsync(o => o.IdUsuario == utilizadorId);

        if (organizador == null)
        {
            return NotFound();
        }

        return organizador;
    }

    // PUT: api/usuario/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateUser(int id, [FromBody] UtilizadorAuth utilizador)
    {
        if (id != utilizador.Id)
        {
            return BadRequest("User ID mismatch.");
        }

        _context.Entry(utilizador).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!UserExists(id))
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }

        return NoContent();
    }

    private bool UserExists(int id)
    {
        return _context.UtilizadorAuth.Any(e => e.Id == id);
    }
}