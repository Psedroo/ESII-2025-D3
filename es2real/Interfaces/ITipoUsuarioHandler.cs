namespace ES2Real.Interfaces;

public interface ITipoUsuarioHandler
{
    string TipoUsuarioSuportado { get; }
    Task CriarDadosEspecificosAsync(UtilizadorAuth utilizador);
}
