using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using ES2Real.Models;

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

        // GET: api/AtividadeParticipante/meus
        [HttpGet("meus")]
        public async Task<ActionResult<List<int>>> GetMinhasAtividades([FromQuery] int idParticipante)
        {
            if (idParticipante == 0)
                return BadRequest("ID do participante inválido.");

            var atividades = await _context.AtividadeParticipantes
                .Where(ap => ap.IdParticipante == idParticipante)
                .Select(ap => ap.IdAtividade)
                .ToListAsync();

            if (atividades == null || atividades.Count == 0)
                return NotFound("Nenhuma atividade encontrada para o participante.");

            return Ok(atividades);
        }

        // GET: api/AtividadeParticipante/evento/5
        [HttpGet("evento/{idEvento}")]
        public async Task<ActionResult<List<Atividade>>> GetAtividadesPorEvento(int idEvento)
        {
            var atividades = await _context.Atividades
                .Where(a => a.EventoAtividades.Any(ea => ea.IdEvento == idEvento))
                .ToListAsync();

            return Ok(atividades);
        }

        // GET: api/AtividadeParticipante/participante/5
        [HttpGet("participante/{idParticipante}")]
        public async Task<ActionResult<List<AtividadeParticipanteDto>>> GetAtividadesDoParticipante(int idParticipante)
        {
            var inscricoes = await _context.AtividadeParticipantes
                .Where(ap => ap.IdParticipante == idParticipante)
                .Select(ap => new AtividadeParticipanteDto
                {
                    IdAtividade = ap.IdAtividade,
                    IdParticipante = ap.IdParticipante
                })
                .ToListAsync();

            if (inscricoes == null || inscricoes.Count == 0)
                return NotFound();

            return Ok(inscricoes);
        }

        // POST: api/AtividadeParticipante/inscrever
        [HttpPost("inscrever")]
        public async Task<IActionResult> Inscrever([FromBody] InscricaoDto inscricao)
        {
            int userId = inscricao.IdParticipante;
            Console.WriteLine($"[INFO] User ID: {userId}");
            Console.WriteLine($"[INFO] IdAtividade recebido: {inscricao.IdAtividade}");

            if (userId == 0)
                return BadRequest("ID do participante inválido.");

            // Verifica se já está inscrito
            bool jaInscrito = await _context.AtividadeParticipantes
                .AnyAsync(ap => ap.IdAtividade == inscricao.IdAtividade && ap.IdParticipante == userId);

            Console.WriteLine($"[INFO] Já inscrito? {jaInscrito}");

            if (jaInscrito)
                return BadRequest("Já inscrito nessa atividade.");

            // Buscar o evento da atividade
            var eventoId = await _context.EventoAtividades
                .Where(ea => ea.IdAtividade == inscricao.IdAtividade)
                .Select(ea => ea.IdEvento)
                .FirstOrDefaultAsync();

            Console.WriteLine($"[INFO] Evento associado à atividade: {eventoId}");

            if (eventoId == 0)
                return BadRequest("Atividade não está associada a nenhum evento.");

            // Verifica se o participante tem bilhete para o evento
            bool temBilhete = await _context.BilheteParticipante
                .Include(bp => bp.Bilhete)
                .AnyAsync(bp => bp.IdParticipante == userId && bp.Bilhete.idEvento == eventoId);

            if (!temBilhete)
                return BadRequest("Participante não possui bilhete para o evento desta atividade.");

            // Inscrever
            var atividadeParticipante = new AtividadeParticipante
            {
                IdAtividade = inscricao.IdAtividade,
                IdParticipante = userId
            };

            _context.AtividadeParticipantes.Add(atividadeParticipante);
            await _context.SaveChangesAsync();

            return Ok();
        }

        // DELETE: api/AtividadeParticipante/cancelar/{idAtividade}
        [HttpDelete("cancelar/{idAtividade}")]
        public async Task<IActionResult> CancelarInscricaoAtividade(int idAtividade, [FromQuery] int idParticipante)
        {
            if (idParticipante == 0)
                return BadRequest("ID do participante inválido.");

            var registo = await _context.AtividadeParticipantes
                .FirstOrDefaultAsync(ap => ap.IdAtividade == idAtividade && ap.IdParticipante == idParticipante);

            if (registo == null)
                return NotFound("Inscrição não encontrada.");

            _context.AtividadeParticipantes.Remove(registo);
            await _context.SaveChangesAsync();

            return NoContent();
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

        public class InscricaoDto
        {
            public int IdAtividade { get; set; }
            public int IdParticipante { get; set; }
        }

        public class AtividadeParticipanteDto
        {
            public int IdAtividade { get; set; }
            public int IdParticipante { get; set; }
        }
    }
}