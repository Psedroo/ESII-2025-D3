using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ES2Real.Models;

namespace ES2Real.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BilheteParticipanteController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public BilheteParticipanteController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<BilheteParticipanteEventoDto>>> GetMyEventos()
        {
            var resultado = await _context.BilheteParticipante
                .Include(bp => bp.Bilhete)
                .ThenInclude(b => b.Evento)
                .Select(bp => new BilheteParticipanteEventoDto
                {
                    IdBilhete = bp.IdBilhete,
                    Evento = bp.Bilhete.Evento
                })
                .ToListAsync();

            return Ok(resultado);
        }

        [HttpDelete("cancelar/{idBilhete}")]
        public IActionResult CancelarInscricao(int idBilhete)
        {
            var registo = _context.BilheteParticipante
                .FirstOrDefault(bp => bp.IdBilhete == idBilhete);

            if (registo == null)
                return NotFound("Inscrição não encontrada.");

            _context.BilheteParticipante.Remove(registo);

            var bilhete = _context.Bilhetes.FirstOrDefault(b => b.Id == idBilhete);
            if (bilhete != null)
                bilhete.Quantidade++;

            _context.SaveChanges();

            return NoContent();
        }
    }

    public class BilheteParticipanteEventoDto
    {
        public int IdBilhete { get; set; }
        public Evento Evento { get; set; } = null!;
    }
}