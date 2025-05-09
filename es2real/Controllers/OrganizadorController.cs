using ES2Real.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
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
        if (organizador == null)
        {
            return BadRequest("Dados inválidos.");
        }

        _context.Organizadores.Add(organizador);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetOrganizador), new { id = organizador.Id }, organizador);
    }

    // GET: api/organizador/byUserId/{utilizadorId}
    [HttpGet("byUserId/{utilizadorId}")]
    public async Task<ActionResult<Organizador>> GetOrganizadorByUserId(int utilizadorId)
    {
        var organizador = await _context.Organizadores
            .Include(o => o.Usuario)
            .FirstOrDefaultAsync(o => o.IdUsuario == utilizadorId);

        if (organizador == null)
        {
            return NotFound();
        }

        return organizador;
    }

    // PUT: api/organizador?id={id}
    [HttpPut]
    public async Task<IActionResult> AtualizarOrganizador([FromQuery] int id, [FromBody] Organizador organizadorAtualizado)
    {
        if (organizadorAtualizado == null || id <= 0)
        {
            return BadRequest("Dados inválidos.");
        }

        var organizadorExistente = await _context.Organizadores.FindAsync(id);
        if (organizadorExistente == null)
        {
            return NotFound($"Organizador com ID {id} não encontrado.");
        }

        organizadorExistente.Nome = organizadorAtualizado.Nome;
        organizadorExistente.Contacto = organizadorAtualizado.Contacto;
        organizadorExistente.DataNascimento = organizadorAtualizado.DataNascimento;

        try
        {
            await _context.SaveChangesAsync();
            return Ok("Organizador atualizado com sucesso.");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Erro ao atualizar: {ex.Message}");
        }
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
