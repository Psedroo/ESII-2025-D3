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

            // GET: api/BilheteParticipante/bilhetes/participante/5
            [HttpGet("bilhetes/participante/{idParticipante}")]
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

            [HttpGet("eventos/participante/{idParticipante}")]
            public async Task<ActionResult<IEnumerable<BilheteParticipanteEventoDto>>> GetEventosDoParticipante(int idParticipante)
            {
                var eventos = await _context.BilheteParticipante
                    .Where(bp => bp.IdParticipante == idParticipante)
                    .Include(bp => bp.Bilhete)
                    .ThenInclude(b => b.Evento)
                    .Where(bp => bp.Bilhete != null && bp.Bilhete.Evento != null)
                    .Select(bp => new BilheteParticipanteEventoDto
                    {
                        IdBilhete = bp.IdBilhete,
                        Evento = new EventoDto
                        {
                            Id = bp.Bilhete.Evento.Id,
                            Nome = bp.Bilhete.Evento.Nome,
                            Data = bp.Bilhete.Evento.Data,
                            Local = bp.Bilhete.Evento.Local,
                            Categoria = bp.Bilhete.Evento.Categoria
                        }
                    })
                    .ToListAsync();

                return Ok(eventos);
            }


            // DELETE: api/BilheteParticipante/cancelar/5
            [HttpDelete("remover/{idBilhete}")]
            public IActionResult RemoverParticipante(int idBilhete)

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
        // DTO básico de bilhete-participante
        public class BilheteParticipanteDto
        {
            public int IdBilhete { get; set; }
            public int IdParticipante { get; set; }
        }
        
        public class BilheteParticipanteEventoDto
        {
            public int IdBilhete { get; set; }
            public EventoDto Evento { get; set; } = new EventoDto();
        }

        public class EventoDto
        {
            public int Id { get; set; }
            public string Nome { get; set; } = string.Empty;
            public DateTime Data { get; set; }
            public string Local { get; set; } = string.Empty;
            public string Categoria { get; set; } = string.Empty;
        }
    }
