using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ES2Real.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MensagensController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public MensagensController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Mensagem/evento/{idEvento}
        [HttpGet("evento/{idEvento}")]
        public async Task<ActionResult<IEnumerable<MensagemDto>>> GetMensagensPorEvento(int idEvento)
        {
            try
            {
                var mensagens = await _context.Mensagens
                    .Where(m => m.IdEvento == idEvento)
                    .Select(m => new MensagemDto
                    {
                        Id = m.Id,
                        Texto = m.Texto,
                        IdEvento = m.IdEvento
                    })
                    .ToListAsync();

                if (!mensagens.Any())
                {
                    return Ok(new List<MensagemDto>()); // Retorna lista vazia se não houver mensagens
                }

                return Ok(mensagens);
            }
            catch (Exception ex)
            {
                // Log do erro (pode usar ILogger para logging real)
                Console.WriteLine($"Erro ao buscar mensagens para o evento {idEvento}: {ex.Message}");
                return StatusCode(500, "Erro interno ao processar a requisição.");
            }
        }
    }

    public class MensagemDto
    {
        public int Id { get; set; }
        public string Texto { get; set; } = string.Empty;
        public int IdEvento { get; set; }
    }
}