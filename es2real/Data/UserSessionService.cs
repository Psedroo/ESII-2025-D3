public class UserSessionService
{
    private UtilizadorAuth? _usuarioLogado;

    public UtilizadorAuth? UsuarioAtual => _usuarioLogado;
    public void SetUsuario(UtilizadorAuth usuario)
    {
        _usuarioLogado = usuario;
        Console.WriteLine($" - Email: {_usuarioLogado?.Email}, Id: {_usuarioLogado?.Id}");
    }

    public void ClearUsuario()
    {
        _usuarioLogado = null;
    }

    public void Logout()
    {
        ClearUsuario();
        Console.WriteLine("Usuário desconectado.");
    }

    public UtilizadorAuth? GetUsuario()
    {
        if (_usuarioLogado == null)
        {
            Console.WriteLine("Usuário não logado.");
            return null;
        }
        return _usuarioLogado;
    }
    
    
}