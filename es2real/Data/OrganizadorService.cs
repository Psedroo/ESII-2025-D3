using ES2Real.Interfaces;

namespace ES2Real.Data;

public class OrganizadorService : ITipoUtilizadorService
{
    private readonly HttpClient _httpClient;

    public OrganizadorService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task CriarRegistoEspecificoAsync(UtilizadorAuth utilizador)
    {
        var organizador = new Organizador
        {
            Nome  = "",
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
