/*
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ES2Real.Models;

[ApiController]
[Route("api/[controller]")]
public class RelatorioController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public RelatorioController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: api/Relatorio/EventosSemRelatorio
    [HttpGet("EventosSemRelatorio")]
    public async Task<ActionResult<IEnumerable<Evento>>> GetEventosSemRelatorio()
    {
        var eventosComRelatorio = await _context.EventoRelatoriosEspecificos
            .Select(er => er.IdEvento)
            .ToListAsync();

        var eventosSemRelatorio = await _context.Eventos
            .Where(e => !eventosComRelatorio.Contains(e.Id))
            .Include(e => e.Organizador)
            .ToListAsync();

        return Ok(eventosSemRelatorio);
    }

    // POST: api/Relatorio/GerarRelatorio/{eventoId}
    [HttpPost("GerarRelatorio/{eventoId}")]
    public async Task<ActionResult> GerarRelatorioEspecifico(int eventoId)
    {
        var evento = await _context.Eventos
            .Include(e => e.EventoAtividades)
            .Include(e => e.Bilhetes)
                .ThenInclude(b => b.BilheteParticipante)
            .FirstOrDefaultAsync(e => e.Id == eventoId);

        if (evento == null)
        {
            return NotFound("Evento não encontrado.");
        }

        var relatorioExistente = await _context.EventoRelatoriosEspecificos
            .AnyAsync(r => r.IdEvento == eventoId);
        if (relatorioExistente)
        {
            return BadRequest("Relatório já existe para este evento.");
        }

        int totalParticipantes = evento.Bilhetes.Sum(b => b.BilheteParticipante.Count);
        decimal receitaTotal = evento.Bilhetes.Sum(b => b.Preco * b.BilheteParticipante.Count);

        var novoRelatorio = new RelatorioEspecifico
        {
            TotalParticipantes = totalParticipantes,
            ReceitaTotal = receitaTotal,
            DataCriacao = DateTime.UtcNow
        };

        _context.RelatoriosEspecificos.Add(novoRelatorio);
        await _context.SaveChangesAsync();

        var associacao = new Evento_RelatorioEspecifico
        {
            IdEvento = eventoId,
            IdRelatorioEspecifico = novoRelatorio.Id
        };
        _context.EventoRelatoriosEspecificos.Add(associacao);
        await _context.SaveChangesAsync();

        return Ok(new
        {
            Mensagem = "Relatório específico gerado com sucesso.",
            Relatorio = novoRelatorio
        });
    }

    // GET: api/Relatorio/DetalhesRelatorio/{eventoId}
    [HttpGet("DetalhesRelatorio/{eventoId}")]
    public async Task<ActionResult> GetDetalhesRelatorio(int eventoId)
    {
        var evento = await _context.Eventos
            .Include(e => e.EventoAtividades)
                .ThenInclude(ea => ea.Atividade)
            .Include(e => e.Bilhetes)
                .ThenInclude(b => b.BilheteParticipante)
            .FirstOrDefaultAsync(e => e.Id == eventoId);

        if (evento == null)
            return NotFound("Evento não encontrado.");

        var participantesPorAtividade = evento.EventoAtividades.Select(ea => new
        {
            Atividade = ea.Atividade.Nome,
            Participantes = ea.Atividade.Bilhetes?
                .SelectMany(b => b.BilheteParticipante)
                .Select(bp => bp.IdParticipante)
                .Distinct()
                .Count() ?? 0
        });

        decimal receitaTotal = evento.Bilhetes.Sum(b => b.Preco * b.BilheteParticipante.Count);
        int totalParticipantes = evento.Bilhetes
            .SelectMany(b => b.BilheteParticipante)
            .Select(bp => bp.IdParticipante)
            .Distinct()
            .Count();

        return Ok(new
        {
            Evento = new
            {
                evento.Id,
                evento.Nome,
                evento.Local,
                evento.Data,
                evento.Hora,
                evento.Categoria
            },
            ReceitaTotal = receitaTotal,
            TotalParticipantes = totalParticipantes,
            ParticipantesPorAtividade = participantesPorAtividade
        });
    }
}
*/
