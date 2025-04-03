namespace ES2Real.Data;

public class UserSessionService
{
    private UsuarioAuth? _usuarioAtual;

    public UsuarioAuth? UsuarioAtual => _usuarioAtual;

    public void SetUsuario(UsuarioAuth usuario)
    {
        _usuarioAtual = usuario;
    }

    public void Logout()
    {
        _usuarioAtual = null;
    }
}
