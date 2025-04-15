using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/[controller]")]
public class BilheteParticipanteController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public BilheteParticipanteController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Evento>>> GetMyEventos()
    {
        List<Bilhete_Participante> bilhetesParticipantes = await _context.BilheteParticipante.ToListAsync();
        
        List<Bilhete> bilhetes = await _context.Bilhetes.ToListAsync();

        HashSet<int> MyEventos = new HashSet<int>();

        foreach (var bp in bilhetesParticipantes)
        {
            Bilhete? bilheteEncontrado = null;

            foreach (var b in bilhetes)
            {
                if (b.Id == bp.IdBilhete)
                {
                    bilheteEncontrado = b;
                }
            }

            if (bilheteEncontrado != null)
            {
                MyEventos.Add(bilheteEncontrado.idEvento);
            }
        }

        List<Evento> eventosDoParticipante = new List<Evento>();

        List<Evento> todosEventos = await _context.Eventos.ToListAsync();

        foreach (var evento in todosEventos)
        {
            if (MyEventos.Contains(evento.Id))
            {
                eventosDoParticipante.Add(evento);
            }
        }

        
        return Ok(eventosDoParticipante);
    }
}