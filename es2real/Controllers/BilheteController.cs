using Microsoft.AspNetCore.Mvc;
using ES2Real.Models;
using ES2Real.Components.Services;
using System.Linq;

namespace ES2Real.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BilheteController : ControllerBase
    {
        private readonly BilheteService _service;
        private readonly ApplicationDbContext _context;

        public BilheteController(BilheteService service, ApplicationDbContext context)
        {
            _service = service;
            _context = context;
        }

        // POST: api/Bilhete
        [HttpPost]
        public ActionResult<Bilhete> CriarBilhete([FromBody] string tipo, decimal preco, int quantidadeBilheteNormal)
        {
            if (!Enum.TryParse<TipoBilhete>(tipo, out var tipoBilhete))
                return BadRequest("Tipo de bilhete inválido.");

            var bilhete = _service.CriarBilhete(tipoBilhete, preco, quantidadeBilheteNormal);
            return Ok(bilhete);
        }

        // DELETE: api/Bilhete/cancelar/5
        [HttpDelete("cancelar/{id}")]
        public IActionResult CancelarInscricao(int id)
        {
            var resultado = _service.CancelarBilhete(id);
            if (!resultado)
                return NotFound("Bilhete não encontrado.");

            return Ok(new { message = "Inscrição cancelada com sucesso." });
        }

        // GET: api/Bilhete/evento/5
        [HttpGet("evento/{idEvento}")]
        public IActionResult GetBilhetesPorEvento(int idEvento)
        {
            var bilhetes = _context.Bilhetes
                .Where(b => b.idEvento == idEvento)
                .ToList();

            return Ok(bilhetes);
        }
    }
}