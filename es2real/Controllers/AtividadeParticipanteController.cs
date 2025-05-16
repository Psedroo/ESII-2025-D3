using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using ES2Real.Models; // Ajusta o namespace do teu projeto

namespace ES2Real.ApiControllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AtividadeParticipanteController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AtividadeParticipanteController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/AtividadeParticipante/evento/5
        [HttpGet("evento/{idEvento}")]
        public async Task<ActionResult<List<Atividade>>> GetAtividadesPorEvento(int idEvento)
        {
            var atividades = await _context.Atividades
                .Where(a => a.EventoAtividades.Any(ea => ea.IdEvento == idEvento))
                .ToListAsync();

            return atividades;
        }

        // POST: api/AtividadeParticipante/inscrever
        [HttpPost("inscrever")]
        public async Task<IActionResult> Inscrever([FromBody] InscricaoDto inscricao)
        {
            var userId = GetUserId();

            if (userId == null)
                return Unauthorized();

            bool jaInscrito = await _context.AtividadeParticipantes
                .AnyAsync(ap => ap.IdAtividade == inscricao.IdAtividade && ap.IdParticipante == userId);

            if (jaInscrito)
                return BadRequest("Já inscrito nessa atividade.");

            var atividadeParticipante = new AtividadeParticipante
            {
                IdAtividade = inscricao.IdAtividade,
                IdParticipante = userId.Value,
            };

            _context.AtividadeParticipantes.Add(atividadeParticipante);
            await _context.SaveChangesAsync();

            return Ok();
        }

        private int? GetUserId()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (int.TryParse(userIdStr, out int userId))
                    return userId;
            }
            return null;
        }
    }

    public class InscricaoDto
    {
        public int IdAtividade { get; set; }
    }
}
