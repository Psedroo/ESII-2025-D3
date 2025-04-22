using ES2Real.Models;

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


    [HttpPut]
    public async Task<IActionResult> AtualizarOrganizador([FromQuery] int id,
        [FromBody] Organizador organizadorAtualizado)
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

        // Atualiza os campos desejados
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
}