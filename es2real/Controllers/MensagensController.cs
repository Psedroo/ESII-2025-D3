using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
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

                return Ok(mensagens);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao buscar mensagens para o evento {idEvento}: {ex.Message}");
                return StatusCode(500, "Erro interno ao processar a requisição.");
            }
        }

        // POST: api/Mensagem
        [HttpPost]
        public async Task<ActionResult<MensagemDto>> CriarMensagem(MensagemDto mensagemDto)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(mensagemDto.Texto) || mensagemDto.IdEvento <= 0)
                {
                    return BadRequest("Texto da mensagem e ID do evento são obrigatórios.");
                }

                // Verify if the event exists and belongs to the logged-in organizer
                var evento = await _context.Eventos.FindAsync(mensagemDto.IdEvento);
                if (evento == null)
                {
                    return BadRequest("Evento não encontrado.");
                }

                var userId = mensagemDto.IdOrganizador;
                if (evento.IdOrganizador != userId)
                {
                    return Forbid("Você não tem permissão para adicionar mensagens a este evento.");
                }

                var mensagem = new Mensagem
                {
                    Texto = mensagemDto.Texto,
                    IdEvento = mensagemDto.IdEvento
                };

                _context.Mensagens.Add(mensagem);
                await _context.SaveChangesAsync();

                mensagemDto.Id = mensagem.Id;
                return CreatedAtAction(nameof(GetMensagensPorEvento), new { idEvento = mensagem.IdEvento }, mensagemDto);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao criar mensagem: {ex.Message}");
                return StatusCode(500, "Erro interno ao processar a requisição.");
            }
        }

        // PUT: api/Mensagem/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> EditarMensagem(int id, MensagemDto mensagemDto)
        {
            try
            {
                var mensagem = await _context.Mensagens.FindAsync(id);
                if (mensagem == null)
                {
                    return NotFound("Mensagem não encontrada.");
                }

                // Verify if the event belongs to the logged-in organizer
                var evento = await _context.Eventos.FindAsync(mensagemDto.IdEvento);
                if (evento == null)
                {
                    return BadRequest("Evento não encontrado.");
                }
                
                if (string.IsNullOrWhiteSpace(mensagemDto.Texto))
                {
                    return BadRequest("Texto da mensagem é obrigatório.");
                }

                mensagem.Texto = mensagemDto.Texto;
                mensagem.IdEvento = mensagemDto.IdEvento;

                _context.Mensagens.Update(mensagem);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao editar mensagem {id}: {ex.Message}");
                return StatusCode(500, "Erro interno ao processar a requisição.");
            }
        }
    }

    public class MensagemDto
    {
        public int Id { get; set; }
        public string Texto { get; set; } = string.Empty;
        public int IdEvento { get; set; }
        
        public int IdOrganizador { get; set; }

    }
}