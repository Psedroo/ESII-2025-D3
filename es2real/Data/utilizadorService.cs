using System.Net.Http.Json;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using ES2Real.Interfaces;

public class utilizadorService
{
    private readonly HttpClient _httpClient;
    private readonly IEnumerable<ITipoUsuarioHandler> _userHandlers;

    public utilizadorService(HttpClient httpClient, IEnumerable<ITipoUsuarioHandler> userHandlers)
    {
        _httpClient = httpClient;
        _userHandlers = userHandlers;
    }

    public async Task<UtilizadorAuth?> RegisterUserAsync(string username, string email, string password, string tipoUsuario)
    {
        var usernameCheck = await _httpClient.GetAsync($"https://localhost:44343/api/auth/exists?username={username}");
        var emailCheck = await _httpClient.GetAsync($"https://localhost:44343/api/auth/exists?email={email}");

        if (usernameCheck.IsSuccessStatusCode && await usernameCheck.Content.ReadFromJsonAsync<bool>())
            throw new InvalidOperationException("The username is already taken.");

        if (emailCheck.IsSuccessStatusCode && await emailCheck.Content.ReadFromJsonAsync<bool>())
            throw new InvalidOperationException("The email is already registered.");

        byte[] saltBytes = new byte[16];
        using (var rng = new RNGCryptoServiceProvider())
        {
            rng.GetBytes(saltBytes);
        }
        string salt = Convert.ToBase64String(saltBytes);
        string senhaHash = HashPassword(password, salt);

        var newUser = new UtilizadorAuth
        {
            Username = username,
            Email = email,
            SenhaHash = senhaHash,
            SenhaSalt = salt,
            TipoUsuario = tipoUsuario
        };

        var response = await _httpClient.PostAsJsonAsync("https://localhost:44343/api/usuario", newUser);

        if (!response.IsSuccessStatusCode)
        {
            string errorMessage = await response.Content.ReadAsStringAsync();
            throw new Exception($"API Error: {response.StatusCode} - {errorMessage}");
        }

        var createdUser = await response.Content.ReadFromJsonAsync<UtilizadorAuth>()
                          ?? throw new Exception("Failed to create user: No user data returned from API");

        var handler = _userHandlers.FirstOrDefault(h => h.TipoUsuarioSuportado == tipoUsuario);
        if (handler != null)
        {
            await handler.CriarDadosEspecificosAsync(createdUser);
        }

        return createdUser;
    }
    
    public async Task<UtilizadorAuth?> AuthenticateUserAsync(string email, string password)
    {
        var response = await _httpClient.GetAsync("https://localhost:44343/api/usuario");

        if (!response.IsSuccessStatusCode)
        {
            Console.WriteLine($"API Error: {response.StatusCode} - {await response.Content.ReadAsStringAsync()}");
            return null;
        }

        var jsonResponse = await response.Content.ReadAsStringAsync();
        var users = JsonSerializer.Deserialize<List<UtilizadorAuth>>(jsonResponse, 
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        if (users == null || users.Count == 0)
        {
            return null;
        }

        var user = users.FirstOrDefault(u => u.Email == email);
        if (user == null || string.IsNullOrEmpty(user.SenhaSalt) || string.IsNullOrEmpty(user.SenhaHash))
        {
            return null;
        }

        string computedHash = HashPassword(password, user.SenhaSalt);
        return computedHash == user.SenhaHash ? user : null;
    }

    public async Task<Participante?> ObterParticipante(int idUtilizador)
    {
        var response = await _httpClient.GetAsync($"https://localhost:44343/api/participante/{idUtilizador}");

        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        return await response.Content.ReadFromJsonAsync<Participante>();
    }

    public async Task<Organizador?> ObterOrganizador(int idUsuario)
    {
        var response = await _httpClient.GetAsync($"https://localhost:44343/api/organizador/{idUsuario}");

        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        return await response.Content.ReadFromJsonAsync<Organizador>();
    }

    public async Task<bool> AtualizarParticipante(int id, string nome, string contacto, DateTime dataNascimento)
    {
        var response = await _httpClient.PutAsJsonAsync($"https://localhost:44343/api/participante/{id}", new
        {
            Nome = nome,
            Contacto = contacto,
            DataNascimento = dataNascimento
        });

        return response.IsSuccessStatusCode;
    }

    public async Task<bool> AtualizarOrganizador(int id, string nome, string contacto, DateTime dataNascimento)
    {
        var response = await _httpClient.PutAsJsonAsync($"https://localhost:44343/api/organizador/{id}", new
        {
            Nome = nome,
            Contacto = contacto,
            DataNascimento = dataNascimento
        });

        return response.IsSuccessStatusCode;
    }

    private string HashPassword(string password, string salt)
    {
        using (var sha256 = SHA256.Create())
        {
            byte[] combinedBytes = Encoding.UTF8.GetBytes(password + salt);
            byte[] hashBytes = sha256.ComputeHash(combinedBytes);
            return Convert.ToBase64String(hashBytes);
        }
    }
    
    public async Task<UtilizadorAuth?> ObterPorEmail(string email)
    {
        var response = await _httpClient.GetAsync($"https://localhost:44343/api/usuario?email={email}");

        if (!response.IsSuccessStatusCode)
        {
            // Retorna null caso não encontre o utilizador ou ocorra algum erro na requisição
            return null;
        }

        return await response.Content.ReadFromJsonAsync<UtilizadorAuth>();
    }
    
}

