﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ES2Real.Data;
using ES2Real.Models;
using ES2Real.Components.Services;

namespace ES2Real.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EventoController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly BilheteService _bilheteService;

        public EventoController(ApplicationDbContext context, BilheteService bilheteService)
        {
            _context = context;
            _bilheteService = bilheteService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Evento>>> GetEventos()
        {
            return await _context.Eventos.ToListAsync();
        }
        

        
        [HttpPost]
        public async Task<ActionResult<Evento>> CriarEvento(EventoCriarDTO eventoDto)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var evento = new Evento
                {
                    Nome = eventoDto.Nome,
                    Categoria = eventoDto.Categoria,
                    Data = DateTime.SpecifyKind(eventoDto.Data, DateTimeKind.Utc),
                    Hora = eventoDto.Hora,
                    Local = eventoDto.Local,
                    Descricao = eventoDto.Descricao,
                    CapacidadeMax = eventoDto.CapacidadeMax,
                    IdOrganizador = eventoDto.IdOrganizador
                };
                
                Console.WriteLine($" Id: {eventoDto.IdOrganizador}");


                _context.Eventos.Add(evento);
                await _context.SaveChangesAsync();
                

                var bilheteNormal = _bilheteService.CriarBilhete(TipoBilhete.Normal, eventoDto.PrecoBilheteNormal, eventoDto.QuantidadeBilheteNormal);
                bilheteNormal.Evento = evento;
                _context.Bilhetes.Add(bilheteNormal);
                

                await _context.SaveChangesAsync();

                await transaction.CommitAsync();
                return CreatedAtAction(nameof(GetEventos), new { id = evento.Id }, evento);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                Console.WriteLine($"Erro ao criar evento: {ex.Message}");
                return StatusCode(500, $"Erro interno ao criar evento: {ex.Message}");
            }
        }




        [HttpDelete("{id}")]
        public async Task<IActionResult> RemoverEvento(int id)
        {
            var evento = await _context.Eventos.FindAsync(id);
            if (evento == null) return NotFound();

            _context.Eventos.Remove(evento);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> AtualizarEvento(int id, [FromBody] Evento eventoAtualizado)
        {
            if (id != eventoAtualizado.Id)
                return BadRequest("ID do evento não corresponde.");

            var eventoExistente = await _context.Eventos.FindAsync(id);
            if (eventoExistente == null)
                return NotFound("Evento não encontrado.");

            eventoExistente.Nome = eventoAtualizado.Nome;
            eventoExistente.Data = DateTime.SpecifyKind(eventoAtualizado.Data, DateTimeKind.Utc);
            eventoExistente.Hora = eventoAtualizado.Hora;
            eventoExistente.Local = eventoAtualizado.Local;
            eventoExistente.Categoria = eventoAtualizado.Categoria;
            eventoExistente.Descricao = eventoAtualizado.Descricao;
            eventoExistente.CapacidadeMax = eventoAtualizado.CapacidadeMax;

            await _context.SaveChangesAsync();

            return NoContent();
        }
        
        [HttpGet("participante/{idUtilizador}")]
        public async Task<ActionResult<List<Evento>>> GetEventosPorParticipante(int idUtilizador)
        {
            var eventos = await _context.BilheteParticipante
                .Include(bp => bp.Bilhete)
                .ThenInclude(b => b.Evento)
                .Where(bp => bp.IdParticipante == idUtilizador)
                .Select(bp => bp.Bilhete.Evento)
                .Distinct()
                .ToListAsync();

            return eventos;
        }
        
        [HttpGet("organizador/{idOrganizador}")]
        public async Task<ActionResult<List<Evento>>> GetEventosPorOrganizador(int idOrganizador)
        {
            var eventos = await _context.Eventos
                .Where(e => e.IdOrganizador == idOrganizador)
                .ToListAsync();

            if (eventos == null || eventos.Count == 0)
                return NotFound("Nenhum evento encontrado para este organizador.");

            return eventos;
        }

    }
    
    public class EventoCriarDTO
    {
        public string Nome { get; set; } = string.Empty;
        public string Categoria { get; set; } = string.Empty;
        public DateTime Data { get; set; }
        public TimeSpan Hora { get; set; }
        public string Local { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty;
        public int CapacidadeMax { get; set; }
        public int IdOrganizador { get; set; }
        public decimal PrecoBilheteNormal { get; set; } 
        public int QuantidadeBilheteNormal { get; set; }
    }

    
}
