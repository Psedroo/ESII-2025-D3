using ES2Real.Interfaces;

namespace ES2Real.TipoUsuarioHandlers;

public class OrganizadorHandler : ITipoUsuarioHandler
{
    private readonly HttpClient _httpClient;

    public OrganizadorHandler(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public string TipoUsuarioSuportado => "Organizador";

    public async Task CriarDadosEspecificosAsync(UtilizadorAuth utilizador)
    {
        var organizador = new Organizador
        {
            Nome = "",
            Contacto = "",
            DataNascimento = DateTime.MinValue,
            IdUsuario = utilizador.Id
        };

        var response = await _httpClient.PostAsJsonAsync("https://localhost:44343/api/organizador", organizador);

        if (!response.IsSuccessStatusCode)
        {
            string errorMessage = await response.Content.ReadAsStringAsync();
            throw new Exception($"API Error (Organizador): {response.StatusCode} - {errorMessage}");
        }
    }
}
