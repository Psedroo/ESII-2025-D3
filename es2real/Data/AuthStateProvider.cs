namespace ES2Real.Data;

using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;
using System.Threading.Tasks;

public class CustomAuthStateProvider : AuthenticationStateProvider
{
    private readonly UserSessionService _userSessionService;

    public CustomAuthStateProvider(UserSessionService userSessionService)
    {
        _userSessionService = userSessionService;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var usuario = _userSessionService.GetUsuario();
        var identity = usuario != null
            ? new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Name, usuario.Email),
                new Claim(ClaimTypes.Role, usuario.TipoUsuario)
            }, "apiauth")
            : new ClaimsIdentity();

        var user = new ClaimsPrincipal(identity);

        return new AuthenticationState(user);
    }

    // Método para notificar os componentes sobre alterações no estado de autenticação
    public void NotifyAuthenticationStateChanged()
    {
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }
}