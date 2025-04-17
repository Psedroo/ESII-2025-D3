using ES2Real.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ES2Real.Controllers
{
    [Route("api/usuario")]
    [ApiController]
    public class UtilizadorController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public UtilizadorController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Create User
        [HttpPost]
        public async Task<ActionResult<UtilizadorAuth>> CreateUser([FromBody] UtilizadorAuth newUser)
        {
            _context.UtilizadorAuth.Add(newUser);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetUsers), new { id = newUser.Id }, newUser);
        }

        // Get All Users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UtilizadorAuth>>> GetUsers()
        {
            var users = await _context.UtilizadorAuth.ToListAsync();
            if (!users.Any())
            {
                return NotFound("No users found.");
            }

            return Ok(users);
        }

        // Get Organizador by UserId
        [HttpGet("byUserId/{utilizadorId}")]
        public async Task<ActionResult<Organizador>> GetOrganizadorByUserId(int utilizadorId)
        {
            var organizador = await _context.Organizadores
                .FirstOrDefaultAsync(o => o.IdUsuario == utilizadorId);

            if (organizador == null)
            {
                return NotFound();
            }

            return organizador;
        }

    }
}