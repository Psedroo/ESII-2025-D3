using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class BilheteController : ControllerBase
{
    private readonly BilheteService _service;

    public BilheteController()
    {
        _service = new BilheteService();
    }

    [HttpPost]
    public ActionResult<Bilhete> CriarBilhete([FromBody] string tipo)
    {
        if (!Enum.TryParse<TipoBilhete>(tipo, out var tipoBilhete))
            return BadRequest("Tipo de bilhete inválido.");

        var bilhete = _service.CriarBilhete(tipoBilhete);
        return Ok(bilhete);
    }
}