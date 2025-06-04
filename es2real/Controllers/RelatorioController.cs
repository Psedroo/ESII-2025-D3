using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ES2Real.Data;
using ES2Real.Models;

namespace ES2Real.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RelatorioController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public RelatorioController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("geral")]
        public async Task<IActionResult> GetRelatorioGeral()
        {
            try
            {
                var eventosPorCategoria = await _context.Eventos
                    .GroupBy(e => e.Categoria)
                    .Select(g => new { Categoria = g.Key, Count = g.Count() })
                    .ToDictionaryAsync(g => g.Categoria, g => g.Count);

                var eventos = await _context.Eventos.ToListAsync();
                var topEventos = new List<(string Nome, int NumParticipantes)>();

                foreach (var evento in eventos)
                {
                    var bilhetes = await _context.Bilhetes
                        .Include(b => b.BilheteParticipante)
                        .Where(b => b.idEvento == evento.Id)
                        .ToListAsync();

                    var numParticipantes = bilhetes
                        .SelectMany(b => b.BilheteParticipante)
                        .Select(bp => bp.IdParticipante)
                        .Distinct()
                        .Count();

                    topEventos.Add((evento.Nome, numParticipantes));
                }

                var maisPopul = string.Join(", ", topEventos
                    .OrderByDescending(e => e.NumParticipantes)
                    .Take(5)
                    .Select(e => $"{e.Nome} ({e.NumParticipantes})"));

                var totalParticipantes = await _context.BilheteParticipante
                    .Select(bp => bp.IdParticipante)
                    .Distinct()
                    .CountAsync();

                var response = new
                {
                    EventosPorCategoria = eventosPorCategoria,
                    MaisPopul = maisPopul,
                    TotalPart = totalParticipantes
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao gerar relatório geral: {ex.Message}");
            }
        }

        [HttpPost("geral")]
        public async Task<IActionResult> CriarRelatorioGeral()
        {
            try
            {
                var eventosPorCategoria = await _context.Eventos
                    .GroupBy(e => e.Categoria)
                    .Select(g => new { Categoria = g.Key, Count = g.Count() })
                    .ToDictionaryAsync(g => g.Categoria, g => g.Count);

                var eventos = await _context.Eventos.ToListAsync();
                var topEventos = new List<(string Nome, int NumParticipantes)>();

                foreach (var evento in eventos)
                {
                    var bilhetes = await _context.Bilhetes
                        .Include(b => b.BilheteParticipante)
                        .Where(b => b.idEvento == evento.Id)
                        .ToListAsync();

                    var numParticipantes = bilhetes
                        .SelectMany(b => b.BilheteParticipante)
                        .Select(bp => bp.IdParticipante)
                        .Distinct()
                        .Count();

                    topEventos.Add((evento.Nome, numParticipantes));
                }

                var maisPopul = string.Join(", ", topEventos
                    .OrderByDescending(e => e.NumParticipantes)
                    .Take(5)
                    .Select(e => $"{e.Nome} ({e.NumParticipantes})"));

                var totalParticipantes = await _context.BilheteParticipante
                    .Select(bp => bp.IdParticipante)
                    .Distinct()
                    .CountAsync();

                var relatorioGeral = new RelatorioGeral
                {
                    NumPorCat = eventosPorCategoria.Sum(c => c.Value),
                    MaisPopul = maisPopul,
                    TotalPart = totalParticipantes,
                    IdRelatorio = 0,
                    EventoRelatoriosGerais = new List<Evento_RelatorioGeral>()
                };

                foreach (var evento in eventos)
                {
                    relatorioGeral.EventoRelatoriosGerais.Add(new Evento_RelatorioGeral
                    {
                        IdEvento = evento.Id,
                        RelatorioGeral = relatorioGeral
                    });
                }

                _context.RelatoriosGerais.Add(relatorioGeral);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao salvar relatório geral: {ex.Message}");
            }
        }
    }
}