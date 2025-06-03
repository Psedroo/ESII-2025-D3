using ES2Real.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Route("api/participante")]
[ApiController]
public class ParticipanteController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public ParticipanteController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: api/participante
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Participante>>> GetParticipantes()
    {
        return await _context.Participantes.ToListAsync();
    }

    // GET: api/participante/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<Participante>> GetParticipante(int id)
    {
        var participante = await _context.Participantes.FindAsync(id);
        if (participante == null)
        {
            return NotFound();
        }
        return participante;
    }

    // POST: api/participante
    [HttpPost]
    public async Task<ActionResult<Participante>> PostParticipante(Participante participante)
    {
        _context.Participantes.Add(participante);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetParticipante), new { id = participante.Id }, participante);
    }
    
    // GET: api/participante/byUserId/{utilizadorId}
    [HttpGet("byUserId/{utilizadorId}")]
    public async Task<ActionResult<Participante>> GetParticipanteByUserId(int utilizadorId)
    {
        var participante = await _context.Participantes
            .Include(p => p.Utilizador)  // Incluir as informações do Utilizador
            .FirstOrDefaultAsync(p => p.IdUtilizador == utilizadorId);

        if (participante == null)
        {
            return NotFound();
        }

        return participante;
    }

    
    // DELETE: api/participante/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteParticipante(int id)
    {
        var participante = await _context.Participantes.FindAsync(id);
        if (participante == null)
        {
            return NotFound();
        }

        _context.Participantes.Remove(participante);
        await _context.SaveChangesAsync();

        return NoContent();
    }
    
    
    [HttpPut]
    public async Task<IActionResult> AtualizarParticipante([FromQuery] int id, [FromBody] Participante participanteAtualizado)
    {
        if (participanteAtualizado == null || id <= 0)
        {
            return BadRequest("Dados inválidos.");
        }

        var participanteExistente = await _context.Participantes.FindAsync(id);
        if (participanteExistente == null)
        {
            return NotFound($"Participante com ID {id} não encontrado.");
        }

        // Atualiza os campos desejados
        participanteExistente.Nome = participanteAtualizado.Nome;
        participanteExistente.Contacto = participanteAtualizado.Contacto;
        participanteExistente.DataNascimento = participanteAtualizado.DataNascimento;

        try
        {
            await _context.SaveChangesAsync();
            return Ok("Participante atualizado com sucesso.");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Erro ao atualizar: {ex.Message}");
        }
    }
    
    // GET: api/participante/inscritos-com-eventos
    [HttpGet("inscritos-com-eventos")]
    public async Task<ActionResult<IEnumerable<object>>> GetParticipantesComEventos()
    {
        try
        {
            var dados = await _context.BilheteParticipante
                .Include(bp => bp.Participante).ThenInclude(p => p.Utilizador)
                .Include(bp => bp.Bilhete).ThenInclude(b => b.Evento)
                .Select(bp => new
                {
                    ParticipanteId = bp.Participante.Id,
                    Nome = bp.Participante.Nome,
                    Email = bp.Participante.Utilizador.Email,
                    Evento = bp.Bilhete.Evento.Nome,
                    IdBilhete = bp.IdBilhete
                })
                .ToListAsync();

            return Ok(dados);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Erro ao obter inscritos: {ex.Message}");
        }
    }

}