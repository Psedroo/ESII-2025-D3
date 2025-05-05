namespace ES2Real.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/[controller]")]
public class EventoController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public EventoController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Evento>>> GetEventos()
    {
        return await _context.Eventos.ToListAsync();
    }

    [HttpPost]
    public async Task<ActionResult<Evento>> CriarEvento(Evento evento)
    {
        
        _context.Eventos.Add(evento);
        await _context.SaveChangesAsync();
        
        var service = new BilheteService();

        var bilheteNormal = service.CriarBilhete(TipoBilhete.Normal);
        bilheteNormal.Id = evento.Id;
        _context.Bilhetes.Add(bilheteNormal);

        var bilheteVip = service.CriarBilhete(TipoBilhete.VIP);
        bilheteVip.Id = evento.Id;
        _context.Bilhetes.Add(bilheteVip);

        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetEventos), new { id = evento.Id }, evento);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> RemoverEvento(int id)
    {
        var evento = await _context.Eventos.FindAsync(id);
        if (evento == null) return NotFound();

        _context.Eventos.Remove(evento);
        await _context.SaveChangesAsync();

        return NoContent();
    }
    
    [HttpPut("{id}")]
    public async Task<IActionResult> AtualizarEvento(int id, [FromBody] Evento eventoAtualizado)
    {
        if (id != eventoAtualizado.Id)
        {
            return BadRequest("ID do evento não corresponde.");
        }

        var eventoExistente = await _context.Eventos.FindAsync(id);
        if (eventoExistente == null)
        {
            return NotFound("Evento não encontrado.");
        }

        eventoExistente.Nome = eventoAtualizado.Nome;
        eventoExistente.Data = DateTime.SpecifyKind(eventoAtualizado.Data, DateTimeKind.Utc);
        eventoExistente.Hora = eventoAtualizado.Hora;
        eventoExistente.Local = eventoAtualizado.Local;
        eventoExistente.Categoria = eventoAtualizado.Categoria;
        eventoExistente.Descricao = eventoAtualizado.Descricao;
        eventoExistente.CapacidadeMax = eventoAtualizado.CapacidadeMax;
        eventoExistente.PrecoIngresso = eventoAtualizado.PrecoIngresso;

        await _context.SaveChangesAsync();

        return NoContent();
    }


}