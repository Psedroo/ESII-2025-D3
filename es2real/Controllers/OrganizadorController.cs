namespace ES2Real.Controllers;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

[Route("api/organizador")]
[ApiController]
public class OrganizadorController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public OrganizadorController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: api/organizador
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Organizador>>> GetOrganizadores()
    {
        return await _context.Organizadores.ToListAsync();
    }

    // GET: api/organizador/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<Organizador>> GetOrganizador(int id)
    {
        var organizador = await _context.Organizadores.FindAsync(id);
        if (organizador == null)
        {
            return NotFound();
        }
        return organizador;
    }

    // POST: api/organizador
    [HttpPost]
    public async Task<ActionResult<Organizador>> PostOrganizador(Organizador organizador)
    {
        _context.Organizadores.Add(organizador);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetOrganizador), new { id = organizador.Id }, organizador);
    }

    // PUT: api/organizador/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> PutOrganizador(int id, Organizador organizador)
    {
        if (id != organizador.Id)
        {
            return BadRequest();
        }

        _context.Entry(organizador).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!_context.Organizadores.Any(e => e.Id == id))
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

    // DELETE: api/organizador/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteOrganizador(int id)
    {
        var organizador = await _context.Organizadores.FindAsync(id);
        if (organizador == null)
        {
            return NotFound();
        }

        _context.Organizadores.Remove(organizador);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
