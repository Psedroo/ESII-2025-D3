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

        // POST: api/Atividade
        [HttpPost]
        public async Task<ActionResult<Atividade>> CriarAtividade([FromBody] Atividade atividade)
        {
            atividade.Data = DateTime.SpecifyKind(atividade.Data, DateTimeKind.Utc);

            if (atividade.EventoAtividades != null)
            {
                foreach (var relacao in atividade.EventoAtividades)
                {
                    relacao.Atividade = null;
                    relacao.Evento = null;
                }
            }

            _context.Atividades.Add(atividade);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetAtividade), new { id = atividade.Id }, atividade);
        }


        // PUT: api/Atividade/5
        [HttpPut("{id}")]
        public async Task<IActionResult> AtualizarAtividade(int id, [FromBody] Atividade atividadeAtualizada)
        {
            if (id != atividadeAtualizada.Id)
                return BadRequest("ID da atividade não corresponde.");

            var atividadeExistente = await _context.Atividades
                .Include(a => a.EventoAtividades)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (atividadeExistente == null)
                return NotFound("Atividade não encontrada.");

            atividadeExistente.Nome = atividadeAtualizada.Nome;
            atividadeExistente.Descricao = atividadeAtualizada.Descricao;
            atividadeExistente.Data = DateTime.SpecifyKind(atividadeAtualizada.Data, DateTimeKind.Utc);
            atividadeExistente.Hora = atividadeAtualizada.Hora;

            if (atividadeAtualizada.EventoAtividades != null && atividadeAtualizada.EventoAtividades.Any())
            {
                var novaRelacao = atividadeAtualizada.EventoAtividades.First();
                var relacaoExistente = atividadeExistente.EventoAtividades
                    .FirstOrDefault(ea => ea.IdEvento == novaRelacao.IdEvento);

                if (relacaoExistente == null)
                {
                    novaRelacao.IdAtividade = id;
                    novaRelacao.Atividade = null;
                    novaRelacao.Evento = null;
                    _context.EventoAtividades.Add(novaRelacao);
                }
            }

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // DELETE: api/Atividade/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> RemoverAtividade(int id)
        {
            var atividade = await _context.Atividades.FindAsync(id);
            if (atividade == null)
                return NotFound();

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