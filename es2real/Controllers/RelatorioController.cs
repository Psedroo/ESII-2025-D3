using System.Text.Json;
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

        // Criar o Relatorio base
        var relatorio = new Relatorio
        {
            Data = DateTime.UtcNow,
            Informacoes = "Relatório geral criado automaticamente."
        };

        _context.Relatorio.Add(relatorio);
        await _context.SaveChangesAsync();

        // Criar o RelatorioGeral e associar
        var relatorioGeral = new RelatorioGeral
        {
            NumPorCat = eventosPorCategoria.Sum(c => c.Value),
            MaisPopul = maisPopul,
            TotalPart = totalParticipantes,
            IdRelatorio = relatorio.Id,
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

        return Ok("Relatório geral criado com sucesso.");
    }
    catch (Exception ex)
    {
        return StatusCode(500, $"Erro ao salvar relatório geral: {ex.Message}");
    }
}


        [HttpPost("gerar-especifico/{eventoId}")]
        public async Task<IActionResult> GerarRelatorioEspecifico(int eventoId, [FromQuery] string? texto)
        {
            try
            {
                var evento = await _context.Eventos.FindAsync(eventoId);
                if (evento == null)
                    return NotFound("Evento não encontrado.");

                var bilhetes = await _context.Bilhetes
                    .Include(b => b.BilheteParticipante)
                    .Where(b => b.idEvento == eventoId)
                    .ToListAsync();

                int numParticipantes = bilhetes
                    .SelectMany(b => b.BilheteParticipante)
                    .Select(bp => bp.IdParticipante)
                    .Distinct()
                    .Count();

                decimal receitaTotal = bilhetes.Sum(b => b.Preco * b.BilheteParticipante.Count);


                var relatorioEspecificoExistente = await _context.EventoRelatoriosEspecificos
                    .Include(er => er.RelatorioEspecifico)
                    .ThenInclude(re => re.Relatorio)
                    .FirstOrDefaultAsync(er => er.IdEvento == eventoId);

                if (relatorioEspecificoExistente != null)
                {
                    relatorioEspecificoExistente.RelatorioEspecifico.NumParticipantesAtiv = numParticipantes;
                    relatorioEspecificoExistente.RelatorioEspecifico.Receita = receitaTotal;

                    var relatorio = await _context.Relatorio.FindAsync(relatorioEspecificoExistente.RelatorioEspecifico.IdRelatorio);
                    if (relatorio != null)
                    {
                        relatorio.Informacoes = string.IsNullOrWhiteSpace(texto) ? " " : texto;
                        relatorio.Data = DateTime.UtcNow;
                    }

                    await _context.SaveChangesAsync();
                    return Ok("Relatório atualizado com sucesso.");
                }

                var info = string.IsNullOrWhiteSpace(texto) ? " " : texto;

                var novoRelatorio = new Relatorio
                {
                    Data = DateTime.UtcNow,
                    Informacoes = info
                };

                _context.Relatorio.Add(novoRelatorio);
                await _context.SaveChangesAsync();

                var novoRelatorioEspecifico = new RelatorioEspecifico
                {
                    NumParticipantesAtiv = numParticipantes,
                    Receita = receitaTotal,
                    Feedback = "",
                    IdRelatorio = novoRelatorio.Id
                };

                novoRelatorioEspecifico.EventoRelatoriosEspecificos.Add(new Evento_RelatorioEspecifico
                {
                    IdEvento = eventoId,
                    RelatorioEspecifico = novoRelatorioEspecifico
                });

                _context.RelatoriosEspecificos.Add(novoRelatorioEspecifico);
                await _context.SaveChangesAsync();

                return Ok("Relatório específico criado com sucesso.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao criar/atualizar relatório: {ex.Message}");
            }
        }

        [HttpGet("texto-relatorio/{eventoId}")]
        public async Task<IActionResult> ObterTextoRelatorio(int eventoId)
        {
            try
            {
                var relatorioEspecifico = await _context.EventoRelatoriosEspecificos
                    .Include(er => er.RelatorioEspecifico)
                    .ThenInclude(re => re.Relatorio)
                    .FirstOrDefaultAsync(er => er.IdEvento == eventoId);

                if (relatorioEspecifico == null || relatorioEspecifico.RelatorioEspecifico?.Relatorio == null)
                    return NotFound("Relatório não encontrado para este evento.");

                // Ir buscar atividades do evento
                var atividades = await _context.EventoAtividades
                    .Where(ea => ea.IdEvento == eventoId)
                    .Include(ea => ea.Atividade)
                    .ThenInclude(a => a.AtividadeParticipantes)
                    .ToListAsync();

                var participantesPorAtividade = atividades
                    .Select(a => new
                    {
                        Atividade = a.Atividade.Nome,
                        NumParticipantes = a.Atividade.AtividadeParticipantes
                            .Select(ap => ap.IdParticipante)
                            .Distinct()
                            .Count()
                    }).ToList();

                return Ok(new
                {
                    Texto = relatorioEspecifico.RelatorioEspecifico.Relatorio.Informacoes,
                    ParticipantesPorAtividade = participantesPorAtividade
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao obter texto do relatório: {ex.Message}");
            }
        }
        
        [HttpGet("numero-participantes/{eventoId}")]
        public async Task<IActionResult> GetNumeroParticipantes(int eventoId)
        {
            try
            {
                var bilhetesDoEvento = await _context.Bilhetes
                    .Where(b => b.idEvento == eventoId)
                    .Select(b => b.Id)
                    .ToListAsync();

                var numParticipantes = await _context.BilheteParticipante
                    .Where(bp => bilhetesDoEvento.Contains(bp.IdBilhete))
                    .Select(bp => bp.IdParticipante)
                    .Distinct()
                    .CountAsync();

                return Ok(numParticipantes);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao obter número de participantes: {ex.Message}");
            }
        }
        [HttpGet("participantes-atividade/{eventoId}")]
        public async Task<ActionResult<List<object>>> GetParticipantesPorAtividade(int eventoId)
        {
            var atividadesDoEvento = await _context.EventoAtividades
                .Where(ea => ea.IdEvento == eventoId)
                .Include(ea => ea.Atividade)
                .Select(ea => ea.Atividade)
                .ToListAsync();

            var resultado = new List<object>();

            foreach (var atividade in atividadesDoEvento)
            {
                var count = await _context.AtividadeParticipantes
                    .CountAsync(ap => ap.IdAtividade == atividade.Id);

                resultado.Add(new
                {
                    atividade = atividade.Nome,
                    totalParticipantes = count
                });
            }

            return Ok(resultado);
        }

        [HttpGet("atividades-com-participantes/{eventoId}")]
        public async Task<IActionResult> GetAtividadesComParticipantes(int eventoId)
        {
            try
            {
                var resultado = await _context.EventoAtividades
                    .Where(ea => ea.IdEvento == eventoId)
                    .Include(ea => ea.Atividade)
                    .ThenInclude(a => a.AtividadeParticipantes)
                    .Select(ea => new
                    {
                        NomeAtividade   = ea.Atividade.Nome,
                        NumParticipantes = ea.Atividade.AtividadeParticipantes
                            .Select(ap => ap.IdParticipante)
                            .Distinct()
                            .Count()
                    })
                    .ToListAsync();

                // Serializa para JSON e escreve na consola
                var json = JsonSerializer.Serialize(resultado, new JsonSerializerOptions
                {
                    WriteIndented = true   // deixa mais legível
                });
                Console.WriteLine($"=== JSON actividades evento {eventoId} ===\n{json}\n=========================");

                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao obter actividades: {ex.Message}");
            }
        }

        [HttpGet("receita-evento/{eventoId}")]
        public async Task<IActionResult> GetReceitaEvento(int eventoId)
        {
            try
            {
                decimal receitaTotal = await _context.Bilhetes
                    .Where(b => b.idEvento == eventoId)
                    .Select(b => b.Preco * b.BilheteParticipante.Count)
                    .SumAsync();

                return Ok(receitaTotal);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao calcular receita: {ex.Message}");
            }
        }




    }
}
