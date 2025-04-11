namespace ES2Real.Data;

public class UserSessionService
{
    private UtilizadorAuth? _usuarioAtual;

    public UtilizadorAuth? UsuarioAtual => _usuarioAtual;

    public void SetUsuario(UtilizadorAuth utilizador)
    {
        _usuarioAtual = utilizador;
    }

    public void Logout()
    {
        _usuarioAtual = null;
    }
}
