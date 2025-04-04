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
}
