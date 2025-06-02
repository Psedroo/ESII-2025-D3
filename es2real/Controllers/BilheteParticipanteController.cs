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

        // GET: api/BilheteParticipante/participante/5
        [HttpGet("participante/{idParticipante}")]
        public async Task<ActionResult<IEnumerable<BilheteParticipanteDto>>> GetBilhetesDoParticipante(int idParticipante)
        {
            var bilhetes = await _context.BilheteParticipante
                .Where(bp => bp.IdParticipante == idParticipante)
                .Select(bp => new BilheteParticipanteDto
                {
                    IdBilhete = bp.IdBilhete,
                    IdParticipante = bp.IdParticipante
                })
                .ToListAsync();

            return Ok(bilhetes);
        }

        // DELETE: api/BilheteParticipante/cancelar/5
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

    public class BilheteParticipanteDto
    {
        public int IdBilhete { get; set; }
        public int IdParticipante { get; set; }
    }
}   