using ES2Real.Models;

namespace ES2Real.TipoUsuarioHandlers;
using ES2Real.Interfaces;

public class ParticipanteHandler : ITipoUsuarioHandler
{
    private readonly HttpClient _httpClient;

    public ParticipanteHandler(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public string TipoUsuarioSuportado => "Participante";

    public async Task CriarDadosEspecificosAsync(UtilizadorAuth utilizador)
    {
        var participante = new Participante
        {
            Nome = "",
            Contacto = "",
            DataNascimento = DateTime.MinValue,
            IdUtilizador = utilizador.Id
        };

        var response = await _httpClient.PostAsJsonAsync("https://localhost:44343/api/participante", participante);

        if (!response.IsSuccessStatusCode)
        {
            string errorMessage = await response.Content.ReadAsStringAsync();
            throw new Exception($"API Error (Participante): {response.StatusCode} - {errorMessage}");
        }
    }
}
