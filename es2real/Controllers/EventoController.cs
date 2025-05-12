using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ES2Real.Data;
using ES2Real.Models;
using ES2Real.Components.Services;

namespace ES2Real.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EventoController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly BilheteService _bilheteService;

        public EventoController(ApplicationDbContext context, BilheteService bilheteService)
        {
            _context = context;
            _bilheteService = bilheteService;
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

            // Criar bilhetes associados
            var bilheteNormal = _bilheteService.CriarBilhete(TipoBilhete.Normal);
            var bilheteVip = _bilheteService.CriarBilhete(TipoBilhete.VIP);

            // Associa manualmente o evento (caso uses navegação)
            bilheteNormal.idEvento = evento.Id;
            bilheteVip.idEvento = evento.Id; 

            _context.Bilhetes.AddRange(bilheteNormal, bilheteVip);
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
                return BadRequest("ID do evento não corresponde.");

            var eventoExistente = await _context.Eventos.FindAsync(id);
            if (eventoExistente == null)
                return NotFound("Evento não encontrado.");

            eventoExistente.Nome = eventoAtualizado.Nome;
            eventoExistente.Data = DateTime.SpecifyKind(eventoAtualizado.Data, DateTimeKind.Utc);
            eventoExistente.Hora = eventoAtualizado.Hora;
            eventoExistente.Local = eventoAtualizado.Local;
            eventoExistente.Categoria = eventoAtualizado.Categoria;
            eventoExistente.Descricao = eventoAtualizado.Descricao;
            eventoExistente.CapacidadeMax = eventoAtualizado.CapacidadeMax;

            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
