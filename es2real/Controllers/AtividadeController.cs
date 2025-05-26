using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ES2Real.Data;
using ES2Real.Models;

namespace ES2Real.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AtividadeController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AtividadeController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Atividade/evento/5
        [HttpGet("evento/{eventoId}")]
        public async Task<ActionResult<IEnumerable<Atividade>>> GetAtividadesPorEvento(int eventoId)
        {
            var atividades = await _context.EventoAtividades
                .Where(ea => ea.IdEvento == eventoId)
                .Include(ea => ea.Atividade)
                .Select(ea => ea.Atividade)
                .ToListAsync();

            return Ok(atividades);
        }

        // GET: api/Atividade/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Atividade>> GetAtividade(int id)
        {
            var atividade = await _context.Atividades.FindAsync(id);
            if (atividade == null)
                return NotFound();

            return Ok(atividade);
        }

        [HttpPost]
        public async Task<ActionResult<Atividade>> CriarAtividade([FromBody] Atividade atividade)
        {
            atividade.Data = DateTime.SpecifyKind(atividade.Data, DateTimeKind.Utc);

            // Previne inserção duplicada de objetos ligados
            if (atividade.EventoAtividades != null)
            {
                foreach (var relacao in atividade.EventoAtividades)
                {
                    relacao.Atividade = null;
                    relacao.Evento = null; // <--- ESTA LINHA É IMPORTANTE
                }
            }

            // Adiciona a atividade
            _context.Atividades.Add(atividade);
            await _context.SaveChangesAsync();

            // Associações com Evento (depois de ter o ID da atividade)
            if (atividade.EventoAtividades != null && atividade.EventoAtividades.Any())
            {
                foreach (var relacao in atividade.EventoAtividades)
                {
                    relacao.IdAtividade = atividade.Id;
                    _context.EventoAtividades.Add(relacao);
                }

                await _context.SaveChangesAsync();
            }

            return CreatedAtAction(nameof(GetAtividade), new { id = atividade.Id }, atividade);
        }



        // DELETE: api/Atividade/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> RemoverAtividade(int id)
        {
            var atividade = await _context.Atividades.FindAsync(id);
            if (atividade == null)
                return NotFound();

            // Remover ligações
            var ligacoes = await _context.EventoAtividades
                .Where(ea => ea.IdAtividade == id)
                .ToListAsync();

            _context.EventoAtividades.RemoveRange(ligacoes);

            _context.Atividades.Remove(atividade);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // POST: api/Atividade/vincular
        [HttpPost("vincular")]
        public async Task<IActionResult> VincularAtividade([FromBody] Evento_Atividade relacao)
        {
            var existe = await _context.EventoAtividades
                .AnyAsync(x => x.IdEvento == relacao.IdEvento && x.IdAtividade == relacao.IdAtividade);

            if (!existe)
            {
                _context.EventoAtividades.Add(relacao);
                await _context.SaveChangesAsync();
            }

            return Ok();
        }
    }
}
