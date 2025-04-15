    namespace ES2Real.Data;
    using ES2Real.Interfaces;

    public class ParticipanteService : ITipoUtilizadorService
    {
        private readonly HttpClient _httpClient;

        public ParticipanteService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task CriarRegistoEspecificoAsync(UtilizadorAuth utilizador)
        {
            var participante = new Participante
            {
                Nome = "", // definir depois
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
