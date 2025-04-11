using System.Net.Http.Json;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

public class utilizadorService
{
    private readonly HttpClient _httpClient;

    public utilizadorService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<UtilizadorAuth?> RegisterUserAsync(string username, string email, string password, string tipoUsuario){
        
        var usernameCheck = await _httpClient.GetAsync($"https://localhost:44343/api/auth/exists?username={username}");
        var emailCheck = await _httpClient.GetAsync($"https://localhost:44343/api/auth/exists?email={email}");

        if (usernameCheck.IsSuccessStatusCode)
        {
            bool usernameExists = await usernameCheck.Content.ReadFromJsonAsync<bool>();
            if (usernameExists)
            {
                throw new InvalidOperationException("The username is already taken.");
            }
        }

        if (emailCheck.IsSuccessStatusCode)
        {
            bool emailExists = await emailCheck.Content.ReadFromJsonAsync<bool>();
            if (emailExists)
            {
                throw new InvalidOperationException("The email is already registered.");
            }
        }
        
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

        var createdUser = await response.Content.ReadFromJsonAsync<UtilizadorAuth>();
        if (createdUser == null)
            throw new Exception("Failed to create user: No user data returned from API");

        // Se o usuário for um Participante, inserir na tabela Participante
        if (tipoUsuario == "Participante")
        {
            var participante = new Participante
            {
                Nome = "", // Definir nome apropriado depois
                Contacto = "",
                DataNascimento = DateTime.MinValue,
                IdUtilizador = createdUser.Id
            };

            var participanteResponse = await _httpClient.PostAsJsonAsync("https://localhost:44343/api/participante", participante);
            
            if (!participanteResponse.IsSuccessStatusCode)
            {
                string errorMessage = await participanteResponse.Content.ReadAsStringAsync();
                throw new Exception($"API Error (Participante): {participanteResponse.StatusCode} - {errorMessage}");
            }
        }
        
        // Se o usuário for um Organizador, inserir na tabela Organizador
        if (tipoUsuario == "Organizador")
        {
            var organizador = new Organizador
            {
                Nome = "", // Definir nome apropriado depois
                Contacto = "",
                DataNascimento = DateTime.MinValue,
                IdUsuario = createdUser.Id
            };

            var organizadorResponse = await _httpClient.PostAsJsonAsync("https://localhost:44343/api/organizador", organizador);

            if (!organizadorResponse.IsSuccessStatusCode)
            {
                string errorMessage = await organizadorResponse.Content.ReadAsStringAsync();
                throw new Exception($"API Error (Organizador): {organizadorResponse.StatusCode} - {errorMessage}");
            }
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
    
    public async Task<bool> UpdateUserAsync(string currentEmail, string newEmail)
    {
        var response = await _httpClient.PutAsJsonAsync("/api/users/update", new { CurrentEmail = currentEmail, NewEmail = newEmail });
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
}
