public class UserSessionService
{
    private UtilizadorAuth? _usuarioLogado;

    public UtilizadorAuth? UsuarioLogado => _usuarioLogado;

    public void SetUsuario(UtilizadorAuth usuario)
    {
        _usuarioLogado = usuario;
        Console.WriteLine(_usuarioLogado.Email);
    }

    public void ClearUsuario()
    {
        _usuarioLogado = null;
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