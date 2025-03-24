using System.Net.Http.Json;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

public class UsuarioService
{
    private readonly HttpClient _httpClient;

    public UsuarioService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<UsuarioAuth?> RegisterUserAsync(string username, string email, string password)
    {
        // Gerar Salt
        byte[] saltBytes = new byte[16];
        using (var rng = new RNGCryptoServiceProvider())
        {
            rng.GetBytes(saltBytes);
        }
        string salt = Convert.ToBase64String(saltBytes);

        // Gerar Hash da Senha com o Salt
        string senhaHash = HashPassword(password, salt);

        var newUser = new UsuarioAuth
        {
            Username = username,
            Email = email,
            SenhaHash = senhaHash,
            SenhaSalt = salt,
            TipoUsuario = "Participante"
        };

        var response = await _httpClient.PostAsJsonAsync("https://localhost:44343/api/usuario", newUser);

        if (!response.IsSuccessStatusCode)
        {
            string errorMessage = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"API Error: {response.StatusCode} - {errorMessage}");
        }

        return response.IsSuccessStatusCode ? await response.Content.ReadFromJsonAsync<UsuarioAuth>() : null;
    }
    
    public async Task<UsuarioAuth?> AuthenticateUserAsync(string email, string password)
    {
        var response = await _httpClient.GetAsync($"https://localhost:44343/api/usuario");

        if (!response.IsSuccessStatusCode)
        {
            Console.WriteLine($"API Error: {response.StatusCode} - {await response.Content.ReadAsStringAsync()}");
            return null;
        }

        var jsonResponse = await response.Content.ReadAsStringAsync();
        Console.WriteLine($"API Response: {jsonResponse}");

        var users = JsonSerializer.Deserialize<List<UsuarioAuth>>(jsonResponse, 
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

    private string HashPassword(string password, string salt)
    {
        using (var sha256 = SHA256.Create())
        {
            byte[] combinedBytes = Encoding.UTF8.GetBytes(password + salt);
            byte[] hashBytes = sha256.ComputeHash(combinedBytes);
            return Convert.ToBase64String(hashBytes);
        }
    }
}